using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Client.Adapters.ECS;
using Client.Infrastructure.ECS.Components;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;

namespace Client.Infrastructure.ECS.Systems;

public sealed partial class RemoteSyncSystem(WorldECS worldECS) : BaseSystem<World, float>(worldECS.World)
{
    [Query]
    [All<RemotePlayerTag, PositionComponent, SceneRefComponent>]
    private void Update(in PositionComponent pos, in SceneRefComponent scene)
    {
        scene.Node.GlobalPosition = pos.Position;
    }
}