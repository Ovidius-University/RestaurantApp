using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class AllergenProfile : Profile
{
    public AllergenProfile()
    {
        //CreateMap<NewAllergenDto, Allergen>();

        CreateMap<Areas.Manager.Models.DTOs.NewAllergenDto, Allergen>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Allergen, Areas.Manager.Models.DTOs.ExistentAllergenDto>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id));
        CreateMap<Areas.Chef.Models.DTOs.NewAllergenDto, Allergen>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Allergen, Areas.Chef.Models.DTOs.ExistentAllergenDto>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id));
        CreateMap<Allergen, Areas.Manager.Models.ViewModels.ShortAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<AllergenFood, Areas.Manager.Models.ViewModels.ShortAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Allergen!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Allergen!.Name}"));
        
        CreateMap<Allergen, Areas.Chef.Models.ViewModels.ShortAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<AllergenFood, Areas.Chef.Models.ViewModels.ShortAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Allergen!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Allergen!.Name}"));

        CreateMap<Allergen, Areas.Cashier.Models.ViewModels.ShortAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<AllergenFood, Areas.Cashier.Models.ViewModels.ShortAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Allergen!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Allergen!.Name}"));

        CreateMap<Allergen, Areas.Manager.Models.ViewModels.AllergenDetailsVm>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));
        CreateMap<Allergen, Areas.Chef.Models.ViewModels.AllergenDetailsVm>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Allergen, Areas.Chef.Models.DTOs.AllergenAddFoodDto>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Allergen, s => s.MapFrom(src => $"{src.Name}"));
        
        CreateMap<Allergen, Areas.Manager.Models.DTOs.AllergenAddFoodDto>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Allergen, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Allergen, CardAllergenVm>()
            .ForMember(d => d.AllergenId, s => s.MapFrom(src => src.Id));
    }
}