using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi_Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PersonController : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> Gets()
        {
            var persons = new List<PersonDto>
            {
                new PersonDto("ahmad", "ahmad", "123")
            };
            return Ok(persons);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetWithPolicy([FromRoute(Name ="id")]string personId)
        {
            return Ok(new PersonDto("ahmad", "ahmad", "123"));
        }
    }


    public record PersonDto(
        string Name = default!,
        string Email = default!,
        string PhoneNumber = default!);

}


