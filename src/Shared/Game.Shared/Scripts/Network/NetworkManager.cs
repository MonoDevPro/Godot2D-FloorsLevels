using Game.Shared.Scripts.Core.Config;
using Game.Shared.Scripts.Network.Repository;
using Game.Shared.Scripts.Network.Transport;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network;

public abstract partial class NetworkManager : Node
{
    public readonly NetManager NetManager;
    private readonly EventBasedNetListener _listener;
    public readonly NetPacketProcessor Processor;

    public virtual NetworkSender Sender { get; }
    public virtual NetworkReceiver Receiver { get; }
    public virtual PeerRepositoryRef PeerRepository { get; }
    
    private int MaxStringLength => NetworkConfigurations.MaxStringLength;
    
    protected NetworkManager()
    {
        _listener = new EventBasedNetListener();
        NetManager = new NetManager(_listener);
        Processor = new NetPacketProcessor(MaxStringLength);
        
        Sender = new NetworkSender(NetManager, Processor);
        Receiver = new NetworkReceiver(Processor, _listener);
        PeerRepository = new PeerRepositoryRef(_listener, NetManager);
    }
    
    public abstract void Start();

    /// <inheritdoc/>
    public virtual void Stop()
    {
        NetManager.DisconnectAll();
        NetManager.Stop();
    }
    
    public void PollEvents()
    {
        NetManager.PollEvents();
    }
}
