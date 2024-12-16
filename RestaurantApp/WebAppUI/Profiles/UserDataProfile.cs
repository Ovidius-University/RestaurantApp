using WebAppUI.Models.CustomIdentity;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Critic.Models.DTOs;
using WebAppUI.Areas.Critic.Models.ViewModels;
using WebAppUI.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.Entities;
namespace WebAppUI.Profiles;
public class UserDataProfile : Profile
{
    public UserDataProfile()
    {
        CreateMap<UserData, UserDataVm>()
            .ForMember(d => d.User, s => s.MapFrom(src => src.User!.UserName))
            .ForMember(d => d.Gender, s => s.MapFrom(src => src.Gender!.Name));

        CreateMap<NewUserDataDto, UserData>();

        CreateMap<UserData, ExistentUserDataDto>()
            .ForMember(d => d.User, s => s.MapFrom(src => src.User!.UserName))
            .ForMember(d => d.Gender, s => s.MapFrom(src => src.Gender!.Name));
    }
}