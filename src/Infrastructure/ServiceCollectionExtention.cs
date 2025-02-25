using Application.Services.Identity;
using Infrastructure.Context;
using Infrastructure.Services.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class ServiceCollectionExtention
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
                 options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")))
                // options.UseInMemoryDatabase(Guid.NewGuid().ToString()))
            .AddTransient<ApplicationDbSeeder>();

        services.AddScoped<ITokenService, TokenService>();
        
        return services;
    }
}
