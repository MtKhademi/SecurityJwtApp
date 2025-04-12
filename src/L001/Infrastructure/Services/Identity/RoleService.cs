using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Context;
using Infrastructure.Extentions;

namespace Infrastructure.Services.Identity;

public class RoleService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper,
    ApplicationDbContext context) : IRoleService
{
    public async Task<IResponseWrapper> CreateRoleAsync(CreateRoleRequest request)
    {
        var roleExist = await roleManager.RoleExistsAsync(request.Name);
        if (roleExist)
            return await ResponseWrapper<string>.FailAsync("Role already exists");

        var role = new ApplicationRole
        {
            Name = request.Name,
            Description = request.Description
        };
        var resultCreate = await roleManager.CreateAsync(role);
        if (!resultCreate.Succeeded)
            return await ResponseWrapper<string>.FailAsync(resultCreate.GetErrors());

        return await ResponseWrapper<ApplicationRole>.SuccessAsync(role, "Role created");
    }

    public async Task<IResponseWrapper> GetRolesAsync()
    {
        var allRoles = await roleManager.Roles.ToListAsync();
        return await ResponseWrapper<List<RoleResponse>>.SuccessAsync(
            data: mapper.Map<List<RoleResponse>>(allRoles), "Roles");
    }

    public async Task<IResponseWrapper> GetPermissionsAsync(string roleId)
    {
        var roleInDb = await roleManager.FindByIdAsync(roleId);
        if (roleId is null)
            return await ResponseWrapper<string>.FailAsync("Role does not exist");

        var allPermissions = AppPermissions.AdminPermissions;
        var roleClaimResponse = new RoleClaimResponse
        {
            Role = new RoleResponse
            {
                Id = roleInDb.Id,
                Description = roleInDb.Description,
                Name = roleInDb.Name
            },
            RoleClaims = new()
        };

        var currentRoleClaims = await GetAllClaimsForRoleAsync(roleId);
        var allPermissionsName = allPermissions.Select(p => p.Name).ToList();
        var currentRoleClaimsValues = currentRoleClaims.Select(c => c.ClaimType).ToList();

        var currentlyAssignedRoloClaimsNames = allPermissionsName
            .Intersect(currentRoleClaimsValues)
            .ToList();
        foreach (var permission in allPermissions)
        {
            if (currentlyAssignedRoloClaimsNames.Any(c => c == permission.Name))
            {
                roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel()
                {
                    RoleId = roleId,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group,
                    IsAssigned = true
                });
            }
            else
            {
                roleClaimResponse.RoleClaims.Add(new RoleClaimViewModel()
                {
                    RoleId = roleId,
                    ClaimType = AppClaim.Permission,
                    ClaimValue = permission.Name,
                    Description = permission.Description,
                    Group = permission.Group,
                    IsAssigned = false
                });
            }
        }
        
        return await ResponseWrapper<RoleClaimResponse>.SuccessAsync(roleClaimResponse, "Role Claims");
    }

    private async Task<List<RoleClaimViewModel>> GetAllClaimsForRoleAsync(string roleId)
    {
        var roleClaim = await context.RoleClaims
            .Where(rol => rol.RoleId == roleId)
            .ToListAsync();

        if (roleClaim.Count > 0)
        {
            var mappedRoleClaim = mapper.Map<List<RoleClaimViewModel>>(roleClaim);
            return mappedRoleClaim;
        }

        return [];
    }
}