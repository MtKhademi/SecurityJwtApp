using Application.Services.Identity;
using AutoMapper;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Extentions;

namespace Infrastructure.Services.Identity;

public class RoleService(
    UserManager<ApplicationUser> userManager,
    RoleManager<ApplicationRole> roleManager,
    IMapper mapper) : IRoleService
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
}