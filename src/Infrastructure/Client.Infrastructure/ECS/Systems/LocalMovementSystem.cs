using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Client.Infrastructure.ECS.Components;
using Godot;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;

namespace Client.Infrastructure.ECS.Systems;


public sealed partial class LocalMovementSystem(WorldECS worldECS) : BaseSystem<World, float>(worldECS.World)
{
    [Query]
    [All<PlayerTag, PositionComponent, SpeedComponent>]
    [None<RemotePlayerTag>]
    private void Update([Data]float delta, ref PositionComponent pos, in SpeedComponent speed)
    {
        Vector2I input = Vector2I.Zero;
        if (Input.IsActionPressed("ui_right")) input.X += 1;
        if (Input.IsActionPressed("ui_left"))  input.X -= 1;
        if (Input.IsActionPressed("ui_down"))  input.Y += 1;
        if (Input.IsActionPressed("ui_up"))    input.Y -= 1;

        var vel = input.Normalized() * speed.Speed;
        pos.Position += vel * delta;
    }
}