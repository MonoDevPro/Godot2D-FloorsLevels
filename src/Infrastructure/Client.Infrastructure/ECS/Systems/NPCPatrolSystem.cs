using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Client.Adapters.ECS;
using Client.Infrastructure.ECS.Components;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;

namespace Client.Infrastructure.ECS.Systems;

public sealed partial class NPCPatrolSystem(WorldECS worldECS) : BaseSystem<World, float>(worldECS.World)
{
    [Query]
    [All<NpcTag, PositionComponent, SceneRefComponent>]
    private void Update([Data]float delta, ref PositionComponent pos, ref NpcTag patrol, in SpeedComponent speed)
    {
        /*var target = patrol.PatrolPoints[patrol.CurrentIndex];
        var dir = (target - pos.Position);
        if (dir.Length() < 5f)
        {
            patrol.CurrentIndex = (patrol.CurrentIndex + 1) % patrol.PatrolPoints.Length;
            return;
        }
        pos.Position += dir.Normalized() * speed.Speed * delta;*/
    }
}