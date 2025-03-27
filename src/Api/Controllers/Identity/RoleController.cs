using Api.Attributes;
using Application.Features.Identity.Role.Commands;
using Application.Features.Identity.Role.Queries;
using Common.Authorization;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[Route("api/[controller]")]
public class RoleController : MyBaseController<RoleController>
{
    [HttpPost]
    [MustHavePermission(AppFeature.Roles, AppActions.Create)]
    public async Task<IActionResult> CreateRoleAsync([FromBody] CreateRoleRequest request)
    {
        var response = await MediatorSender.Send(new CreateRoleCommand() { Request = request });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpGet]
    [MustHavePermission(AppFeature.Roles, AppActions.Read)]
    public async Task<IActionResult> GetRolesAsync()
    {
        var response = await MediatorSender.Send(new GetRolesQuery());
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpGet("/permission/{roleId}")]
    [MustHavePermission(AppFeature.Roles, AppActions.Read)]
    public async Task<IActionResult> GetPersmissions(string roleId)
    {
        var response = await MediatorSender.Send(new GetPermissionsQuery() { RoleId = roleId });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    } 
}