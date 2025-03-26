using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetRolesQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetRolesQueryHandler(IUserService userService) : IRequestHandler<GetRolesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await userService.GetRolesAsync(request.UserId);
    }
}