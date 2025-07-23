using Arch.System;
using Client.Application.Abstraction.Boundary;
using Microsoft.Extensions.Logging;

namespace Client.Infrastructure.ECS;

public sealed class EcsLoop : IGameLoopAdapter
{
    private readonly ILogger<EcsLoop> _logger;
    private readonly Group<float> _deltaGroup;
    
    public EcsLoop(ILogger<EcsLoop> logger, Group<float> deltaGroup)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _deltaGroup = deltaGroup ?? throw new ArgumentNullException(nameof(deltaGroup));
        
        _deltaGroup.Initialize();
        
        _logger.LogInformation("[EcsRunner] UpdateECS initialized with group: {GroupName}", _deltaGroup.Name);
    }

    public void Tick(double d) // Called by the game loop adapter to update the ECS world
    {
        var delta = (float)d; // Convert double to float for ECS systems
        _deltaGroup.BeforeUpdate(delta);    // Calls .BeforeUpdate on all systems   ( can be overriden )
        _deltaGroup.Update(delta);          // Calls .Update on all systems         ( can be overriden )
        _deltaGroup.AfterUpdate(delta);     // Calls .AfterUpdate on all systems    (can be overridden)
    }
    
    public void Dispose()
    {
        _deltaGroup.Dispose();
        
        _logger.LogInformation("[EcsRunner] UpdateECS disposed and resources cleaned up.");
    }
}
