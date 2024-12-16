using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class GenderProfile : Profile
{
    public GenderProfile()
    {
        CreateMap<NewGenderDto, Gender>();
        CreateMap<Gender, ExistentGenderDto>();

        CreateMap<Gender, Models.DTOs.GenderDto>()
            .ForMember(d => d.GenderId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Gender, Areas.Admin.Models.DTOs.GenderDto>()
            .ForMember(d => d.GenderId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));
    }
}