using Application.Services.Identity;
using AutoMapper;
using Common.Responses.Wrappers;
using MediatR;

namespace Application.Features.Identity.Queries;

public class GetAllUsersQuery : IRequest<IResponseWrapper>
{
    
}

public class GetAllUsersQueryHandler(IUserService userService,
    IMapper mapper) : IRequestHandler<GetAllUsersQuery, IResponseWrapper>
{
    public Task<IResponseWrapper> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        return userService.GetAllUsersAsync();
    }
} 