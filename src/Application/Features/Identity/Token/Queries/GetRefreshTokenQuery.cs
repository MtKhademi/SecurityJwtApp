using Application.Services.Identity;
using Common.Requests;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetRefreshTokenQuery : IRequest<IResponseWrapper>
{
    public RefreshTokenRequest RefreshTokenRequest { get; set; }
}

internal class GetRefreshTokenQueryHandler(ITokenService tokenService) : IRequestHandler<GetRefreshTokenQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRefreshTokenQuery request, CancellationToken cancellationToken)
    {
        return await tokenService.GetRefreshTokenAsync(request.RefreshTokenRequest);
    }
}