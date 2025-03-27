using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Queries;

public class GetRolesQuery : IRequest<IResponseWrapper>
{
    
}

public class GetRolesQueryHandler(IRoleService roleService) : IRequestHandler<GetRolesQuery, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        return await roleService.GetRolesAsync();
    }
}