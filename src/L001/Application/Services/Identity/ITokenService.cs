using Common.Requests;
using Common.Requests.Identity;
using Common.Responses;
using Common.Responses.Wrappers;

namespace Application.Services.Identity;

public interface ITokenService
{
    Task<ResponseWrapper<TokenResponse>> GetTokenAsync(TokenRequest tokenRequest, CancellationToken cancellationToken = default);
    Task<ResponseWrapper<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest? refreshTokenRequest, CancellationToken cancellationToken = default);
}