using Application.Services.Identity;
using AutoMapper;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Identity;
using Common.Responses.Wrappers;

namespace Infrastructure.Services.Identity;

public class UserService(
    UserManager<ApplicationUser> _userManager,
    RoleManager<ApplicationRole> _roleManager,
    IMapper mapper) : IUserService
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
            return await ResponseWrapper.FailAsync(GetIdentityResultErrors(userResult));

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

        return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrors(updateResult));
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

        return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrors(changeResult));
    }

    public async Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request)
    {
        var userInDb = await _userManager.FindByIdAsync(request.UserId);
        if (userInDb is null)
            return await ResponseWrapper<UserResponse>.FailAsync("User not found");

        userInDb.IsActive = request.Active;
        var identityResult = await _userManager.UpdateAsync(userInDb);
        if (identityResult.Succeeded) return await ResponseWrapper<string>.SuccessAsync("User modified successfully");
        return await ResponseWrapper<string>.FailAsync(GetIdentityResultErrors(identityResult));
    }

    private List<string> GetIdentityResultErrors(IdentityResult identityResult)
    {
        return identityResult.Errors.Select(er => er.Description).ToList();
    }
}