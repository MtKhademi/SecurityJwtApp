using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Application.AppConfigs;
using Application.Services.Identity;
using Common.Requests;
using Common.Responses;
using Common.Responses.Wrappers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService 
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly AppConfiguration _appConfiguration;

    public TokenService(UserManager<ApplicationUser> userManager, 
        RoleManager<ApplicationRole> roleManager,
        IOptions<AppConfiguration> appConfiguration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _appConfiguration = appConfiguration.Value;
    }

    public async Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByEmailAsync(tokenRequest.Email);
        if(user is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid email address.");
        if(!user.IsActive)
            return await ResponseWrapper<TokenResponse>.FailAsync("User not active. Please contact the admin");
        
        if(!user.EmailConfirmed)
            return await ResponseWrapper<TokenResponse>.FailAsync("Email not confirmed. Please contact the admin");
        
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, tokenRequest.Password);
        if(!isPasswordValid)
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid password.");

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);

        await _userManager.UpdateAsync(user);

        return new ResponseWrapper<TokenResponse>()
        {
            IsSuccessful = true,
            ResponseData = new TokenResponse()
            {
                Token =await GenerateJWTTokenAsync(user),
                RefreshToken = user.RefreshToken,
                RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
            }
        };
    }

    
    public async Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        if(refreshTokenRequest is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid refresh token.");
        
        var userPrincipal = GetPrincipalFromExpiredToken(refreshTokenRequest.Token);
        var userEmail = userPrincipal.FindFirstValue(ClaimTypes.Email);
        var user = await _userManager.FindByEmailAsync(userEmail);
        
        if(user is null)
            return await ResponseWrapper<TokenResponse>.FailAsync("User not found.");
        if(user.RefreshToken != refreshTokenRequest.Token ||
           user.RefreshTokenExpiryDate <= DateTime.Now)
            return await ResponseWrapper<TokenResponse>.FailAsync("Invalid refresh token.");

        var token = GenerateEncryptedToken(GetSigningCredentials(), await GetClaimsAsync(user));

        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryDate = DateTime.Now.AddDays(7);
        await _userManager.UpdateAsync(user);

        var response = new TokenResponse()
        {
            Token = token,
            RefreshToken = user.RefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryDate
        };
        
        return await ResponseWrapper<TokenResponse>.SuccessAsync(response);
    }


    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rnd = RandomNumberGenerator.Create())
        {
            rnd.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
    {
       return GenerateEncryptedToken(GetSigningCredentials(),await GetClaimsAsync(user));
    }

    private string GenerateEncryptedToken(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
    {
        var token = new JwtSecurityToken(claims: claims, 
            expires: DateTime.UtcNow.AddMinutes(_appConfiguration.TokenExpiryInMinutes),
            signingCredentials: signingCredentials);
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var encryptedToken = tokenHandler.WriteToken(token);
        return encryptedToken;
    }

    private SigningCredentials GetSigningCredentials()
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret));
        return new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
    }

    private async Task<IEnumerable<Claim>> GetClaimsAsync(ApplicationUser user)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roles = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var permissionClaims = new List<Claim>();

        foreach (var role in roles)
        {
            roleClaims.Add(new Claim(ClaimTypes.Role, role));
            var currentRole = await _roleManager.FindByNameAsync(role);
            var allPermissionsForCurrentRole = await _roleManager.GetClaimsAsync(currentRole);
            permissionClaims.AddRange(allPermissionsForCurrentRole);    
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Name, user.FirstName),
            new(ClaimTypes.Surname, user.LastName),
            new(ClaimTypes.MobilePhone, user.PhoneNumber ?? string.Empty),
        };
        
        claims.AddRange(roleClaims);
        claims.AddRange(permissionClaims);

        return claims;
    }

    private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appConfiguration.Secret)),
            ValidateIssuer = false,
            ValidateAudience = false,
            RoleClaimType = ClaimTypes.Role,
            ClockSkew = TimeSpan.Zero
        };
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
        
        if(securityToken is not JwtSecurityToken jwtSecurityToken || 
           !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;

    }
    
}