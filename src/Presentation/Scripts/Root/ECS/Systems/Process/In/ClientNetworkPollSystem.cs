using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Network.Data;
using GodotFloorLevels.Scripts.Root.Network;
using LiteNetLib;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems.Process.In;

public partial class ClientNetworkPollSystem(World world, ClientNetwork net) : BaseSystem<World, float>(world)
{
    private readonly NetManager _net = net.NetManager;
    private readonly ClientReceiver _netReceiver = net.Receiver;
    
    public override void Update(in float delta)
    {
        _net.PollEvents();
        base.Update(in delta);
    }
    
    public override void Initialize()
    {
        _netReceiver.StateMessageReceived += ApplyStateReceived;
        base.Initialize();
    }
    
    public override void Dispose()
    {
        _netReceiver.StateMessageReceived -= ApplyStateReceived;
        base.Dispose();
    }
    
    private static readonly QueryDescription StateQueryDescription = new QueryDescription()
        .WithAll<NetworkedTag, PositionComponent, VelocityComponent>();
    private void ApplyStateReceived(StateMessage received)
    {
        World.InlineQuery<StateMessage, NetworkedTag, PositionComponent, VelocityComponent>(in StateQueryDescription, ref received);
    }
}
