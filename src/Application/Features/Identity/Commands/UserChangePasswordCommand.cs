using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands;

public class UserChangePasswordCommand : IRequest<IResponseWrapper>
{
    public ChangePasswordRequest ChangePasswordRequest { get; set; }
    public string UserId { get; set; }
}

public class UserChangePasswordCommandHandler(IUserService userService) : IRequestHandler<UserChangePasswordCommand, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(UserChangePasswordCommand request, CancellationToken cancellationToken)
    {
        return userService.ChangeUserPasswordAsync(request.ChangePasswordRequest,request.UserId);
    }
}