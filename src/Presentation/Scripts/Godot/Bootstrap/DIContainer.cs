using Game.Shared.ECS;
using Godot;
using GodotFloorLevels.Scripts.ECS;
using Microsoft.Extensions.DependencyInjection;

namespace GodotFloorLevels.Scripts.Godot.Bootstrap;

#nullable enable
// DIContainer.cs - Autoload
public partial class DIContainer : Node
{
    private static IServiceProvider Provider = null!;
    
    public static T GetRequiredService<T>() where T : notnull => Provider.GetRequiredService<T>();
    public static T? GetServiceOrNull<T>() where T : class => Provider.GetService<T>();

    public override void _Ready()
    {
        // Initialize the ECS world and services
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // ECS Core
        services.AddSingleton<EcsRunner>();
        
        Provider = services.BuildServiceProvider();
    }
    
    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// This method disposes of the ECS world and update runner.
    /// </summary>
    public override void _ExitTree()
    {
        base._ExitTree();
    }
}
