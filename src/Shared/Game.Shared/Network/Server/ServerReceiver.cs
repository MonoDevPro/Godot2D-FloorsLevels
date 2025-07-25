using Game.Shared.Network.Common.Data;
using Game.Shared.Network.Common.Receiver;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Server;

public sealed partial class ServerReceiver(
    NetPacketProcessor processor, 
    EventBasedNetListener listener) : NetworkReceiver(processor,listener)
{
    protected override void RegisterMessages()
    {
        SubscribeSerializableMessage<InputMessage, NetPeer>(ProcessMessage);
    }

    // Input message handler
    [Signal] public delegate void InputReceivedEventHandler(int entityId, Vector2 input);
    private void ProcessMessage(InputMessage message, NetPeer peer)
    {
        EmitSignal(SignalName.InputReceived, message.EntityId, message.Input);
    }
}
