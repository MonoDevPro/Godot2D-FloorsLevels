using Game.Shared.Config;
using Godot;
using LiteNetLib;
using LiteNetLib.Utils;

namespace Game.Shared.Scripts.Network;

public abstract partial class NetworkManager : Node
{
    public readonly NetManager NetManager;
    protected readonly EventBasedNetListener _listener;
    protected readonly NetPacketProcessor _packetProcessor;
    
    private int MaxStringLength => NetworkConfigurations.MaxStringLength;
    
    protected NetworkManager()
    {
        _listener = new EventBasedNetListener();
        NetManager = new NetManager(_listener);
        _packetProcessor = new NetPacketProcessor(MaxStringLength);
    }
    
    public override void _Ready()
    {
    }
    
    public abstract void Start();

    /// <inheritdoc/>
    public virtual void Stop()
    {
        NetManager.DisconnectAll();
        NetManager.Stop();
    }
    
    // Obsolete: method for polling events, update to use the new ECS system instead.
    /*public void PollEvents()
    {
        _netManager.PollEvents();
    }*/
}
