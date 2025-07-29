using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.ECS.NetworkSync;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Transport.Data;

namespace GodotFloorLevels.Scripts.Root.ECS.Network;

public static class NetworkReceiveGroup
{
    public static ISystem<float> AddStateSyncSystem(World world, NetworkManager manager)
    {
        return new LambdaNetReceiveSystem<StateMessage>(world, manager, (w, msg, peerId) =>
        {
            QueryDescription query = new QueryDescription()
                .WithAll<NetworkedTag, PositionComponent, VelocityComponent>();
            world.Query(query,
                (ref NetworkedTag net, ref PositionComponent pos, ref VelocityComponent vel) =>
                {
                    if (net.Id != msg.Id)
                        return;
                    pos.Value = msg.NewPosition;
                    vel.Value = msg.NewVelocity;
                });
        }, 32, 64);
    }
    
}
