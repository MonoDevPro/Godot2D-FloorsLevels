using Game.Shared.Network.Common;
using Game.Shared.Network.Common.Sender;
using Game.Shared.Network.Config;
using Godot;
using LiteNetLib;

namespace Game.Shared.Network.Server;

/// <summary>
/// Client-specific adapter implementing connection logic.
/// </summary>
public sealed partial class ServerNetwork: NetworkManager
{
    private int Port => NetworkConfigurations.Port;
    private string SecretKey => NetworkConfigurations.SecretKey;
    private int MaxClients => NetworkConfigurations.MaxClients;
    
    public ServerReceiver Receiver { get; private set; }
    public ServerSender Sender { get; private set; }
    
    public override void _Ready()
    {
        base._Ready();
        
        Receiver = new ServerReceiver(_packetProcessor, _listener);
        Sender = new ServerSender(_netManager, _packetProcessor);
        
        AddChild(Receiver);
        AddChild(Sender);
    }
    private void RegisterEvents()
    {
        _listener.PeerConnectedEvent += OnPeerConnected;
        _listener.PeerDisconnectedEvent += OnPeerDisconnected;
        _listener.ConnectionRequestEvent += OnConnectionRequest;
    }

    private void UnregisterEvents()
    {
        _listener.PeerConnectedEvent -= OnPeerConnected;
        _listener.PeerDisconnectedEvent -= OnPeerDisconnected;
        _listener.ConnectionRequestEvent -= OnConnectionRequest;
    }
    
    private void OnPeerConnected(NetPeer peer)
    {
        GD.Print($"Client connected: {peer.Address}");
    }
    private void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        GD.Print($"Client disconnected: {peer.Address}, Reason: {disconnectInfo.Reason}");
    }
    private void OnConnectionRequest(ConnectionRequest request)
    {
        if (_netManager.ConnectedPeersCount < MaxClients)
            request.AcceptIfKey(SecretKey);
        else
            request.Reject();
    }
    
    public override void Start()
    {
        _netManager.Start(Port);
    }
}
