using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace WebApi_Identity.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public IActionResult Authticate([FromBody] Credential credential)
    {
        // creating security context
        if (credential.UserName == "admin" && credential.Password == "123")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credential.UserName),
                new Claim(ClaimTypes.Email, "admin@email.com"),
                new Claim("Department", "HR"),
                new Claim("Admin", "true"),
                new Claim("Manager", "true"),
                new Claim("EmploymentDate", "2025-01-01")
            };
            var expiresAt = DateTime.Now.AddMinutes(10);
            return Ok(new
            {
                access_token = CreateToken(claims,expiresAt),
                expires_at = expiresAt
            });
        }
        
        return Unauthorized("You are not authorized to access the endoint");
    }

    private string CreateToken(IEnumerable<Claim> claims, DateTime expiresAt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]);
        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = DateTime.UtcNow,
            Expires = expiresAt,
            SigningCredentials =
                new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
}

public class Credential
{
    public string UserName { get; set; } = default!;
    public string Password { get; set; } = default!;
}