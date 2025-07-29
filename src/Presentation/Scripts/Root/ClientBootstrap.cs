using GodotFloorLevels.Scripts.Resources;
using Godot;
using GodotFloorLevels.Scripts.Config;
using GodotFloorLevels.Scripts.Root.ECS;
using GodotFloorLevels.Scripts.Root.Network;

namespace GodotFloorLevels.Scripts.Root;

public sealed partial class ClientBootstrap : Node
{
    public ClientNetwork ClientNetwork { get; private set; }
    public ClientECS ClientECS { get; private set; }
    public PlayerSpawner PlayerSpawner { get; private set; }

    public static ClientBootstrap Instance { get; private set; }
    
    public override void _Ready()
    {
        // Ensure this is a singleton instance
        if (Instance != null)
        {
            GD.PrintErr("[GameRoot] Instance already exists. This should be a singleton.");
            return;
        }
        
        Instance = this;
        
        ClientNetwork = GetNode<ClientNetwork>(nameof(ClientNetwork));
        ClientECS = GetNode<ClientECS>(nameof(ClientECS));
        PlayerSpawner = GetNode<PlayerSpawner>(nameof(PlayerSpawner));
        
        GD.Print("[Client] Bootstrap complete");
        
        ClientNetwork.Start();

        PlayerSpawner.CreatePlayer(0);
        
        
        GodotInputMap.SetupDefaultInputs();
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        
        // Update Network
        ClientNetwork.PollEvents();
        
        // Update Process systems
        ClientECS.UpdateProcessSystems((float)delta);
    }
    
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        
        // Update Physics systems
        ClientECS.UpdatePhysicsSystems((float)delta);
    }
}
