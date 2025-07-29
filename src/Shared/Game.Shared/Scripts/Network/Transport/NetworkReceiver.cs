using Game.Shared.Scripts.Network.Seralization.Extensions;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Transport;

/// <summary>
/// Recebe pacotes e despacha para o NetPacketProcessor
/// </summary>
public class NetworkReceiver : IDisposable
{
    private readonly NetPacketProcessor _processor;
    private readonly EventBasedNetListener _listener;
    public event Action<int, int> LatencyUpdated;

    public NetworkReceiver(NetPacketProcessor processor, EventBasedNetListener listener)
    {
        _processor = processor;
        _listener = listener;
        Initialize();
    }

    private void Initialize()
    {
        RegisterCustomTypes();
        RegisterMessages();
        _listener.NetworkReceiveEvent += OnPacketReceived;
        _listener.NetworkLatencyUpdateEvent += OnLatencyReceived;
    }

    protected virtual void RegisterMessages()
    {
        // Common messages can be registered here
    }

    private void OnPacketReceived(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod method)
    {
        try
        {
            _processor.ReadAllPackets(reader, peer);
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[NetworkReceiver] Erro ao ler pacote: {ex}");
        }
        finally
        {
            reader.Recycle();
        }
    }

    private void OnLatencyReceived(NetPeer peer, int latency)
        => LatencyUpdated?.Invoke(peer.Id, latency);

    private void RegisterCustomTypes()
    {
        GodotTypeRegistry.RegisterBasicGodotTypes(_processor);
        ECSComponentRegistry.RegisterECSComponents(_processor);
        // outros tipos...
    }

    public IDisposable SubscribeSerializableMessage<T, TData>(Action<T, TData> onReceive)
        where T : unmanaged, INetSerializable
    {
        _processor.SubscribeNetSerializable(onReceive);
        return new DisposableAction(() => _processor.RemoveSubscription<T>());
    }

    public virtual void Dispose()
    {
        _listener.NetworkReceiveEvent -= OnPacketReceived;
        _listener.NetworkLatencyUpdateEvent -= OnLatencyReceived;
    }
}
