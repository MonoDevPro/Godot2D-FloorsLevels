using Arch.System;
using Game.Shared.Scripts.ECS;
using Game.Shared.Scripts.ECS.NetworkSync;
using Godot;
using GodotFloorLevels.Scripts.Root.ECS.Network;
using GodotFloorLevels.Scripts.Root.ECS.Systems.Physics;
using GodotFloorLevels.Scripts.Root.ECS.Systems.Process;
using GodotFloorLevels.Scripts.Root.Network;

namespace GodotFloorLevels.Scripts.Root.ECS;

public partial class ClientECS : EcsRunner
{
    public static ClientECS Instance { get; private set; }

    /// --> Singleton instance for easy access

    private ClientNetwork GetClientNetwork() => ClientNetwork.Instance;
    
    public override void _Ready()
    {
        // impede instâncias duplicadas
        if (Instance != null && Instance != this)
        {
            GD.PrintErr("Duplicate ClientECS instance detected. Destroying the new one.");
            QueueFree();
            return;
        }

        // define singleton
        Instance = this;

        base._Ready();
    }
    
    protected override void OnCreateProcessSystems(List<ISystem<float>> systems)
    {
        systems.AddRange(
            [
                // 1) Always poll network first
                NetworkReceiveGroup.AddStateSyncSystem(World, GetClientNetwork()),
                // 2) Inputs Local -> Process
                new ClientInputProcessSystem(World, GetClientNetwork()),
                new NetworkPublisherSystem(World, GetClientNetwork())
            ]
        );
        
        base.OnCreateProcessSystems(systems);
    }

    /// <inheritdoc/>
    protected override void OnCreatePhysicsSystems(List<ISystem<float>> systems)
    {
        systems.AddRange(
            [
                // 1) Inputs Process -> Physics Inputs
                new ClientInputPhysicsSystem(World),
                // 2) Physics Process -> Network Reconciliation
                new ClientOutputPhysicsSystem(World),
            ]
        );
        
        base.OnCreatePhysicsSystems(systems);
    }
}
