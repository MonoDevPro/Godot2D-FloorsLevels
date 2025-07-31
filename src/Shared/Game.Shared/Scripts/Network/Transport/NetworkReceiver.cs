using Arch.Core;
using LiteNetLib;
using LiteNetLib.Utils;
using Godot;
using System.Collections.Concurrent;
using Arch.LowLevel;

namespace Game.Shared.Scripts.Network.Transport;

/// <summary>
/// Responsável por receber e processar mensagens da rede no contexto do ECS
/// </summary>
public class NetworkReceiver
{
    private readonly NetPacketProcessor _processor;
    private readonly EventBasedNetListener _listener;

    public NetworkReceiver(NetPacketProcessor processor, EventBasedNetListener listener)
    {
        _processor = processor ?? throw new ArgumentNullException(nameof(processor));
        _listener = listener ?? throw new ArgumentNullException(nameof(listener));
        
        // Registra callback para recebimento de mensagens
        _listener.NetworkReceiveEvent += OnNetworkReceive;
    }

    private void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod deliveryMethod)
    {
        try
        {
            // Processa o pacote e enfileira a mensagem
            _processor.ReadAllPackets(reader, peer);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[NetworkReceiver] Erro ao processar pacote: {ex.Message}");
        }
    }
    
    public IDisposable RegisterMessageHandler<T>(Action<T, NetPeer> callback) where T : struct, INetSerializable
    {
        _processor.SubscribeNetSerializable(callback);
        return new DisposableAction(() =>
        {
            _processor.RemoveSubscription<T>();
            GD.Print($"[NetworkReceiver] Unregistered handler for {typeof(T).Name}");
        });
    }

    public void Dispose()
    {
        _listener.NetworkReceiveEvent -= OnNetworkReceive;
        GD.Print("[NetworkReceiver] Disposed");
    }
}
