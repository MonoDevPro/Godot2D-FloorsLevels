using Game.Shared.Resources.Loader;
using Godot;

namespace GodotFloorLevels.Scripts.Resources;

[GlobalClass]
public partial class GameScenesResource : Resource
{
    // Entities
    [Export] private PackedScene CharacterNodePacked { get; set; }
}
