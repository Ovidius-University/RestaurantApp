using Microsoft.DotNet.Scaffolding.Shared;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Models.CustomIdentity;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;

public class ChefProfile : Profile
{
    public ChefProfile()
    {
        //CreateMap<NewChefDto, Chef>();
        //CreateMap<Chef, ExistentChefDto>();

        CreateMap<ExistentUserDto, CardChefVm>()
            .ForMember(d => d.ChefId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Email));
    }
}