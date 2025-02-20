using Infrastructure.Context;
using Infrastructure.Models;

namespace Api;

public static class ServiceCollectionExtention
{

    internal static IServiceCollection AddIdentitySettings(this IServiceCollection services)
    {
        services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
        {
            options.Password.RequiredLength = 6;
            options.Password.RequireDigit = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.User.RequireUniqueEmail = true;
        }).AddEntityFrameworkStores<ApplicationDbContext>();
        return services;
    }

    
    internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var seeder = serviceScope.ServiceProvider.GetService<ApplicationDbSeeder>();
        seeder?.SeedAsync().GetAwaiter().GetResult();
        return app;
    }
}