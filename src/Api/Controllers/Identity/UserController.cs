using Api.Attributes;
using Application.Features.Identity.Commands;
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
}