using WebAppUI.Models.CustomIdentity;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;

namespace WebAppUI.Profiles;

public class RoleProfile : Profile
{
    public RoleProfile()
    {
        CreateMap<AppRole, NewRoleDto>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<AppRole, ExistentRole>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));
    }
}