using Client.Application.Abstraction.Boundary;
using Client.Infrastructure.Bootstrap.DependencyInjection;
using Client.Infrastructure.Godot.Adapters;
using Godot;
using Microsoft.Extensions.DependencyInjection;

namespace Client.Infrastructure.Godot.Bootstrap;

// DIContainer.cs - Autoload
public partial class DIContainer : Node
{
    private IServiceProvider _serviceProvider = null!;
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
        
        // Infrastructure Services
        services.AddInfrastructureServices();
        
        // Godot
        services.AddSingleton<ILocalInputReader, InputAdapter>();
        
        _serviceProvider = services.BuildServiceProvider();
    }
    
    public T GetService<T>() where T : notnull => _serviceProvider.GetRequiredService<T>();
    public T? GetServiceOrDefault<T>() where T : class => _serviceProvider.GetService<T>();
    
    /// <summary>
    /// Called when the node is removed from the scene tree.
    /// This method disposes of the ECS world and update runner.
    /// </summary>
    public override void _ExitTree()
    {
        Instance = null!;
        
        base._ExitTree();
    }
}
