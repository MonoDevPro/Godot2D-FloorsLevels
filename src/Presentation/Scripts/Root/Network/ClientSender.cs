using Game.Shared.Scripts.Network.Data;
using Game.Shared.Scripts.Network.Sender;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace GodotFloorLevels.Scripts.Root.Network;

public sealed class ClientSender(
    NetManager netManager, 
    NetPacketProcessor packetProcessor) 
    : NetworkSender(netManager, packetProcessor)
{
    public void SendInputVector(Vector2 vector2)
    {
        var packet = new InputMessage {
            EntityId = 0, // Assuming 0 is the local player entity ID
            Input = vector2 };

        SendToServer(ref packet, DeliveryMethod.ReliableOrdered);
    }
    
    private void SendToServer<T>(ref T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : INetSerializable
            => Send(ref packet, 0, method); // Assuming server is always peer ID 0
}
