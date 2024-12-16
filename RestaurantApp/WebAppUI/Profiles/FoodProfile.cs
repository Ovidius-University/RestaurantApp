using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Models.ViewModels;
using WebAppUI.Models.DTOs;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WebAppUI.Profiles;

public class FoodProfile : Profile
{
    public FoodProfile()
    {
        CreateMap<Food, Areas.Admin.Models.ViewModels.FoodVm>()
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : 0))
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));

        CreateMap<Food, Areas.Chef.Models.ViewModels.FoodVm>()
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));

        CreateMap<Food, Areas.Manager.Models.ViewModels.FoodVm>()
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));

        CreateMap<Food, Areas.Cashier.Models.ViewModels.FoodVm>()
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : src.Price))
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));

        CreateMap<Areas.Chef.Models.DTOs.NewFoodDto, Food>()
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => false))
            .ForMember(d => d.IsHomeMade, s => s.MapFrom(src => true));

        CreateMap<Food, Areas.Chef.Models.DTOs.ExistentFoodDto>();

        CreateMap<Areas.Manager.Models.DTOs.NewFoodDto, Food>()
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => false))
            .ForMember(d => d.IsHomeMade, s => s.MapFrom(src => false));

        CreateMap<Food, Areas.Manager.Models.DTOs.ExistentFoodDto>();

        CreateMap<Food, Areas.Cashier.Models.DTOs.ExistentFoodDto>()
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : src.Price));

        CreateMap<Food, Areas.Critic.Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title));

        CreateMap<Food, Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<Food, Areas.Admin.Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<Food, Areas.Chef.Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<Food, Areas.Manager.Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<Food, Areas.Cashier.Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<Food, Areas.Delivery.Models.ViewModels.ShortFoodVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock))
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => src.IsFinal));

        CreateMap<Food, Areas.Chef.Models.DTOs.FoodDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<Food, Areas.Manager.Models.DTOs.FoodDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<Food, Areas.Admin.Models.DTOs.FoodDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<Food, Areas.Critic.Models.DTOs.FoodDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<Food, Areas.Cashier.Models.DTOs.FoodDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"));

        CreateMap<Food, Models.DTOs.FoodDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => $"{src.Title}"))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock));

        CreateMap<Food, Areas.Chef.Models.ViewModels.FoodDetailsVm>()
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));

        CreateMap<Food, Areas.Manager.Models.ViewModels.FoodDetailsVm>()
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));

        CreateMap<Food, Areas.Cashier.Models.ViewModels.FoodDetailsVm>()
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : src.Price))
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name));
        
        CreateMap<FoodOffer, Areas.Chef.Models.ViewModels.FoodOfferVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Food!.Price))
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.NewPrice));

        CreateMap<FoodOffer, Areas.Chef.Models.DTOs.FoodOfferDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Ingredients, s => s.MapFrom(src => string.Join(", ", src.Food!.Ingredients!.Select(a => $"{a.Ingredient!.Name}"))))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Food!.Price));
        /**/
        CreateMap<Food, Areas.Chef.Models.DTOs.FoodOfferDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Price));
        /**/
        CreateMap<Areas.Chef.Models.DTOs.FoodOfferDto, FoodOffer>();

        CreateMap<Areas.Chef.Models.DTOs.AddEditFoodOfferDto, FoodOffer>();

        CreateMap<FoodOffer, Areas.Manager.Models.ViewModels.FoodOfferVm>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Food!.Price))
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.NewPrice));

        CreateMap<FoodOffer, Areas.Manager.Models.DTOs.FoodOfferDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Ingredients, s => s.MapFrom(src => string.Join(", ", src.Food!.Ingredients!.Select(a => $"{a.Ingredient!.Name}"))))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Food!.Price));
        /**/
        CreateMap<Food, Areas.Manager.Models.DTOs.FoodOfferDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Title))
            .ForMember(d => d.Price, s => s.MapFrom(src => src.Price));
        /**/
        CreateMap<Areas.Manager.Models.DTOs.FoodOfferDto, FoodOffer>();

        CreateMap<Areas.Manager.Models.DTOs.AddEditFoodOfferDto, FoodOffer>();

        CreateMap<Food, FoodPriceVm>();

        CreateMap<Food, CardFoodVm>()
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : 0))
            .ForMember(d => d.PromoText, s => s.MapFrom(src => src.Offer != null ? src.Offer.PromoText : string.Empty))
            .ForMember(d => d.Ingredients, s => s.MapFrom(src => string.Join(", ", src.Ingredients!.Select(a => $"{a.Ingredient!.Name}"))))
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name))
            .ForMember(d => d.HomeMade, s => s.MapFrom(src => src.IsHomeMade == true ? "Yes" : "No"))
            .ForMember(d => d.Chef, s => s.MapFrom(src => src.IsHomeMade == true ? src.Chef!.UserName : null))
            .ForMember(d => d.Provider, s => s.MapFrom(src => src.IsHomeMade == false ? src.Provider!.Name : null));

        CreateMap<Food, Models.ViewModels.FoodDetailsVm>()
            .ForMember(d => d.NewPrice, s => s.MapFrom(src => src.Offer != null ? src.Offer.NewPrice : 0))
            .ForMember(d => d.Category, s => s.MapFrom(src => src.Category!.Name))
            .ForMember(d => d.Ingredients, s => s.MapFrom(src => string.Join(", ", src.Ingredients!.Select(a => $"{a.Ingredient!.Name}"))))
            .ForMember(d => d.Allergens, s => s.MapFrom(src => string.Join(", ", src.Allergens!.Select(a => $"{a.Allergen!.Name}"))))
            .ForMember(d => d.HomeMade, s => s.MapFrom(src => src.IsHomeMade == true ? "Yes" : "No"))
            .ForMember(d => d.Chef, s => s.MapFrom(src => src.IsHomeMade == true ? src.Chef!.UserName : null))
            .ForMember(d => d.Provider, s => s.MapFrom(src => src.IsHomeMade == false ? src.Provider!.Name : null));

        /*
        CreateMap<Food, NewShopCartDto>()
            //.ForMember(d => d.FoodId, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Stock, s => s.MapFrom(src => src.Stock));
        */
    }
}