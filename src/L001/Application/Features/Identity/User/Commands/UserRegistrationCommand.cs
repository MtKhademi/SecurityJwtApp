using Application.Services.Identity;
using Common.Requests.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Commands;

public class UserRegistrationCommand : IRequest<IResponseWrapper>
{
    public UserRegistrationRequest Request { get; set; }
}

public class UserRegistrationCommandHandler(IUserService userService)
    : IRequestHandler<UserRegistrationCommand, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(UserRegistrationCommand request, CancellationToken cancellationToken)
    {
        return userService.RegisterUserAsync(request.Request);
    }
}