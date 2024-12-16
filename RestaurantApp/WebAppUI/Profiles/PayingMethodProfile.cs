using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class PayingMethodProfile : Profile
{
    public PayingMethodProfile()
    {
        CreateMap<NewPayingMethodDto, PayingMethod>();
        CreateMap<PayingMethod, ExistentPayingMethodDto>();

        CreateMap<PayingMethod, Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<PayingMethod, Areas.Admin.Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<PayingMethod, Areas.Chef.Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<PayingMethod, Areas.Cashier.Models.DTOs.PayingMethodDto>()
            .ForMember(d => d.PayingMethodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));
    }
}