using Arch.Core;
using Arch.System;
using Game.Shared.ECS.Components;
using Game.Shared.ECS.Systems;
using Godot;

namespace GameServer;

public partial class ServerBootstrap : Node
{
    private ServerECS _ecsRunner;
    
    private NetworkManager _networkManager;

    public override void _Ready()
    {
        // ECS
        _ecsRunner = GetNode<ServerECS>("ServerECS");
        
        // Network
        _networkManager = GetNode<NetworkManager>("NetworkManager");
        
        GD.Print("[Server] Bootstrap complete");
    }
}