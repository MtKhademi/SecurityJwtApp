using Common.Requests.Identity;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface IUserService
{
    Task<IResponseWrapper> RegisterUserAsync(UserRegistrationRequest request);
    Task<IResponseWrapper> GetUserByIdAsync(string userId);
    
    Task<IResponseWrapper> GetAllUsersAsync();
    
    Task<IResponseWrapper> UpdateUserAsync(UpdateUserRequest request,string userId);
    
    Task<IResponseWrapper> ChangeUserPasswordAsync(ChangePasswordRequest request,string userId);
    
    Task<IResponseWrapper> ChangeUserStatusAsync(ChangeUserStatusRequest request);
}