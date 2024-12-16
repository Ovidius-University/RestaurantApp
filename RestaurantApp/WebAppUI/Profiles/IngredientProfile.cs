using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class IngredientProfile : Profile
{
    public IngredientProfile()
    {
        //CreateMap<NewIngredientDto, Ingredient>();

        CreateMap<Areas.Manager.Models.DTOs.NewIngredientDto, Ingredient>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Ingredient, Areas.Manager.Models.DTOs.ExistentIngredientDto>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id));

        CreateMap<Areas.Chef.Models.DTOs.NewIngredientDto, Ingredient>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Ingredient, Areas.Chef.Models.DTOs.ExistentIngredientDto>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id));

        CreateMap<Ingredient, Areas.Admin.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<IngredientFood, Areas.Admin.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Ingredient!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Ingredient!.Name}"));

        CreateMap<Ingredient, Areas.Manager.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<IngredientFood, Areas.Manager.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Ingredient!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Ingredient!.Name}"));

        CreateMap<Ingredient, Areas.Chef.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<IngredientFood, Areas.Chef.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Ingredient!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Ingredient!.Name}"));

        CreateMap<Ingredient, Areas.Cashier.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<IngredientFood, Areas.Cashier.Models.ViewModels.ShortIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Ingredient!.Id))
            .ForMember(d => d.Name, s => s.MapFrom(src => $"{src.Ingredient!.Name}"));

        CreateMap<Ingredient, Areas.Admin.Models.ViewModels.IngredientVm>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name));

        CreateMap<Ingredient, Areas.Manager.Models.ViewModels.IngredientDetailsVm>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.Provider, s => s.MapFrom(src => src.Provider!.Name));

        CreateMap<Ingredient, Areas.Chef.Models.ViewModels.IngredientDetailsVm>()
            .ForMember(d => d.Name, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.Provider, s => s.MapFrom(src => src.Provider!.Name));

        CreateMap<Ingredient, Areas.Chef.Models.DTOs.IngredientAddFoodDto>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Ingredient, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Ingredient, Areas.Manager.Models.DTOs.IngredientAddFoodDto>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Ingredient, s => s.MapFrom(src => $"{src.Name}"));

        CreateMap<Ingredient, CardIngredientVm>()
            .ForMember(d => d.IngredientId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Provider, s => s.MapFrom(src => src.Provider!.Name));
    }
}