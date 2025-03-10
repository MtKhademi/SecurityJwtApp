﻿using System.Reflection;

namespace Infrastructure.Context;

public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser,
    ApplicationRole,
    string, 
    IdentityUserClaim<string>,
    IdentityUserRole<string>,
    IdentityUserLogin<string>,
    ApplicationRoleClaim,
    IdentityUserToken<string>>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        foreach (var property in builder.Model.GetEntityTypes()
            .SelectMany(t=>t.GetProperties())
            .Where(p=>p.ClrType==typeof(decimal) || p.ClrType==typeof(decimal?)))
        {
            property.SetColumnType("decimal(18,2)");
        }

        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);

    }

    public DbSet<Employee> Employees => Set<Employee>();
}
