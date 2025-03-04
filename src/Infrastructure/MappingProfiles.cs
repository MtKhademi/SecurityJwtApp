using AutoMapper;
using Common.Responses.Identity;

namespace Infrastructure;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<ApplicationUser, UserResponse>();
    }
}