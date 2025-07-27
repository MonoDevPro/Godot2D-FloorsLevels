#nullable enable
using Game.Shared.Resources.Loader;
using Godot;

namespace Game.Shared.Resources;

/// <summary>
/// Representa um caminho para recurso Godot do tipo T (Resource ou ConfigFile),
/// com helpers de carregamento síncrono e assíncrono.
/// </summary>
public readonly struct ResourcePath<T>(string path)
    where T : GodotObject
{
    public string Path { get; } = path;
    private GodotResourceLoader Loader => GodotResourceLoader.Instance;

    /// <summary>
    /// Carrega o recurso. Se T for ConfigFile, usa ConfigFile.Load(),
    /// senão delega ao GodotResourceLoader.
    /// </summary>
    /// <exception cref="ResourceLoaderException"/>
    public T Load()
    {
        // Caso especial para ConfigFile
        if (typeof(T) == typeof(ConfigFile))
        {
            var cfg = new ConfigFile();
            var err = cfg.Load(Path);
            if (err != Error.Ok)
                throw new ResourceLoaderException(
                    $"Falha ao carregar ConfigFile em '{Path}': {err}");
            return cfg as T
                   ?? throw new ResourceLoaderException(
                       $"ConfigFile carregado não pôde ser convertido para {typeof(T).Name}");
        }

        // Para todo outro Godot.Resource
        var res = Loader.LoadResource<T>(Path);
        if (res == null)
            throw new ResourceLoaderException(
                $"Não foi possível carregar recurso '{Path}' como {typeof(T).Name}.");
        return res;
    }

    /// <summary>
    /// Carregamento assíncrono (funciona apenas para Resource, não para ConfigFile).
    /// </summary>
    public Task<T?> LoadAsync(
        Action<float>? onProgress      = null,
        bool useSubThread              = true,
        int  timeoutSeconds            = 5,
        ResourceLoader.CacheMode cacheMode              = ResourceLoader.CacheMode.Reuse,
        CancellationToken cancellation = default)
    {
        if (typeof(T) == typeof(ConfigFile))
            throw new InvalidOperationException(
                "LoadAsync não está implementado para ConfigFile; use Load().");

        return Loader.LoadAsync<T>(
            Path,
            typeHint: "",
            onProgress,
            useSubThread,
            timeoutSeconds,
            cacheMode,
            cancellation
        );
    }

    /// <summary>
    /// Se T for PackedScene, instancia um nó do tipo TNode.
    /// </summary>
    /// <typeparam name="TNode"></typeparam>
    public TNode Instantiate<TNode>() where TNode : Node
    {
        if (typeof(T) != typeof(PackedScene))
            throw new ResourceLoaderException(
                "Instantiate só faz sentido quando T == PackedScene");

        var packed = Load() as PackedScene
                     ?? throw new ResourceLoaderException(
                         $"PackedScene não encontrado em '{Path}'");

        return packed.Instantiate<TNode>();
    }

    public override string ToString() => Path;
}
