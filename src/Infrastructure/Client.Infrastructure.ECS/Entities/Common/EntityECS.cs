using Arch.Core;
using Client.Infrastructure.ECS.Components.Common;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure.ECS.Entities.Common;

public abstract class EntityECS : IDisposable
{
    protected World World { get; }
    protected Entity Entity { get; }
    protected ILogger? Logger { get; }
    protected QueryDescription QueryDescription { get; } = new();
    
    private bool _disposed;

    protected EntityECS(World world, Entity e, ILogger? logger = null)
    {
        World = world ?? throw new ArgumentNullException(nameof(world), "World não pode ser nulo.");
        Entity = e;
        Logger = logger;
        
        Logger?.LogInformation("Criando entidade {EntityId} do tipo {EntityType}", e.Id, GetType().Name);
    }

    /// <summary>
    /// Subclasses devem implementar para registrar seus próprios components
    /// </summary>
    protected virtual void RegisterComponents()
    {
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) 
            return;
        
        if (disposing)
        {
            // limpa managed (event handlers, pools, etc.)
            Logger?.LogInformation("Destruindo entidade {EntityId}", Entity.Id);
            World.Destroy(Entity);
        }
        // limpa unmanaged, se houver
        _disposed = true;
    }

    /// <summary>
    /// Adiciona um component ECS genérico usando construtor padrão
    /// </summary>
    public bool AddComponent<T>() where T : IComponent, new()
    {
        if (HasComponent<T>())
            return false;
        
        Logger?.LogTrace($"Adicionando component {typeof(T).Name} na entidade {Entity.Id}.");
        World.Add(Entity, new T());
        return true;
    }

    /// <summary>
    /// Adiciona um component ECS genérico (instância fornecida)
    /// </summary>
    public bool AddComponent<T>(T component) where T : IComponent
    {
        if (HasComponent<T>())
            return false;

        Logger?.LogTrace("Adicionando component {Name} na entidade {EntityId}.", typeof(T).Name, Entity.Id);
        World.Add(Entity, component);
        return true;
    }

    /// <summary>
    /// Remove um component ECS da entidade
    /// </summary>
    public bool RemoveComponent<T>() where T : IComponent
    {
        if (!HasComponent<T>())
            return false;

        Logger?.LogTrace("Removendo component {Name} de {EntityId}.", typeof(T).Name, Entity.Id);
        World.Remove<T>(Entity);
        return true;
    }
    
    public void UpdateComponent<T>(T component) where T : IComponent
    {
        if (!HasComponent<T>())
        {
            Logger?.LogWarning("Tentativa de atualizar component {Name} que não existe na entidade {EntityId}.", typeof(T).Name, Entity.Id);
            return;
        }

        Logger?.LogTrace("Atualizando component {Name} na entidade {EntityId}.", typeof(T).Name, Entity.Id);
        World.Set(Entity, component);
    }
    
    public bool HasComponent<T>() where T : IComponent
    {
        if (World.Has<T>(Entity))
        {
            Logger?.LogTrace("Entidade {EntityId} possui o component {ComponentName}.", Entity.Id, typeof(T).Name);
            return true;
        }

        Logger?.LogWarning("Entidade {EntityId} não possui o component {ComponentName}.", Entity.Id, typeof(T).Name);
        return false;
    }
    
    public bool CheckAlive()
    {
        if (World.IsAlive(Entity))
            return true;

        Logger?.LogCritical("Entidade {EntityId} não está viva no ECS.", Entity.Id);
        return false;
    }
    
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    
    ~EntityECS()
    {
        Dispose(disposing: false);
    }
}
