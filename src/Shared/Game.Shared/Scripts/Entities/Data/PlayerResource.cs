using Game.Shared.Scripts.Core.Enums;
using Godot;

namespace Game.Shared.Scripts.Entities.Data;

[GlobalClass]
public partial class PlayerResource : Resource
{
    // Character properties
    [Export] public int Id { get; set; } = 0; // Default Network ID
    [Export] public string Name { get; set; } = "Default Character";
    [Export] public string Description { get; set; } = "This is a default character description.";
    [Export] public bool IsClient { get; set; } = true; // Indicates if this is a client character
    [Export] public bool IsLocalPlayer { get; set; } = true; // Indicates if this is the local player character
    [Export] public VocationEnum Vocation = VocationEnum.None;
    [Export] public GenderEnum Gender = GenderEnum.None;
    [Export] public float Speed { get; set; } = 200f; // Default movement speed
    [Export] public Vector2 Position { get; set; } = Vector2.Zero; // Default position
    [Export] public Vector2 Velocity { get; set; } = Vector2.Zero; // Default velocity

    public void UpdateFromResource(PlayerResource resource)
    {
        Id = resource.Id;
        Name = resource.Name;
        Description = resource.Description;
        IsLocalPlayer = resource.IsLocalPlayer;
        Vocation = resource.Vocation;
        Gender = resource.Gender;
        Speed = resource.Speed;
    }

    public override string ToString()
    {
        return $"PlayerResource(" +
               $"Id: {Id}, " +
               $"Name: {Name}, " +
               $"Description: {Description}, " +
               $"IsLocalPlayer: {IsLocalPlayer}, " +
               $"Vocation: {Vocation}, " +
               $"Gender: {Gender}, " +
               $"Speed: {Speed})";
    }
}
