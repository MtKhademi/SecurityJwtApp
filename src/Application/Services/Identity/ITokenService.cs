using Common.Requests;
using Common.Responses;

namespace Application.Services.Identity;

public interface ITokenService
{
    Task<TokenResponse> GetTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default);
    Task<TokenResponse> GetRefreshTokenAsync(RefreshTokenRequest refreshTokenRequest, CancellationToken cancellationToken = default);
}