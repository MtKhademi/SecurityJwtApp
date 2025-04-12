using Application.Services.Identity;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetUserByIdQuery : IRequest<IResponseWrapper>
{
    public string UserId { get; set; }
}

public class GetUserByIdQueryHandler(
    IUserService userService) : IRequestHandler<GetUserByIdQuery, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return userService.GetUserByIdAsync(request.UserId);
    }
}