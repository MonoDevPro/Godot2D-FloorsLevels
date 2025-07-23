using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Client.Infrastructure.ECS.Components;

namespace Client.Infrastructure.ECS.Systems;


public sealed partial class LocalMovementSystem(WorldECS worldECS) : BaseSystem<World, float>(worldECS.World)
{
    [Query]
    [All<CharacterTag, PositionComponent, SpeedComponent>]
    [None<RemotePlayerTag>]
    private void Update([Data]float delta, ref PositionComponent pos, in SpeedComponent speed)
    {
    }
}
