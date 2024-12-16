using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class PrivacyProfile : Profile
{
    public PrivacyProfile()
    {
        CreateMap<Areas.Admin.Models.DTOs.ExistentPrivacyDto, Privacy>();
        CreateMap<Privacy, Areas.Admin.Models.DTOs.ExistentPrivacyDto>();
        CreateMap<Privacy, Models.DTOs.ExistentPrivacyDto>();
    }
}