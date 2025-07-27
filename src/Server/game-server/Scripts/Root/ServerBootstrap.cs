using GameServer.Scripts.Root.ECS;
using GameServer.Scripts.Root.Network;
using Godot;

namespace GameServer.Scripts.Root;

public sealed partial class ServerBootstrap : Node
{
    public ServerNetwork ServerNetwork { get; private set; }
    public ServerECS ServerECS { get; private set; }
    
    public static ServerBootstrap Instance { get; private set; }  /// --> Singleton instance for easy access

    public override void _Ready()
    {
        // Ensure this is a singleton instance
        if (Instance != null)
        {
            GD.PrintErr("[GameRoot] Instance already exists. This should be a singleton.");
            QueueFree(); // Remove this instance if it already exists
            return;
        }
        
        Instance = this;
        
        ServerNetwork = GetNode<ServerNetwork>(nameof(ServerNetwork));
        ServerECS = GetNode<ServerECS>(nameof(ServerECS));
        
        // Start the ECS and Network systems
        ServerNetwork.Start();
        
        GD.Print("[Server] Bootstrap complete");
    }
}
