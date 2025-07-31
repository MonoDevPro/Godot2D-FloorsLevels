using Arch.Core;
using Arch.System;
using Godot;

namespace Game.Shared.Scripts.ECS;

public abstract partial class EcsRunner : Node
{
    public World World { get; private set; }
    
    // Network groups - executam em ordem específica
    private readonly Group<float> _processGroup = new("ProcessGroup");
    private readonly Group<float> _physicsGroup = new("PhysicsGroup");

    public override void _Ready()
    {
        World = World.Create();
        
        GD.Print("[Arch ECS] Criando mundo ECS...");
        
        // Inicializa sistemas de processo
        var updateSystems = new List<ISystem<float>>();
        OnCreateProcessSystems(updateSystems);
        if (updateSystems.Count > 0)
        {
            _processGroup.Add(updateSystems.ToArray());
            _processGroup.Initialize();
            GD.Print($"[Arch ECS] {_processGroup.Name} systems initialized. Count: {updateSystems.Count}.");
        }

        // Inicializa sistemas de física
        var physicsSystems = new List<ISystem<float>>();
        OnCreatePhysicsSystems(physicsSystems);
        if (physicsSystems.Count > 0)
        {
            _physicsGroup.Add(physicsSystems.ToArray());
            _physicsGroup.Initialize();
            GD.Print($"[Arch ECS] {_physicsGroup.Name} systems initialized. Count: {physicsSystems.Count}.");
        }
        
        GD.Print("[Arch ECS] Mundo ECS criado com sucesso");
    }
    
    /// <summary>
    /// Override para registrar sistemas que rodam em _Process
    /// </summary>
    protected virtual void OnCreateProcessSystems(List<ISystem<float>> systems)
    {
        // Ex: systems.Add(new YourRenderingSystem(World));
    }
    
    /// <summary>
    /// Override para registrar sistemas que rodam em _PhysicsProcess
    /// </summary>
    protected virtual void OnCreatePhysicsSystems(List<ISystem<float>> systems)
    {
        // Ex: systems.Add(new PhysicsMovementSystem(World));
    }
    
    /// <summary>
    /// Atualiza sistemas de processo com pipeline de rede correta
    /// Ordem: NetworkReceive -> Process -> NetworkPublish
    /// </summary>
    public virtual void UpdateProcessSystems(float delta)
    {
        _processGroup.BeforeUpdate(delta);
        _processGroup.Update(delta);
        _processGroup.AfterUpdate(delta);
    }
    
    /// <summary>
    /// Atualiza apenas sistemas de física
    /// </summary>
    public virtual void UpdatePhysicsSystems(float delta)
    {
        _physicsGroup.BeforeUpdate(delta);
        _physicsGroup.Update(delta);
        _physicsGroup.AfterUpdate(delta);
    }

    public override void _ExitTree()
    {
        _processGroup?.Dispose();
        _physicsGroup?.Dispose();
        World?.Dispose();
        GD.Print("[Arch ECS] ECS Runner exited and resources cleaned up.");
    }
}
