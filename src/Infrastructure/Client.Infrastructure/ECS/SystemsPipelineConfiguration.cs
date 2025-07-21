using Client.Infrastructure.ECS.Systems.Pipeline;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS.Systems;
using Microsoft.Extensions.DependencyInjection;
using LocalMovementSystem = Client.Infrastructure.ECS.Systems.LocalMovementSystem;
using NPCPatrolSystem = Client.Infrastructure.ECS.Systems.NPCPatrolSystem;
using RemoteSyncSystem = Client.Infrastructure.ECS.Systems.RemoteSyncSystem;

namespace Client.Infrastructure.ECS;

// 2. Configuration class para pipeline
public static class SystemsPipelineConfiguration
{
    public static ISystemsPipelineBuilder ConfigureDefaultPipeline(ISystemsPipelineBuilder builder, IServiceProvider provider)
    {
        var world = provider.GetRequiredService<WorldECS>();
        
        return builder
            .AddSystem<LocalMovementSystem>(new LocalMovementSystem(world), "Processa input do jogador")
            .AddSystem<RemoteSyncSystem>(new RemoteSyncSystem(world), "Sincroniza estado remoto")
            .AddSystem<NPCPatrolSystem>(new NPCPatrolSystem(world), "Gerencia patrulha de NPCs")
            .AddSystem<ChunkLoadingSystem>(new ChunkLoadingSystem(world), "Carrega e descarrega chunks");
    }
}