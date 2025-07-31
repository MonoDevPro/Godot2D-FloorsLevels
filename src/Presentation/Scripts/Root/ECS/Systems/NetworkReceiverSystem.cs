using System.Runtime.CompilerServices;
using Arch.Core;
using Game.Shared.Scripts.ECS.Components;
using Game.Shared.Scripts.ECS.NetworkSync;
using Game.Shared.Scripts.Network;
using Game.Shared.Scripts.Network.Data.Input;
using Game.Shared.Scripts.Network.Data.Sync;
using Game.Shared.Scripts.Network.Transport;
using LiteNetLib;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems;

public sealed class NetworkReceiverSystem(World world, NetworkManager manager) : LambdaNetReceiveSystem(world, manager)
{
    private readonly QueryDescription _stateDesc = new QueryDescription()
        .WithAll<NetworkedTag, VelocityComponent, PositionComponent>();
    
    protected override List<IDisposable> RegisterNetworkHandlers(NetworkReceiver receiver)
    {
        var subscriptions = new List<IDisposable>();
        
        // Registra o handler para receber mensagens de estado
        subscriptions.Add(receiver.RegisterMessageHandler<InputResponse>(OnStateReceived));
        
        return subscriptions;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void OnStateReceived(InputResponse data, NetPeer peer)
    {
        World.Query(_stateDesc, (
            ref NetworkedTag tag, 
            ref VelocityComponent velocity, 
            ref PositionComponent position ) =>
        {
            if (tag.Id != data.Id) return;
            velocity.Value = data.NewVelocity;
            position.Value = data.NewPosition;
        });
    }
}
