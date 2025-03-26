using Api.Attributes;
using Application.Features.Identity.Commands;
using Application.Features.Identity.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[Route("api/[controller]")]
public class UserController : MyBaseController<UserController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Users,AppActions.Create)]
    public async Task<IActionResult> RegisterUser([FromBody]UserRegistrationRequest request)
    {
        var response = await MediatorSender.Send(new UserRegistrationCommand { Request = request });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("{userId}")]
    [MustHavePermission(AppFeature.Users,AppActions.Read)]
    public async Task<IActionResult> GetUser([FromRoute] string userId)
    {
        var user = await MediatorSender.Send(new GetUserByIdQuery{UserId = userId});
        if (user.IsSuccessful)
            return Ok(user);
        return BadRequest(user);
    }

    [HttpGet]
    [MustHavePermission(AppFeature.Users,AppActions.Read)]
    public async Task<IActionResult> GetUsers()
    {
        var response = await MediatorSender.Send(new GetAllUsersQuery());
        if (response.IsSuccessful)
            return Ok(response);
        
        return BadRequest(response);
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUserAsync([FromRoute] string userId, [FromBody] UpdateUserRequest request)
    {
        var response = await MediatorSender.Send(new UserUpdateCommand
        {
            UpdateUserRequest = request,
            UserId = userId
        });

        if (response.IsSuccessful)
            return Ok(response);

        return BadRequest(response);
    }

    [HttpPut("{userId}/change-password")]
    public async Task<IActionResult> ChangePassword([FromRoute] string userId, [FromBody] ChangePasswordRequest request)
    {
        var response = await MediatorSender.Send(new UserChangePasswordCommand
        {
            UserId = userId,
            ChangePasswordRequest = request
        });
        if (response.IsSuccessful)
            return Ok(response); 
        return BadRequest(response); 
    }

    [HttpPut("{userId}/change-status")]
    public async Task<IActionResult> ChangeUserStatus([FromRoute] string userId,
        [FromBody] ChangeUserStatusRequest request)
    {
        var response = await MediatorSender.Send(new UserChangeStatusCommand()
        {
            Request = request
        });
        if (response.IsSuccessful)
            return Ok(response); 
        return BadRequest(response); 
    }

    [HttpGet("roles/{userId}")]
    public async Task<IActionResult> GetRoles(string userId)
    {
        var response = await MediatorSender.Send(new GetRolesQuery { UserId = userId });
        if (response.IsSuccessful)
            return Ok(response);
        
        return NotFound(response);
    }

    [HttpPut("roles/change")]
    public async Task<IActionResult> UpdateRoles([FromBody] UpdateUserRoleRequest request)
    {
        var response = await MediatorSender.Send(new UpdateUserRoleCommand { Request = request });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }
}