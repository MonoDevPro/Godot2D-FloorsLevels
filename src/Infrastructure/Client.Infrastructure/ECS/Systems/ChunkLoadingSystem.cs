using Arch.Core;
using Arch.System;
using Arch.System.SourceGenerator;
using Client.Infrastructure.ECS.Components;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS.Components;

namespace GodotFloorLevels.Scripts.Infrastructure.ArchECS.Systems;

public sealed partial class ChunkLoadingSystem(WorldECS worldECS) : BaseSystem<World, float>(worldECS.World)
{
    [Query]
    [All<ChunkRequestComponent>]
    [None<ChunkLoadedTag>]
    private void Update(Entity entity, in ChunkRequestComponent req)
    {
        // Utilize seu ThreadedResourceLoader aqui
        // Após carregar, adicione:
        // World.AddComponent(entity, new ChunkLoadedTag());
        // var chunkNode = instancia do seu Chunk.tscn;
        // World.AddComponent(entity, new SceneRefComponent { Node = chunkNode });
    }
}