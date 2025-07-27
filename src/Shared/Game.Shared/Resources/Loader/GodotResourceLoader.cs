using Godot;
using static Godot.ResourceLoader;

#nullable enable
namespace Game.Shared.Resources.Loader;
public class ResourceLoaderException : Exception
{
    public ResourceLoaderException(string message) : base(message)
    {
    }

    public ResourceLoaderException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public ResourceLoaderException() : base("An error occurred while loading a resource.")
    {
    }
}

/// <summary>
/// Serviço de carregamento de recursos para um client de jogo 2D em Godot 4.4.
/// </summary>
public sealed partial class GodotResourceLoader : GodotObject
{
    public static GodotResourceLoader Instance { get; } = new();
    public GodotResourceLoader()
    {
    }
    
    private const int DefaultTimeoutSeconds = 5;
    public T? LoadResource<T>(
        string path, 
        string typeHint = "",
        CacheMode cacheMode = CacheMode.Reuse) where T : GodotObject
    {
        if (!ValidatePath(path))
            return null;
        
        if (TryGetResourceAvailableFromCache<T>(path, cacheMode, out var resource))
            return resource;
        
        if (!ValidateResourceExists<T>(path, typeHint))
            return null;

        try
        {
            var loadedResource = ResourceLoader.Load<T>(path, typeHint, cacheMode);
            if (loadedResource is not null) 
                return loadedResource;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Exceção ao carregar recurso '{path}': {ex.Message}");
            return null;
        }
        
        GD.PrintErr($"Falha ao carregar recurso '{path}' do tipo {typeof(T).Name}.");
        return null;
    }
    
