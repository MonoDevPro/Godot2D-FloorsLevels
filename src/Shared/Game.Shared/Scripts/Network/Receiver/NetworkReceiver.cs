using System.Diagnostics.CodeAnalysis;
using Game.Shared.Scripts.Network.Data;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network.Receiver;

public abstract class NetworkReceiver(NetPacketProcessor processor, EventBasedNetListener netListener)
{
    // Processador de pacotes para lidar com mensagens serializáveis
    // Network Events
    private readonly List<IDisposable> _subscriptions = [];      // Guarda disposables das inscrições de rede

    protected void Initialize()
    {
        // Register custom types for serialization
        RegisterCustomTypes();
        // Register messages packet when the node is ready
        RegisterMessages();
        // Register events to listen for incoming packets
        RegisterEvents();
    }
    
    /// <summary>
    /// Desfaz todas as inscrições.
    /// </summary>
    protected void DisconnectAll()
    {
        // 1) Remove inscrições de rede
        foreach (var sub in _subscriptions)
            sub.Dispose();
        _subscriptions.Clear();
    }

    private void RegisterEvents()
    {
        netListener.NetworkReceiveEvent += OnPacketReceived;
        netListener.NetworkLatencyUpdateEvent += OnLatencyReceived;
    }
    
    private void UnregisterEvents()
    {
        netListener.NetworkReceiveEvent -= OnPacketReceived;
        netListener.NetworkLatencyUpdateEvent -= OnLatencyReceived;
    }
    
    protected virtual void OnPacketReceived(NetPeer peer, NetPacketReader reader, byte channel, DeliveryMethod method)
    {
        // Process the packet using the NetPacketProcessor
        processor.ReadAllPackets(reader, peer);
        reader.Recycle(); // Recycle the reader to avoid memory leaks
    }
    
    protected virtual void OnLatencyReceived(NetPeer peer, int latency)
    { /* Handle ping response if needed */ }
    
    private void RegisterCustomTypes()
    {
        GodotTypeRegistry.RegisterBasicGodotTypes(processor);
        ECSComponentRegistry.RegisterECSComponents(processor);
        // … outros tipos
    }
    
    protected abstract void RegisterMessages();
    
    protected IDisposable SubscribeSerializableMessage<T, TData>(Action<T, TData> onReceive) 
        where T : INetSerializable, new()
    {
        processor.SubscribeNetSerializable(onReceive);
        
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name} with data {typeof(TData).Name}");
        });
        return disposable;
    }
    
    protected IDisposable SubscribeSerializableMessage<T, TData>(Action<T, TData> onReceive, Func<T> packetConstructor)
        where T : INetSerializable
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name} with data {typeof(TData).Name}");
        });
        
        processor.SubscribeNetSerializable(onReceive, packetConstructor);
        _subscriptions.Add(disposable);
        return disposable;
    }

    protected IDisposable SubscribeSerializableMessage<T>(Action<T> onReceive) 
        where T : INetSerializable, new()
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name}");
        });
        
        processor.SubscribeNetSerializable(onReceive);
        _subscriptions.Add(disposable);
        return disposable;
    }
    
    protected IDisposable SubscribeSerializableMessage<T>(Action<T> onReceive, Func<T> packetConstructor) 
        where T : INetSerializable
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name}");
        });
        
        processor.SubscribeNetSerializable(onReceive, packetConstructor);
        _subscriptions.Add(disposable);
        return disposable;
    }
    
    protected IDisposable SubscribeMessage<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T, TData>(
        Action<T, TData> onReceive)
        where T : class, new()
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name} with data {typeof(TData).Name}");
        });
        
        processor.SubscribeReusable(onReceive);
        _subscriptions.Add(disposable);
        return disposable;
    }
    
    protected IDisposable SubscribeMessage<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T>(
        Action<T> onReceive)
        where T : class, new()
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name}");
        });
        
        processor.SubscribeReusable(onReceive);
        _subscriptions.Add(disposable);
        return disposable;
    }
    
    protected IDisposable SubscribeMessage<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T>(
        Action<T> onReceive,
        Func<T> packetConstructor)
        where T : class, new()
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name}");
        });
        
        processor.Subscribe(onReceive, packetConstructor);
        _subscriptions.Add(disposable);
        return disposable;
    }

    protected IDisposable SubscribeMessage<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] T, TUserData>(
        Action<T, TUserData> onReceive,
        Func<T> packetConstructor)
        where T : class, new()
    {
        var disposable = new DisposableAction(() =>
        {
            processor.RemoveSubscription<T>(); // Remove a assinatura quando o IDisposable for descartado
            GD.Print($"Unsubscribed message from {typeof(T).Name} with data {typeof(TUserData).Name}");
        });
        
        // Return the disposable to allow unsubscribing later
        processor.Subscribe(onReceive, packetConstructor);
        _subscriptions.Add(disposable);
        return disposable;
    }
}
