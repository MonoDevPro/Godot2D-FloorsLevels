using Client.Domain.ValueObjects;
using Client.Infrastructure.ECS.Entities;
using Client.Infrastructure.Godot.Bootstrap;
using Client.Infrastructure.Godot.Entities.Common;
using Godot;

namespace Client.Infrastructure.Godot.Entities;

public partial class PlayerNode : GodotBody2D
{
    private PlayerECS playerECS = null!;
    
    public override void _Ready()
    {
        // Configure Camera
        var cam = GetNode<Camera2D>("Camera2D");
        cam.MakeCurrent();
        
        // Initialize PlayerECS
        playerECS = DIContainer.Instance.GetService<PlayerECS>();
        
        playerECS.
    }
    public override void _Process(double delta)
    {
        // Update PlayerECS
        playerECS.UpdateComponent(ReadInput);
    }
    
    private PlayerInput ReadInput()
    {
        return new PlayerInput
        {
            AxisX = Input.GetActionStrength("move_right") - Input.GetActionStrength("move_left"),
            AxisY = Input.GetActionStrength("move_down") - Input.GetActionStrength("move_up"),
            AttackPressed = Input.IsActionJustPressed("attack"),
            JumpPressed = Input.IsActionJustPressed("jump")
        };
    }
}
