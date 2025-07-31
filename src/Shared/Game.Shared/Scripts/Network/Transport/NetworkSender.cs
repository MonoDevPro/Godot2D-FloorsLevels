using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Transport;

public class NetworkSender(
    NetManager netManager, NetPacketProcessor packetProcessor)
{
    private readonly NetDataWriter _writer = new();
    
    public void Send<T>(ref T packet, int peerId = 0, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : unmanaged, INetSerializable
    {
        if (!netManager.TryGetPeerById(peerId, out var peer))
        {
            OnSendError(peerId);
            return;
        }
        
        packetProcessor.WriteNetSerializable<T>(_writer, ref packet);
        peer.Send(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    public void Broadcast<T>(ref T packet, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : struct, INetSerializable
    {
        packetProcessor.WriteNetSerializable<T>(_writer, ref packet);
        netManager.SendToAll(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    public void BroadcastArray<T>(T[] arrayPacket, DeliveryMethod method = DeliveryMethod.ReliableOrdered) 
        where T : struct, INetSerializable
    {
        GD.Print($"Broadcasting array of {typeof(T).Name} with length {arrayPacket.Length}.");
        
        for (int i = 0; i < arrayPacket.Length; i++)
            packetProcessor.WriteNetSerializable(_writer, ref arrayPacket[i]);
        
        netManager.SendToAll(_writer, method);
        _writer.Reset(); // Clear the writer to reuse it
    }
    
    public void BroadcastData(ReadOnlySpan<byte> data, DeliveryMethod method = DeliveryMethod.ReliableOrdered)
    {
        if (data.IsEmpty)
        {
            GD.PrintErr("[NetworkSender] Attempted to broadcast empty data.");
            return;
        }
        
        netManager.SendToAll(data, method);
    }
    
    public void SerializeData<T>(NetDataWriter writer, ref T packet)
        where T : struct, INetSerializable
    {
        packetProcessor.WriteNetSerializable<T>(_writer, ref packet);
    }
    
    protected virtual void OnSendError(int peerId)
    {
        // Pode ser sobrescrito para log ou reconexão
        GD.PrintErr($"[NetworkSender] Peer {peerId} não encontrado.");
    }
}
