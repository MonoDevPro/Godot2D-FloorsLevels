using Game.Shared.Scripts.Core.Config;
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
    }
    
    public override void Start()
    {
        NetManager.Start();
        NetManager.Connect(Host, Port, SecretKey);
    }
}
