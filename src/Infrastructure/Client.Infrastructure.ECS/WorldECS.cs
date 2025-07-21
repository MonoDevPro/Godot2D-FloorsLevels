using System;
using Arch.Core;
using Microsoft.Extensions.Logging;

namespace GodotFloorLevels.Scripts.Infrastructure.ArchECS;

public sealed class WorldECS : IDisposable
{
    private readonly ILogger<WorldECS> _logger;
    public World World { get; private set; }

    public WorldECS(ILogger<WorldECS> logger)
    {
        _logger = logger;
        
        World = World.Create();
        _logger.LogInformation("[Arch ECS] Mundo ECS criado");
    }
    
    public void Dispose()
    {
        // Dispose of the ECS world when the node is removed from the scene tree
        World.Dispose();                           // Dispose of the ECS world
        World = null!;                              // Clear the world reference
        
        _logger.LogInformation("ECS Runner exited and resources cleaned up.");
    }
}