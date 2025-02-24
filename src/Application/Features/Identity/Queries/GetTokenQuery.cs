using Application.Services.Identity;
using Common.Requests;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetTokenQuery : IRequest<IResponseWrapper>
{
    public TokenRequest TokenRequest { get; set; }
    
}

public class GetTokenQueryHandler(ITokenService tokenService) : IRequestHandler<GetTokenQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetTokenQuery request, CancellationToken cancellationToken)
    {
        return await tokenService.GetTokenAsync(request.TokenRequest);
    }
}