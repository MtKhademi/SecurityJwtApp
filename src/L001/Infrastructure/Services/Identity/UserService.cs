using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;
using Infrastructure.Extentions;

namespace Infrastructure.Services.Identity;

public class UserService(
    UserManager<ApplicationUser> _userManager,
    RoleManager<ApplicationRole> _roleManager,
    IMapper mapper,
    ICurrentUserService currentUserService) : IUserService
{
    public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request)
    {
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);

        if (userWithSameEmail is not null)
            return await ResponseWrapper.FailAsync("User with this email already exists");

        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName is not null)
            return await ResponseWrapper.FailAsync("User with this user-name already exists");

        var newUser = new ApplicationUser
        {
            Email = request.Email,
            LastName = request.LastName,
            FirstName = request.FirstName,
            UserName = request.UserName,
            PhoneNumber = request.PhoneNumber,
            IsActive = request.Activate,
            EmailConfirmed = request.AutoConfirmEmail,
            RefreshToken = ""
        };

        var userResult = await _userManager.CreateAsync(newUser, request.Password);

        if (!userResult.Succeeded)
            return await ResponseWrapper.FailAsync(userResult.GetErrors());

        await _userManager.AddToRoleAsync(newUser, AppRoles.Basic);

        return await ResponseWrapper<string>.SuccessAsync("User created successfully");
    }

    public async Task<IResponseWrapper> GetUserByIdAsync(string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb is null)
            return await ResponseWrapper<UserResponse>.FailAsync("User not found");


        return await ResponseWrapper<UserResponse>.SuccessAsync(
            mapper.Map<UserResponse>(userInDb));
    }

    public async Task<IResponseWrapper> GetAllUsersAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return await ResponseWrapper<List<UserResponse>>.SuccessAsync(
            mapper.Map<List<UserResponse>>(users));
    }

    public async Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request, string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb is null)
            return await ResponseWrapper<UserResponse>.FailAsync("User not found");

        userInDb.FirstName = request.FirstName;
        userInDb.LastName = request.LastName;
        userInDb.PhoneNumber = request.PhoneNumber;

        var updateResult = await _userManager.UpdateAsync(userInDb);

        if (updateResult.Succeeded)
            return await ResponseWrapper<string>.SuccessAsync("User modified successfully");

        return await ResponseWrapper<string>.FailAsync(updateResult.GetErrors());
    }

    public async Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request, string userId)
    {
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb is null)
            return await ResponseWrapper<UserResponse>.FailAsync("User not found");

        var changeResult =
            await _userManager.ChangePasswordAsync(userInDb, request.CurrentPassword, request.NewPassword);
        if (changeResult.Succeeded)
            return await ResponseWrapper<string>.SuccessAsync("Password changed successfully");

        return await ResponseWrapper<string>.FailAsync(changeResult.GetErrors());
    }

    public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb is null)
            return await ResponseWrapper<UserResponse>.FailAsync("User not found");

        userInDb.IsActive = request.Active;
        var identityResult = await _userManager.UpdateAsync(userInDb);
        if (identityResult.Succeeded) return await ResponseWrapper<string>.SuccessAsync("User modified successfully");
        return await ResponseWrapper<string>.FailAsync(identityResult.GetErrors());
    }

    public async Task<IResponseWrapper> GetRolesAsync(string userId)
    {
        var userRoles = new List<UserRoleViewModel>();
        var userInDb = await _userManager.FindByIdAsync(userId);
        if (userInDb is null)
            return await ResponseWrapper<string>.FailAsync("User not found");

        var allRoles = await _roleManager.Roles.ToListAsync();
        foreach (var role in allRoles)
        {
            var userRoleVM = new UserRoleViewModel
            {
                RoleName = role.Name,
                RoleDescription = role.Description,
                IsAssignedToUser = false
            };

            if (await _userManager.IsInRoleAsync(userInDb, role.Name))
            {
                userRoleVM.IsAssignedToUser = true;
            }

            userRoles.Add(userRoleVM);
        }

        return await ResponseWrapper<List<UserRoleViewModel>>.SuccessAsync(userRoles);
    }

    public async Task<IResponseWrapper> UpdateUserRolesAsync(UpdateUserRoleRequest request)
    {
        // cannot un-assign administrator
        // default admin user seeded by addplication cannot be assigned/un-assigned
        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb is null)
            return await ResponseWrapper<string>.FailAsync("User not found");

        if (userInDb.Email == AppCredentials.DefaultAdminEmail || userInDb.Email == AppCredentials.DefaultBasicEmail)
            return await ResponseWrapper<string>.FailAsync("You cannot change basic data");

        var roles = await _userManager.GetRolesAsync(userInDb);
        var rolesToBeAssigned = request.Roles.Where(r => r.IsAssignedToUser).ToList();

        var currentLoggedUser = await _userManager.FindByIdAsync(currentUserService.UserId);
        if (currentLoggedUser is null)
            return await ResponseWrapper<string>.FailAsync("User not found");

        if (!await _userManager.IsInRoleAsync(currentLoggedUser, AppRoles.Admin))
            return await ResponseWrapper<string>.FailAsync("Only administrators can change roles");

        var result = await _userManager.RemoveFromRolesAsync(userInDb, roles);
        if (!result.Succeeded)
            return await ResponseWrapper<string>.FailAsync(result.GetErrors());
        
        var resultAddRoles = await _userManager.AddToRolesAsync(userInDb,rolesToBeAssigned.Select(x=>x.RoleName));
        if(!resultAddRoles.Succeeded)
            return await ResponseWrapper<string>.FailAsync(resultAddRoles.GetErrors());
        
        return await ResponseWrapper<string>.SuccessAsync("User roles modified successfully");
    }

}