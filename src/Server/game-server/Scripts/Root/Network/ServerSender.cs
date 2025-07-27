using Game.Shared.Scripts.Network.Data;
using Game.Shared.Scripts.Network.Sender;
using LiteNetLib;
using LiteNetLib.Utils;

namespace GameServer.Scripts.Root.Network;

public sealed partial class ServerSender(
    NetManager netManager, 
    NetPacketProcessor packetProcessor) 
    : NetworkSender(netManager, packetProcessor)
{
    public void SendStateMessage(ref StateMessage message)
    {
        Broadcast(ref message);
    }
    public void BroadcastStateMessage(StateMessage[] message)
    {
        BroadcastArray(message);
    }
}
