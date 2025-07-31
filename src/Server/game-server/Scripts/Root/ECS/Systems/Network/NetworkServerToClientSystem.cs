using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Network.Data.Sync;
using LiteNetLib;
using LiteNetLib.Utils;

namespace GameServer.Scripts.Root.ECS.Systems.Network;

public partial class NetworkServerToClientSystem : BaseSystem<World, float>
{
    private readonly PlayerSpawner _spawner;
    private readonly NetDataWriter _writer = new();

    public NetworkServerToClientSystem(World world, PlayerSpawner spawner) : base(world)
    {
        _spawner = spawner;
    }

    public override void BeforeUpdate(in float t)
    {
        _writer.Reset();
        
        base.BeforeUpdate(in t);
    }

    [Query]
    [All<NetworkedTag>]
    private void SyncPlayerPositionToClient(in NetworkedTag tag, SceneBodyRefComponent body)
    {
        // Verifica se o cliente está conectado
        if (!_spawner.TryGetPlayerById(tag.Id, out var player))
            return;

        var packet1 = new PositionSyncMessage { NetId = tag.Id, Position = body.Value.GlobalPosition, Velocity = body.Value.Velocity };
        _spawner.Sender.SerializeData(_writer, ref packet1);
    }

    public override void AfterUpdate(in float t)
    {
        // Envia os dados serializados para todos os clientes conectados
        if (_writer.Length > 0)
            _spawner.Sender.BroadcastData(_writer.AsReadOnlySpan(), DeliveryMethod.Unreliable);
        
        _spawner.NetworkManager.PollEvents();
        
        base.AfterUpdate(in t);
    }
}