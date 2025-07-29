using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Game.Shared.Scripts.ECS.Components;

namespace GodotFloorLevels.Scripts.Root.ECS.Systems;

public partial class RenderSystem(World world) : BaseSystem<World, float>(world)
{
    [Query]
    [All<NetworkedTag, PositionComponent, SceneBodyRefComponent>]
    private void UpdateVisualSync([Data] in float delta, in PositionComponent pos, in SceneBodyRefComponent sceneBody)
    {
        sceneBody.Value.GlobalPosition = sceneBody.Value.GlobalPosition.Lerp(pos.Value, delta * 10f);
    }
}
