using Arch.Core;
using Arch.System;
using Godot;

namespace Game.Shared.Scripts.ECS;

public abstract partial class EcsRunner : Node
{
    public World World { get; private set; }
    
    // Network
    
    private readonly Group<float> _processGroup = new("ProcessGroup");
    private readonly Group<float> _physicsGroup = new("PhysicsGroup");

    public override void _Ready()
    {
        World = World.Create();
        
        // Cria sistemas de _Process
        var updateSystems = new List<ISystem<float>>{
            // Add your common systems here, e.g.:
            // new ExampleSystem(World),
            // new AnotherSystem(World),
            // new YetAnotherSystem(World)
        };
        OnCreateProcessSystems(updateSystems);
        _processGroup.Add(updateSystems.ToArray());
        _processGroup.Initialize();
        GD.Print($"[Arch ECS] {_processGroup.Name} systems initialized. Count: {updateSystems.Count}.");

        // Cria sistemas de _PhysicsProcess
        var physicsSystems = new List<ISystem<float>>{
            // Add your common systems here, e.g.:
            // new ExampleSystem(World),
            // new AnotherSystem(World),
            // new YetAnotherSystem(World)
        };
        OnCreatePhysicsSystems(physicsSystems);
        _physicsGroup.Add(physicsSystems.ToArray());
        _physicsGroup.Initialize();
        GD.Print($"[Arch ECS] {_physicsGroup.Name} systems initialized. Count: {physicsSystems.Count}.");

        GD.Print("[Arch ECS] Mundo ECS criado");
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
    
    public virtual void UpdateProcessSystems(float delta)
    {
        _processGroup.BeforeUpdate(delta);
        _processGroup.Update(delta);
        _processGroup.AfterUpdate(delta);
    }
    
    public virtual void UpdatePhysicsSystems(float delta)
    {
        _physicsGroup.BeforeUpdate(delta);
        _physicsGroup.Update(delta);
        _physicsGroup.AfterUpdate(delta);
    }

    public override void _ExitTree()
    {
        _processGroup.Dispose();
        _physicsGroup.Dispose();
        World.Dispose();
        GD.Print("ECS Runner exited and resources cleaned up.");
    }
}
