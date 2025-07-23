using Arch.System;
using Client.Infrastructure.ECS;
using Client.Infrastructure.ECS.Entities;
using Client.Infrastructure.ECS.Systems;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Infrastructure.Bootstrap.DependencyInjection;

public static class ServicesEcsExtensions
{
    /// <summary>
    /// Registers all ArchECS services.
    /// </summary>
    /// <param name="services">The service collection to register the services into.</param>
    public static void RegisterArchServices(this IServiceCollection services)
    {
        // ECS Core
        services.AddSingleton<WorldECS>();
        
        // UpdateECS com configuração declarativa
        services.AddSingleton<IGameLoopAdapter, EcsLoop>();
        services.AddSingleton<Group<float>>(provider =>
        {
            var world = provider.GetRequiredService<WorldECS>();
            ISystem<float>[] systems =
            [
                new LocalMovementSystem(world),
                new RemoteSyncSystem(world),
            ];
            return new Group<float>("ECS Systems Pipeline", systems);
        });
        
        // Entities
        RegisterEntities(services);
    }
    
    private static void RegisterEntities(IServiceCollection services)
    {
        services.AddTransient<NpcECS>(provider => 
            NpcECS.Create(provider.GetRequiredService<WorldECS>()));
        services.AddTransient<PlayerECS>(provider => 
            PlayerECS.Create(provider.GetRequiredService<WorldECS>()));
    }
}