    /// <summary>
    /// Carrega um recurso de forma assíncrona.
    /// </summary>
    public async Task<T?> LoadAsync<T>(
        string path,
        string typeHint = "",
        Action<float>? onProgress = null,
        bool useSubThread = true,
        int timeoutSeconds = DefaultTimeoutSeconds,
        CacheMode cacheMode = CacheMode.Reuse,
        CancellationToken cancellationToken = default
    ) where T : GodotObject
    {
        if (!ValidatePath(path))
            return null;
        
        if (TryGetResourceAvailableFromCache<T>(path, cacheMode, out var cachedResource))
        {
            onProgress?.Invoke(1.0f);
            return cachedResource;
        }
        
        // Validação importante: verifica se o arquivo existe antes de iniciar o carregamento threaded
        if (!ValidateResourceExists<T>(path, typeHint))
            return null;
        
        // Inicia o loader assíncrono
        var loaderResult = LoadThreadedRequest(path, typeHint, useSubThread, cacheMode);
        
        if (loaderResult != Error.Ok)
        {
            GD.PrintErr($"Erro ao iniciar carregamento de '{path}': {loaderResult}");
            return null;
        }
        
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));

            return await PollLoadingStatus<T>(path, onProgress, timeoutCts.Token);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            GD.Print($"Carregamento de '{path}' foi cancelado pelo usuário.");
            // Cleanup do loader thread
            CleanupThreadedLoad(path);
            throw;
        }
        catch (OperationCanceledException)
        {
            // Cleanup do loader thread
            CleanupThreadedLoad(path);
            throw new TimeoutException($"Timeout carregando '{path}' após {timeoutSeconds}s");
        }
        catch (Exception ex) when (ex is not ResourceLoaderException)
        {
            // Cleanup do loader thread
            CleanupThreadedLoad(path);
            throw new ResourceLoaderException(
                $"Erro inesperado ao carregar recurso '{path}': {ex.Message}", ex
            );
        }
    }
    
    /// <summary>
    /// Carrega múltiplos recursos de forma assíncrona e paralela.
    /// </summary>
    public async Task<T[]> LoadMultipleResourcesAsync<T>(
        Tuple<string, string>[] pathsWithHints,
        Action<string, float>? onProgress = null,
        Action<float>? onOverallProgress = null,
        bool useSubThread = true,
        int timeoutSeconds = DefaultTimeoutSeconds,
        CacheMode cacheMode = CacheMode.Reuse,
        CancellationToken cancellationToken = default
    ) where T : Resource
    {
        if (pathsWithHints.Length == 0)
            return []; // C# 12 collection expressions

        var tasks = new Task<T?>[pathsWithHints.Length];
        var progressTracker = new float[pathsWithHints.Length];
        var lockObject = new object();
        
        for (int i = 0; i < pathsWithHints.Length; i++)
        {
            var (path, hint) = pathsWithHints[i];
            var index = i; // Captura o índice para evitar closure issues
            
            tasks[i] = LoadAsync<T>(
                path, 
                hint,
                progress =>
                {
                    // Atualiza o progresso individual
                    onProgress?.Invoke(path, progress);
                    
                    // Atualiza o progresso geral
                    lock (lockObject)
                    {
                        progressTracker[index] = progress;
                        var overallProgress = progressTracker.Sum() / pathsWithHints.Length;
                        onOverallProgress?.Invoke(overallProgress);
                    }
                }, 
                useSubThread,
                timeoutSeconds,
                cacheMode,
                cancellationToken
            );
        }

        try
        {
            var results = await Task.WhenAll(tasks);
            
            // Filtra recursos nulos e retorna apenas os válidos
            var validResults = new List<T>();
            for (int i = 0; i < results.Length; i++)
            {
                if (results[i] != null)
                    validResults.Add(results[i]!);
                else
                    GD.PrintErr($"Falha ao carregar recurso: {pathsWithHints[i]}");
            }

            return validResults.ToArray();
        }
        catch (Exception)
        {
            // Em caso de erro, tenta fazer cleanup de todos os paths
            foreach (var (path, _) in pathsWithHints)
                CleanupThreadedLoad(path);
            
            throw;
        }
    }

    private static bool ValidatePath(string path)
    {
        if (!string.IsNullOrWhiteSpace(path)) 
            return true;
        
        GD.PrintErr("Caminho do recurso não pode ser nulo ou vazio.");
        return false;
    }
    
    private static bool ValidateResourceExists<T>(string path, string typeHint = "") where T : GodotObject
    {
        if (ResourceLoader.Exists(path, typeHint)) 
            return true;
        
        GD.PrintErr($"Recurso '{path}' não encontrado ou não é do tipo {typeof(T).Name}.");
        return false;
    }

    /// <summary>
    /// Verifica se um recurso específico existe e é do tipo esperado.
    /// </summary>
    private static bool TryGetResourceAvailableFromCache<T>(string path, CacheMode cacheMode, out T? resource) where T : GodotObject
    {
        resource = null;
        
        if (cacheMode != CacheMode.Reuse)
            return false;
        
        // Verifica se o recurso já está em cache
        if (!ResourceLoader.HasCached(path))
            return false;

        resource = ResourceLoader.GetCachedRef(path) as T;
        return resource is not null;
    }

    private async Task<T?> PollLoadingStatus<T>(
        string path, 
        Action<float>? onProgress, 
        CancellationToken cancellationToken
    ) where T : GodotObject
    {
        if (Engine.GetMainLoop() is not SceneTree sceneTree)
            throw new ResourceLoaderException("SceneTree não está disponível");

        var godotArray = new global::Godot.Collections.Array();
        var lastProgress = 0f;

        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();
            
            var loadStatus = LoadThreadedGetStatus(path, godotArray);
            switch (loadStatus)
            {
                case ThreadLoadStatus.InProgress:
                {
                    // Atualiza progresso se disponível
                    if (godotArray.Count > 0)
                    {
                        var currentProgress = godotArray[0].AsSingle();
                        if (Math.Abs(lastProgress - currentProgress) > 0.01f)
                        {
                            lastProgress = currentProgress;
                            onProgress?.Invoke(lastProgress);
                        }
                    }

                    // Aguarda o próximo frame
                    await sceneTree.ToSignal(sceneTree, SceneTree.SignalName.ProcessFrame);
                    break;
                }

                case ThreadLoadStatus.Loaded:
                    // Carregamento concluído
                    onProgress?.Invoke(1.0f);
                    var loadedResource = LoadThreadedGet(path);
                    if (loadedResource is not T resource)
                        throw new ResourceLoaderException(
                            $"Recurso carregado de '{path}' não é do tipo esperado {typeof(T).Name}");
                    return resource;

                case ThreadLoadStatus.Failed:
                    throw new ResourceLoaderException($"Falha ao carregar '{path}'");

                case ThreadLoadStatus.InvalidResource:
                    throw new ResourceLoaderException($"Recurso inválido em '{path}'");

                default:
                    throw new ResourceLoaderException($"Status de carregamento inesperado: {loadStatus}");
            }
        }
    }

    /// <summary>
    /// Limpa um carregamento threaded que pode estar pendente.
    /// </summary>
    private static void CleanupThreadedLoad(string path)
    {
        // Cancela o carregamento se estiver pendente
        // A godot não fornece uma maneira direta de cancelar
        try
        {
            var status = LoadThreadedGetStatus(path);
            if (status == ThreadLoadStatus.InProgress)
            {
                // Tenta obter o recurso para limpar o thread
                LoadThreadedGet(path);
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Erro durante cleanup de '{path}': {ex.Message}");
        }
    }

    /// <summary>
    /// Pré-carrega recursos em background para uso futuro.
    /// </summary>
    public void PreloadResource(string path, string? hint = null)
    {
        if (!ValidatePath(path)) 
            return;

        var result = LoadThreadedRequest(path, hint ?? "", true, CacheMode.Reuse);
        if (result != Error.Ok)
            GD.PrintErr($"Erro ao iniciar pré-carregamento de '{path}': {result}");
    }

    /// <summary>
    /// Verifica se um recurso específico foi carregado com sucesso.
    /// </summary>
    public bool IsResourceLoaded(string path)
    {
        if (!ValidatePath(path)) 
            return false;
            
        var status = LoadThreadedGetStatus(path);
        return status == ThreadLoadStatus.Loaded;
    }

    /// <summary>
    /// Obtém o status atual de carregamento de um recurso.
    /// </summary>
    public (ThreadLoadStatus status, float progress) GetLoadingStatus(string path)
    {
        if (!ValidatePath(path)) 
            return (ThreadLoadStatus.InvalidResource, 0f);
            
        var array = new global::Godot.Collections.Array();
        var status = LoadThreadedGetStatus(path, array);
        var progress = array.Count > 0 ? array[0].AsSingle() : 0f;
        return (status, progress);
    }

    /// <summary>
    /// Cancela todos os carregamentos pendentes (útil ao sair do jogo).
    /// </summary>
    public void CancelAllPendingLoads()
    {
        // Nota: Godot não fornece uma maneira direta de cancelar todos os loads
        // Esta é uma funcionalidade que seria implementada mantendo uma lista
        // de paths sendo carregados, mas por simplicidade não foi implementada aqui
        GD.Print("Tentando cancelar carregamentos pendentes...");
    }
}
