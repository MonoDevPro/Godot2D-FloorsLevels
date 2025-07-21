using Client.Infrastructure;
using Client.Infrastructure.ECS;
using Godot;
using GodotFloorLevels.Scripts.Infrastructure.ArchECS;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Adapters.Bootstrap;

// DIContainer.cs - Autoload
public sealed partial class DIContainer : Node
{
    private ServiceProvider _serviceProvider = null!;
    public static DIContainer Instance { get; private set; } = null!;

    public override void _Ready()
    {
        Instance = this;
        ConfigureServices();
    }

    private void ConfigureServices()
    {
        var services = new ServiceCollection();
        
        // Domain Services
        //services.AddScoped<IPlayerRepository, PlayerRepository>();
        //services.AddScoped<INetworkService, GodotNetworkService>();
        
        // Use Cases
        //services.AddScoped<MovePlayerUseCase>();
        //services.AddScoped<ConnectToServerUseCase>();
        
        // ArchECS
        services.RegisterArchServices();
        
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    public T GetService<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
    public T? GetServiceOrDefault<T>() where T : class => _serviceProvider.GetService<T>();
    
    public override void _ExitTree()
    {
        _serviceProvider?.Dispose();
        _serviceProvider = null!;
        Instance = null!;
    }
}