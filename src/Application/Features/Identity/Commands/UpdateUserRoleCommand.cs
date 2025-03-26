using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands;

public class UpdateUserRoleCommand : IRequest<IResponseWrapper>
{
    public UpdateUserRoleRequest Request { get; set; }
}

public class UpdateUserRoleCommandHandler(IUserService userService) : IRequestHandler<UpdateUserRoleCommand, IResponseWrapper>
{
    public async Task<IResponseWrapper> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
    {
        return await userService.UpdateUserRolesAsync(request.Request);
    }
}