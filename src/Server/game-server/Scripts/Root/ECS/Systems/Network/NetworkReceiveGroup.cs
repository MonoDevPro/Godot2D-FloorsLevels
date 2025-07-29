using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.ECS.NetworkSync;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Transport.Data;

namespace GameServer.Scripts.Root.ECS.Systems.Network;

public class NetworkSyncGroup
{
    private readonly Group<float> _group;
    
    public NetworkSyncGroup(World world, NetworkManager manager)
    {
        _group = new Group<float>("SyncNetworkGroup");

        AddInputSyncSystem(world, manager);
        
        _group.Initialize();
    }
    
    public void Push(World world, float delta)
    {
        _group.BeforeUpdate(delta);
        _group.Update(delta);
        _group.AfterUpdate(delta);
    }
    public void Dispose()
    {
        _group.Dispose();
    }
    
    private void AddInputSyncSystem(World world, NetworkManager manager)
    {
        _group.Add(new LambdaNetReceiveSystem<InputMessage>(world, manager, (w, msg, peerId) =>
        {
            QueryDescription query = new QueryDescription()
                .WithAll<NetworkedTag, PositionComponent, VelocityComponent>();
            world.Query(query,
                (ref NetworkedTag net, ref InputCommandComponent cmd) =>
                {
                    if (net.Id != peerId)
                        return;
                    cmd.Value = msg.Value;
                });
        }, 32, 32));
    }
    
}