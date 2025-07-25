using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.ECS.Components;

namespace Game.Shared.ECS.Systems;

public partial class MovementSystem(World world) : BaseSystem<World, float>(world)
{
    [Query] // Client-side movement system: applies InputCommand to Position
    [All<LocalTag, PositionComponent, InputCommandComponent, SpeedComponent>]
    private void UpdateLocalPrediction([Data] in float delta, ref PositionComponent pos, in InputCommandComponent cmd, in SpeedComponent speed)
    {
        pos.Position += cmd.Input.Normalized() * speed.Speed * delta;
    }
    
    [Query] // Client interpolation for remote entities
    [All<RemoteTag, PositionComponent, SceneRefComponent>]
    private void UpdateRemoteInterpolation(in PositionComponent pos, in SceneRefComponent scene)
    {
        scene.Node.GlobalPosition = pos.Position; // use lerp for smoothness
    }
}

public partial class NpcAISystem(World world) : BaseSystem<World, float>(world)
{
    // Patrulha NPC (rodando no server)
    [Query]
    [All<NPCPatrolTag, PositionComponent, SpeedComponent>]
    private void UpdatePatrol([Data] in float delta, ref PositionComponent pos, ref NPCPatrolTag patrol, in SpeedComponent speed)
    {
        var target = patrol.PatrolPoints[patrol.CurrentIndex];
        var dir = target - pos.Position;
        if (dir.Length() < 5f) patrol.CurrentIndex = (patrol.CurrentIndex + 1) % patrol.PatrolPoints.Length;
        else pos.Position += dir.Normalized() * speed.Speed * delta;
    }
}
