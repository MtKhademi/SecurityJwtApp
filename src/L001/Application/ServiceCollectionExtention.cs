using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ServiceCollectionExtention
{
    
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        var assembly = Assembly.GetExecutingAssembly();
        
        services.AddMediatR(cfg=>cfg.RegisterServicesFromAssemblies(assembly));

        services.AddAutoMapper(assembly);
        
        return services;
    }
}