using Game.Shared.Network.Common.Data;
using Game.Shared.Network.Common.Receiver;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Client;

public sealed partial class ClientReceiver(
    NetPacketProcessor processor, 
    EventBasedNetListener listener) : NetworkReceiver(processor,listener)
{
    protected override void RegisterMessages()
    {
        SubscribeSerializableMessage<StateMessage, NetPeer>(ProcessMessage);
    }

    // State message handler
    [Signal] public delegate void StateReceivedEventHandler(int entityId, Vector2 position);
    private void ProcessMessage(StateMessage message, NetPeer peer)
    {
        EmitSignal(SignalName.StateReceived, message.EntityId, message.Position);
    }
}
