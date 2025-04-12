using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands;

public class UserUpdateCommand : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
    public UpdateUserRequest UpdateUserRequest { get; set; }
}

public class UserUpdateCommandHandler(IUserService userService) : IRequestHandler<UserUpdateCommand, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(UserUpdateCommand request, CancellationToken cancellationToken)
    {
        return userService.UpdateUserAsync(request.UpdateUserRequest,request.UserId);
    }
}