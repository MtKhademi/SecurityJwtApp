using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Role.Queries;

public class GetPermissionsQuery : IRequest<IResponseWrapper>
{
    public string RoleId { get; set; }
}

public class GetPermissionsQueryHandler(IRoleService roleService) : IRequestHandler<GetPermissionsQuery, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(GetPermissionsQuery request, CancellationToken cancellationToken)
    {
        return roleService.GetPermissionsAsync(request.RoleId);
    }
}