using System;
using Arch.System;
using Microsoft.Extensions.Logging;

namespace GodotFloorLevels.Scripts.Infrastructure.ArchECS;

public sealed class UpdateECS : IDisposable
{
    private readonly ILogger<UpdateECS> _logger;
    private readonly Group<float> _deltaGroup;
    
    public UpdateECS(ILogger<UpdateECS> logger, Group<float> deltaGroup)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _deltaGroup = deltaGroup ?? throw new ArgumentNullException(nameof(deltaGroup));
        
        _deltaGroup.Initialize();
        
        _logger.LogInformation("[EcsRunner] UpdateECS initialized with group: {GroupName}", _deltaGroup.Name);
    }
    
    public void Update(float delta)
    {
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