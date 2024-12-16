using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class SpecialDayProfile : Profile
{
    public SpecialDayProfile()
    {
        CreateMap<NewSpecialDayDto, SpecialDay>();
        CreateMap<SpecialDay, ExistentSpecialDayDto>();
        CreateMap<ExistentSpecialDayDto, SpecialDay>();
        CreateMap<SpecialDay, ExistentSpecialDayDto>()
            .ForMember(d => d.StartHour, s => s.MapFrom(src => src.StartHour))
            .ForMember(d => d.EndHour, s => s.MapFrom(src => src.EndHour));
        /*
        CreateMap<SpecialDay, SpecialDayDto>()
            .ForMember(d => d.SpecialDayId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Day, s => s.MapFrom(src => src.Day))
            .ForMember(d => d.Month, s => s.MapFrom(src => src.Month));
        */
    }
}