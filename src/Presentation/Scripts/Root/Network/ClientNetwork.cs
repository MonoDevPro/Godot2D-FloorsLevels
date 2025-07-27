using Game.Shared.Config;
using Game.Shared.Scripts.Network;
using Godot;
using LiteNetLib;

namespace GodotFloorLevels.Scripts.Root.Network;

/// <summary>
/// Client-specific adapter implementing connection logic.
/// </summary>
public sealed partial class ClientNetwork : NetworkManager
{
    public static ClientNetwork Instance { get; private set; } /// --> Singleton instance for easy access
    
    private string Host => NetworkConfigurations.Host;
    private int Port => NetworkConfigurations.Port;
    private string SecretKey => NetworkConfigurations.SecretKey;
    
    public ClientReceiver Receiver { get; private set; }
    public ClientSender Sender { get; private set; }
    
    public override void _Ready()
    {
        // 1) impede instâncias duplicadas
        if (Instance != null && Instance != this)
        {
            GD.PushWarning("Duplicate ServerNetwork singleton detected. Destroying the new one.");
            QueueFree();
            return;
        }

        // 2) define singleton
        Instance = this;

        base._Ready();

        // 3) cria os componentes
        Receiver = new ClientReceiver(_packetProcessor, _listener);
        Sender = new ClientSender(NetManager, _packetProcessor);
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
        NetManager.Start();
        NetManager.Connect(Host, Port, SecretKey);
    }
}
