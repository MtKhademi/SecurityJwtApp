using Application.Services.Identity;
using Common.Requests;
using Common.Responses;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService 
{
    public Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}