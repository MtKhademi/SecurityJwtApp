using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands;

public class UserChangeStatusCommand : IRequest<IResponseWrapper>
{
    public ChangeUserStatusRequest Request { get; set; }
}

public class UserChangeStatusCommandHandler(IUserService userService) : IRequestHandler<UserChangeStatusCommand, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(UserChangeStatusCommand request, CancellationToken cancellationToken)
    {
        return userService.ChangeUserStatusAsync(request.Request);
    }
}