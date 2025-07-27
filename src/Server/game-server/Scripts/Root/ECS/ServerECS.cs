using Arch.System;
using Game.Shared.ECS;
using GameServer.Scripts.Root.ECS.Systems.Network.In;
using GameServer.Scripts.Root.ECS.Systems.Physics.In;
using GameServer.Scripts.Root.ECS.Systems.Physics.Out;
using GameServer.Scripts.Root.ECS.Systems.Process.In;
using GameServer.Scripts.Root.ECS.Systems.Process.Out;
using GameServer.Scripts.Root.Network;
using Godot;

namespace GameServer.Scripts.Root.ECS;

public partial class ServerECS : EcsRunner
{
    public static ServerECS Instance { get; private set; }

    /// --> Singleton instance for easy access

    private ServerNetwork GetServerNetwork() => ServerNetwork.Instance;
    
    public override void _Ready()
    {
        // impede instâncias duplicadas
        if (Instance != null && Instance != this)
        {
            GD.PushWarning("Duplicate ServerECS instance detected. Destroying the new one.");
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
                new ServerNetworkPollSystem(World, GetServerNetwork()),
                // 1) Inputs -> Process
                new ServerInputProcessSystem(World),
                // 2) Process -> Outputs
                new ServerOutputProcessSystem(World, GetServerNetwork()),
            ]
        );
        
        base.OnCreateProcessSystems(systems);
    }

    /// <inheritdoc/>
    protected override void OnCreatePhysicsSystems(List<ISystem<float>> systems)
    {
        systems.AddRange(
            [
                // 1) Inputs -> Physics
                new ServerInputPhysicsSystem(World),
                // 2) Physics -> Outputs
                new ServerOutputPhysicsSystem(World),
            ]
        );
        
        base.OnCreatePhysicsSystems(systems);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
