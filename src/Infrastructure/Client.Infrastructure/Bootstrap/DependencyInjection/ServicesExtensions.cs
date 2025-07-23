using Microsoft.Extensions.DependencyInjection;

namespace Client.Infrastructure.Bootstrap.DependencyInjection;

public static class ServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register your infrastructure services here
        services.RegisterArchServices();
        
        return services;
    }
}
