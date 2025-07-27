using Arch.Core;
using Arch.System;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.Network.Data;
using GameServer.Scripts.Root.Network;
using Godot;
using LiteNetLib;

namespace GameServer.Scripts.Root.ECS.Systems.Network.In;

public partial class ServerNetworkPollSystem(World world, ServerNetwork net) : BaseSystem<World, float>(world)
{
    private readonly NetManager _net = net.NetManager;
    private readonly ServerReceiver _netReceiver = net.Receiver;
    
    public override void Update(in float delta)
    {
        _net.PollEvents();
        base.Update(in delta);
    }


    public override void Initialize()
    {
        _netReceiver.InputMessageReceived += ApplyInputReceived;
        base.Initialize();
        
        GD.Print("ServerNetworkPollSystem initialized.");
    }
    
    public override void Dispose()
    {
        _netReceiver.InputMessageReceived -= ApplyInputReceived;
        base.Dispose();
    }
    
    private static readonly QueryDescription InputQueryDescription = new QueryDescription()
        .WithAll<NetworkedTag, InputCommandComponent>();
    private void ApplyInputReceived(InputMessage message)
    {
        World.InlineQuery<InputMessage, NetworkedTag, InputCommandComponent>(in InputQueryDescription, ref message);
    }
}