using Game.Shared.Network.Common.Sender;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Server;

public sealed partial class ServerSender(
    NetManager netManager, 
    NetPacketProcessor packetProcessor) 
    : NetworkSender(netManager, packetProcessor)
{
    public void SendInput()
    {
        
    }
}
