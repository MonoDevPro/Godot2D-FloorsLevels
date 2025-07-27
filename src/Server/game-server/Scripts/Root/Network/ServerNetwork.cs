using Game.Shared.Config;
using Game.Shared.Scripts.Network;
using Godot;

namespace GameServer.Scripts.Root.Network;

/// <summary>
/// Client-specific adapter implementing connection logic.
/// </summary>
public partial class ServerNetwork: NetworkManager
{
    public static ServerNetwork Instance { get; private set; } /// --> Singleton instance for easy access

    public PeerRepositoryRef PeerRepository { get; private set; }
    public ServerReceiver    Receiver       { get; private set; }
    public ServerSender      Sender         { get; private set; }
    
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
        Receiver       = new ServerReceiver(_packetProcessor, _listener);
        Sender         = new ServerSender(NetManager, _packetProcessor);
        PeerRepository = new PeerRepositoryRef(_listener, NetManager);
        
        GD.Print("[ServerNetwork] Ready");
    }
    public override void Start()
    {
        var port = NetworkConfigurations.Port;
        
        PeerRepository.Start();
        NetManager.Start(port);
    }
    
    public override void Stop()
    {
        base.Stop();
        PeerRepository.Stop();
    }
}
