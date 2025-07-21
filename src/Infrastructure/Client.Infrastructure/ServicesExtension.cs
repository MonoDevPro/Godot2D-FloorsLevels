using Client.Infrastructure.ECS;
using Client.Infrastructure.ECS.Entities;
using Client.Infrastructure.ECS.Systems.Pipeline;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure;

public static class ServicesExtension
{
    /// <summary>
    /// Registers all ArchECS services.
    /// </summary>
    /// <param name="services">The service collection to register the services into.</param>
    public static void RegisterArchServices(this IServiceCollection services)
    {
        // ECS Core
        services.AddSingleton<WorldECS>();
        
        // Pipeline Builder
        services.AddTransient<ISystemsPipelineBuilder, SystemsPipelineBuilder>();
        
        // UpdateECS com configuração declarativa
        services.AddSingleton<UpdateECS>(provider =>
        {
            var builder = provider.GetRequiredService<ISystemsPipelineBuilder>();
            var pipeline = SystemsPipelineConfiguration.ConfigureDefaultPipeline(builder, provider);
            var systemsGroup = pipeline.Build();
            
            return new UpdateECS(
                provider.GetRequiredService<ILogger<UpdateECS>>(),
                systemsGroup
            );
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