using Common.Authorization;

namespace Infrastructure.Context;

public class ApplicationDbSeeder(
    ApplicationDbContext context,
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager)
{
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<ApplicationRole> _roleManager = roleManager;
    private readonly ApplicationDbContext _context = context;

    public async Task SeedAsync()
    {
        // Check for pending and apply if any
        await CheckAndApplyPendingMigrationasync();
        // seed roles
        await SeedRolesAsync();
        // seed admin
        await SeedAdminUserAsync();

    }

    private async Task CheckAndApplyPendingMigrationasync()
    {
        if ((await _context.Database.GetPendingMigrationsAsync()).Any())
        {
            await _context.Database.MigrateAsync();
        }
    }

    private async Task SeedRolesAsync()
    {
        foreach (var roleName in AppRoles.DefaultRoles)
        {
            if (await _roleManager.Roles.FirstOrDefaultAsync(rol => rol.Name == roleName) is not ApplicationRole role)
            {
                role = new ApplicationRole
                {
                    Name = roleName,
                    Description = $"{roleName} role."
                };
                await _roleManager.CreateAsync(role);
            }
            //Add permission
            if (roleName == AppRoles.Admin)
            {
                // admin 
                await AssignPermissionsToRoleAsync(role, AppPermissions.AdminPermissions);
            } 
            else if(roleName == AppRoles.Basic)
            {
                // basic
                await AssignPermissionsToRoleAsync(role, AppPermissions.BasicPermissions);
            }
        }
    }

    private async Task AssignPermissionsToRoleAsync(ApplicationRole role, IReadOnlyList<AppPermission> permissions)
    {
        var currentClaim = await _roleManager.GetClaimsAsync(role);
        foreach (var permission in permissions)
        {
            if (!currentClaim.Any(claim => claim.Type == AppClaim.Permission && claim.Value == permission.Name))
            {
                await _context.RoleClaims.AddAsync(new ApplicationRoleClaim
                {
                    RoleId = role.Id,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group
                });
                await _context.SaveChangesAsync();
            }
        }
    }

    private async Task SeedAdminUserAsync()
    {
        string adminUserName =
            AppCredentials.DefaultEmail[..AppCredentials.DefaultEmail.IndexOf('@')].ToLowerInvariant();
        var adminUser = new ApplicationUser
        {
            FirstName = "Mt",
            LastName = "Khademi",
            Email = AppCredentials.DefaultEmail,
            UserName = adminUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            PhoneNumber = "+98 9399172443",
            NormalizedEmail = AppCredentials.DefaultEmail.ToUpperInvariant(),
            NormalizedUserName = adminUserName.ToUpperInvariant(),
            IsActive = true
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == adminUser.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.DefaultPassword);
            await _userManager.CreateAsync(adminUser);
        }
        
        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic) &&
            !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
        }
    }
    
}