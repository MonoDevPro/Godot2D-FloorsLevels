using Client.Infrastructure.Godot.Entities.Common;
using Godot;

namespace Client.Infrastructure.Godot.Entities;

public partial class PlayerNode : GodotBody2D
{
    public override void _Ready()
    {
        // Configure Camera
        var cam = GetNode<Camera2D>("Camera2D");
        cam.MakeCurrent();
    }
}