using Game.Shared.Scripts.Core.Config;
using Game.Shared.Scripts.Network;
using Godot;

namespace GameServer.Scripts.Root.Network;

/// <summary>
/// Client-specific adapter implementing connection logic.
/// </summary>
public partial class ServerNetwork: NetworkManager
{
    public static ServerNetwork Instance { get; private set; } /// --> Singleton instance for easy access
    
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
