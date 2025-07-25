using Godot;
using GodotFloorLevels.Scripts.Godot.Entities;
using GodotFloorLevels.Scripts.Godot.Resources.Loader;

namespace GodotFloorLevels.Scripts.Godot.Resources;

[GlobalClass]
public partial class GameScenesResource : Resource
{
    // Loader for resources
    [Export] private GodotResourceLoader Loader { get; set; } = new();
    
    // Entities
    [Export] private PackedScene CharacterNodePacked { get; set; }
    
    public GodotCharacter2D CreateCharacter()
    {
        if (CharacterNodePacked == null)
        {
            GD.PrintErr("[GameScenesResource] CharacterPacked is not set.");
            return null;
        }
        
        // Instantiate the character from the PackedScene
        var character = CharacterNodePacked.Instantiate<GodotCharacter2D>();
        
        character.
        
        return character;
    }
}
