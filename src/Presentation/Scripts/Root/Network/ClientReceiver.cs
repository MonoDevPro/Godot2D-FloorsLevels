using Game.Shared.Scripts.Network.Data;
using Game.Shared.Scripts.Network.Receiver;
using LiteNetLib;
using LiteNetLib.Utils;

namespace GodotFloorLevels.Scripts.Root.Network;

public sealed class ClientReceiver : NetworkReceiver
{
    public event Action<StateMessage> StateMessageReceived;
    
    public ClientReceiver(NetPacketProcessor processor, EventBasedNetListener listener) : base(processor,listener)
    {
        Initialize();
    }
    
    protected override void RegisterMessages()
    {
        SubscribeSerializableMessage<StateMessage, NetPeer>(ProcessMessage);
    }

    // State message handler
    private void ProcessMessage(StateMessage message, NetPeer peer)
    {
        StateMessageReceived?.Invoke(message);
    }
}
