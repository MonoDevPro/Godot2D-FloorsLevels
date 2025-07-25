using Game.Shared.Network.Common.Receiver;
using Game.Shared.Network.Common.Sender;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Network.Common;

public abstract partial class NetworkManager : Node
{
    protected readonly NetManager _netManager;
    protected readonly EventBasedNetListener _listener;
    protected readonly NetPacketProcessor _packetProcessor;
    
    protected NetworkManager()
    {
        _listener = new EventBasedNetListener();
        _netManager = new NetManager(_listener);
        _packetProcessor = new NetPacketProcessor();
    }
    
    public override void _Ready()
    {
    }
    
    public abstract void Start();

    /// <inheritdoc/>
    public virtual void Stop()
    {
        _netManager.DisconnectAll();
        _netManager.Stop();
    }
    
    /// <summary>
    /// Polls network events; should be called regularly (e.g., in a game loop).
    /// </summary>
    public void PollEvents()
    {
        _netManager.PollEvents();
    }
}
