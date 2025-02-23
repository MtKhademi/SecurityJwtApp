using Application.Services.Identity;
using Common.Requests;
using Common.Responses;
using Common.Responses.Wrappers;

namespace Infrastructure.Services.Identity;

public class TokenService : ITokenService 
{
    public Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}