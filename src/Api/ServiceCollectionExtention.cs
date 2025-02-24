using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using Application.AppConfigs;
using Common.Authorization;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

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
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        return services;
    }

    
    internal static IApplicationBuilder SeedDatabase(this IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var seeder = serviceScope.ServiceProvider.GetService<ApplicationDbSeeder>();
        seeder?.SeedAsync().GetAwaiter().GetResult();
        return app;
    }

    internal static IServiceCollection AddJwtAuthentication(this IServiceCollection services,
        AppConfiguration config)
    {
        var key = Encoding.ASCII.GetBytes(config.Secret);

        services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(bearer =>
            {
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    RoleClaimType = ClaimTypes.Role,
                    ClockSkew = TimeSpan.Zero
                };

                bearer.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = c =>
                    {
                        if (c.Exception is SecurityTokenValidationException)
                        {
                            c.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                            c.Response.ContentType = "application/json";
                            var result = Newtonsoft.Json.JsonConvert.SerializeObject(
                                ResponseWrapper.Fail("An unhandled error has occured"));
                            return c.Response.WriteAsync(result);
                        }

                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        context.HandleResponse();
                        if (!context.Response.HasStarted)
                        {
                            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                            context.Response.ContentType = "application/json";
                            var result =
                                Newtonsoft.Json.JsonConvert.SerializeObject(
                                    ResponseWrapper.Fail("You are not Authorized"));
                            return context.Response.WriteAsync(result);
                        }
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                        context.Response.ContentType = "application/json";
                        var result =
                            Newtonsoft.Json.JsonConvert.SerializeObject(
                                ResponseWrapper.Fail("You are not Authorized to access this resource"));
                        return context.Response.WriteAsync(result);
                    },
                };
            });


        services.AddAuthorization(options =>
        {
            foreach (var prop in typeof(AppPermissions).GetNestedTypes().SelectMany(c=>c.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)))
            {
                var propertyValue = prop.GetValue(null);
                if (propertyValue is not null)
                {
                    options.AddPolicy(propertyValue.ToString(),policy=>policy.RequireClaim(AppClaim.Permission,propertyValue.ToString()));
                }
            }
        });
        
        return services;
    }

    internal static AppConfiguration GetApplicationSettings(this IServiceCollection services, IConfiguration config)
    {
        var applicationSettings = config.GetSection(nameof(AppConfiguration));
        services.Configure<AppConfiguration>(applicationSettings);
        return applicationSettings.Get<AppConfiguration>(); 
    }
}