using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Sender;

public abstract class NetworkSender(
    NetManager netManager, NetPacketProcessor packetProcessor)
{
    private readonly NetDataWriter _writer = new();
    
    protected void Send<T>(T packet, int peerId = 0, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : class, new()
    {
        if (!netManager.TryGetPeerById(peerId, out var peer))
        {
            GD.PrintErr($"Peer with ID {peerId} not found.");
            return;
        }
        
        packetProcessor.Write<T>(_writer, packet);
        peer.Send(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    protected void Send<T>(ref T packet, int peerId = 0, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : INetSerializable
    {
        if (!netManager.TryGetPeerById(peerId, out var peer))
        {
            GD.PrintErr($"Peer with ID {peerId} not found.");
            return;
        }
        
        packetProcessor.WriteNetSerializable<T>(_writer, ref packet);
        peer.Send(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    protected void Broadcast<T>(T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : class, new()
    {
        packetProcessor.Write<T>(_writer, packet);
        netManager.SendToAll(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    protected void Broadcast<T>(ref T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : INetSerializable
    {
        packetProcessor.WriteNetSerializable<T>(_writer, ref packet);
        netManager.SendToAll(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    protected void BroadcastArray<T>(T[] arrayPacket, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : INetSerializable
    {
        GD.Print($"Broadcasting array of {typeof(T).Name} with length {arrayPacket.Length}.");
        
        for (int i = 0; i < arrayPacket.Length; i++)
            packetProcessor.WriteNetSerializable(_writer, ref arrayPacket[i]);
        
        netManager.SendToAll(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
}
