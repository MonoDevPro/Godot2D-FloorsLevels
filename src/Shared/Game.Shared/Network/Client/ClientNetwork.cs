using Game.Shared.Network.Common;
using Game.Shared.Network.Config;
using Godot;
using LiteNetLib;

namespace Game.Shared.Network.Client;

/// <summary>
/// Client-specific adapter implementing connection logic.
/// </summary>
public sealed partial class ClientNetwork : NetworkManager
{
    private string Host => NetworkConfigurations.Host;
    private int Port => NetworkConfigurations.Port;
    private string SecretKey => NetworkConfigurations.SecretKey;
    
    public ClientReceiver Receiver { get; private set; }
    public ClientSender Sender { get; private set; }
    
    public override void _Ready()
    {
        base._Ready();
        
        Receiver = new ClientReceiver(_packetProcessor, _listener);
        Sender = new ClientSender(_netManager, _packetProcessor);
        
        AddChild(Receiver);
        AddChild(Sender);
    }
    
    private void RegisterEvents()
    {
        _listener.PeerConnectedEvent += OnPeerConnected;
        _listener.PeerDisconnectedEvent += OnPeerDisconnected;
    }
    private void UnregisterEvents()
    {
        _listener.PeerConnectedEvent -= OnPeerConnected;
        _listener.PeerDisconnectedEvent -= OnPeerDisconnected;
    }
    
    private void OnPeerConnected(NetPeer peer)
    {
        GD.Print($"Connected to server: {peer.Address}");
    }
    
    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        GD.Print($"Disconnected from server: {peer.Address}, Reason: {disconnectInfo.Reason}");
    }
    
    public override void Start()
    {
        _netManager.Start();
        _netManager.Connect(Host, Port, SecretKey);
    }
}
