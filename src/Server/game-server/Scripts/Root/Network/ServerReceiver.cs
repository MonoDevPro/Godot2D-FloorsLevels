using Game.Shared.Scripts.Network.Data;
using Game.Shared.Scripts.Network.Receiver;
using LiteNetLib;
using LiteNetLib.Utils;

namespace GameServer.Scripts.Root.Network;

public sealed class ServerReceiver : NetworkReceiver
{
    public event Action<InputMessage> InputMessageReceived;
    
    public ServerReceiver(NetPacketProcessor processor, 
        EventBasedNetListener listener) : base(processor,listener)
    {
        Initialize();
    }

    protected override void RegisterMessages()
    {
        SubscribeSerializableMessage<InputMessage, NetPeer>(ProcessMessage);
    }

    private void ProcessMessage(InputMessage message, NetPeer peer)
    {
        InputMessageReceived?.Invoke(message);
    }
}
