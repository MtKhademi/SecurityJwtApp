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
        await SeedBasicUserAsync();
        await SeedAdminUserAsync();

    }

    private async Task CheckAndApplyPendingMigrationasync()
    {
        if ((await _context.Database.GetPendingMigrationsAsync()).Any())
        {
            var xx = await _context.Database.GetPendingMigrationsAsync();
            
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
            AppCredentials.DefaultAdminEmail[..AppCredentials.DefaultAdminEmail.IndexOf('@')].ToLowerInvariant();
        var adminUser = new ApplicationUser
        {
            FirstName = "Mt",
            LastName = "Khademi",
            Email = AppCredentials.DefaultAdminEmail,
            UserName = adminUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            PhoneNumber = "+98 9399172443",
            NormalizedEmail = AppCredentials.DefaultAdminEmail.ToUpperInvariant(),
            NormalizedUserName = adminUserName.ToUpperInvariant(),
            IsActive = true,
            RefreshToken = "",
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == adminUser.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            adminUser.PasswordHash = password.HashPassword(adminUser, AppCredentials.DefaultAdminPassword);
            await _userManager.CreateAsync(adminUser);
        }
        
        // Assign role to user
        if (!await _userManager.IsInRoleAsync(adminUser, AppRoles.Basic) &&
            !await _userManager.IsInRoleAsync(adminUser, AppRoles.Admin))
        {
            await _userManager.AddToRolesAsync(adminUser, AppRoles.DefaultRoles);
        }
    }
    private async Task SeedBasicUserAsync()
    {
        string basicUserName =
            AppCredentials.DefaultBasicEmail[..AppCredentials.DefaultBasicEmail.IndexOf('@')].ToLowerInvariant();
        var basicUser = new ApplicationUser
        {
            FirstName = "Mt",
            LastName = "Khademi",
            Email = AppCredentials.DefaultBasicEmail,
            UserName = basicUserName,
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            PhoneNumber = "+98 9399172443",
            NormalizedEmail = AppCredentials.DefaultBasicEmail.ToUpperInvariant(),
            NormalizedUserName = basicUserName.ToUpperInvariant(),
            IsActive = true,
            RefreshToken = "",
        };

        if (!await _userManager.Users.AnyAsync(u => u.Email == basicUser.Email))
        {
            var password = new PasswordHasher<ApplicationUser>();
            basicUser.PasswordHash = password.HashPassword(basicUser, AppCredentials.DefaultBasicPassword);
            await _userManager.CreateAsync(basicUser);
        }
        
        // Assign role to user
        if (!await _userManager.IsInRoleAsync(basicUser, AppRoles.Basic))
        {
            await _userManager.AddToRolesAsync(basicUser, AppRoles.BasicRoles);
        }
    }
    
}