using Microsoft.DotNet.Scaffolding.Shared;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Models.CustomIdentity;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;

public class ProviderProfile : Profile
{
    public ProviderProfile()
    {
        CreateMap<NewProviderDto, Provider>();
        CreateMap<Provider, ExistentProviderDto>();

        CreateMap<Provider, CardProviderVm>()
            .ForMember(d => d.ProviderId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));
    }
}