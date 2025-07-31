using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Data.Input;
using Game.Shared.Scripts.Network.Data.Join;
using Godot;
using LiteNetLib;

namespace GameServer.Scripts.Root.ECS.Systems.Network;

// Renomeamos o sistema para refletir sua única responsabilidade
public partial class NetworkToCommandSystem : BaseSystem<World, float>
{
    private PlayerSpawner PlayerSpawner => ServerBootstrap.Instance.PlayerSpawner;
    
    // ... (mesmo construtor e NetPacketProcessor de antes) ...
    private readonly NetworkManager _serverManager;

    public NetworkToCommandSystem(World world, NetworkManager serverManager) : base(world) 
    {
        _serverManager = serverManager;
        
        serverManager.Receiver.RegisterMessageHandler<InputRequest>(OnPlayerInputReceived);
    }
    
    public override void Update(in float t) => _serverManager.PollEvents();

    private void OnPlayerInputReceived(InputRequest packet, NetPeer peer)
    {
        if (!PlayerSpawner.TryGetEntityById(peer.Id, out var entity))
            return;
        
        World.Add(entity, new InputRequestCommand { Value = packet.Value });
    }
}