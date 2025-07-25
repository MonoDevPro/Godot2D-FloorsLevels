using Game.Shared.ECS;
using Godot;
using GodotFloorLevels.Scripts.ECS;
using GodotFloorLevels.Scripts.Godot.Entities.Repositories;
using GodotFloorLevels.Scripts.Godot.Resources;

namespace GodotFloorLevels.Scripts.Godot.Scenes.Game;

public sealed partial class GameRoot : Node
{
    [Export] public GameScenesResource GameScenes { get; set; }
    public static GameRoot Instance { get; private set; }
    
    private EcsRunner EcsRunner { get; set; }
    
    public CharacterRepository CharacterRepository { get; } = new();
    
    public override void _Ready()
    {
        // Ensure this is a singleton instance
        if (Instance != null)
        {
            GD.PrintErr("[GameRoot] Instance already exists. This should be a singleton.");
            return;
        }
        
        Instance = this;
        
        // Initialize the ECS runner
        EcsRunner = new EcsRunner();
    }
    
    public override void _Process(double delta)
    {
        // Update the ECS world
        EcsRunner.Update((float)delta);
    }
    
    public override void _ExitTree()
    {
        Instance = null; // Clear the singleton instance
        
        // Dispose of the ECS world and update runner
        GD.Print("[GameRoot] Exiting and disposing ECS Runner...");
        EcsRunner.Dispose();
        EcsRunner = null;
        
        base._ExitTree();
    }
}
