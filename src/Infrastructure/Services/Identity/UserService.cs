using Application.Services.Identity;
using Common.Authorization;
using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Infrastructure.Services.Identity;

public class UserService(
    UserManager<ApplicationUser> _userManager,
    RoleManager<ApplicationRole> _roleManager) : IUserService
{
    public async Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request)
    {
        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        
        if(userWithSameEmail is not null )
            return await ResponseWrapper.FailAsync("User with this email already exists");

        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if(userWithSameUserName is not null)
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
     
        if(!userResult.Succeeded)
            return await ResponseWrapper.FailAsync($"User creation failed :: {string.Join('-',userResult.Errors.Select(x=>x.Description))}");

        await _userManager.AddToRoleAsync(newUser, AppRoles.Basic); 

        return await ResponseWrapper<string>.SuccessAsync("User created successfully");
    }
}