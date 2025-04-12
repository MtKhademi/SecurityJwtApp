using Application.Features.Identity.Queries;
using Common.Requests;
using Common.Requests.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers.Identity;

[Route("api/[controller]")]
public class TokenController : MyBaseController<TokenController>
{
    [HttpPost("get-token")]
    public async Task<IActionResult> GetToken([FromBody] TokenRequest request)
    {
        var response = await MediatorSender.Send(new GetTokenQuery()
        {
            TokenRequest = request
        });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> GetRefreshToken([FromBody] RefreshTokenRequest request)
    {
        var response = await MediatorSender.Send(new GetRefreshTokenQuery()
        {
            RefreshTokenRequest = request
        });
        if (response.IsSuccessful)
            return Ok(response);
        return BadRequest(response);
    }
}