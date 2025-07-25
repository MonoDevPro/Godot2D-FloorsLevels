using Game.Shared.Network.Common.Sender;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Client;

public sealed partial class ClientSender(
    NetManager netManager, 
    NetPacketProcessor packetProcessor) 
    : NetworkSender(netManager, packetProcessor)
{
    public void SendInput()
    {
        
    }
}
