using WebAppUI.Models.CustomIdentity;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Chef.Models.DTOs;
namespace WebAppUI.Profiles;
public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<AppUser, Areas.Admin.Models.DTOs.ExistentUserDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Email, s => s.MapFrom(src => src.UserName));

        CreateMap<AppUser, Areas.Chef.Models.DTOs.ExistentUserDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Email, s => s.MapFrom(src => src.UserName));
    }
}