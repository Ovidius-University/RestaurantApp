using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class InformationProfile : Profile
{
    public InformationProfile()
    {
        CreateMap<Areas.Admin.Models.DTOs.ExistentInformationDto, Information>();
        CreateMap<Information, Areas.Admin.Models.DTOs.ExistentInformationDto>();
        CreateMap<Information, Models.DTOs.ExistentInformationDto>();
    }
}