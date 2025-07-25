using Client.Domain.Enums;
using Godot;
using GodotFloorLevels.Scripts.Godot.Resources.Loader;

namespace GodotFloorLevels.Scripts.Godot.Resources.Utils;

[GlobalClass]
public partial class CharacterResource : Resource
{
    // Character properties
    [Export] public int Id { get; set; } = 0;
    [Export] public string Name { get; set; } = "Default Character";
    [Export] public string Description { get; set; } = "This is a default character description.";
    [Export] public bool IsLocalPlayer { get; set; } = false;
    [Export] public VocationEnum Vocation = VocationEnum.None;
    [Export] public GenderEnum Gender = GenderEnum.None;
    [Export] public float MovementSpeed { get; set; } = 3f;
    [Export] public float AttackSpeed { get; set; } = 1f;
    
    // Sprite resource for the character
    [Export] public GodotSpriteResource SpriteResource { get; set; }
    
    public override string ToString()
    {
        return $"{Name} ({Id}) - {Description} " +
               $"[Local: {IsLocalPlayer}, Speed: {MovementSpeed}, Attack: {AttackSpeed}]";
    }
}
