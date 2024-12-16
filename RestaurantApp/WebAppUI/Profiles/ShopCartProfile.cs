using WebAppUI.Models.CustomIdentity;
using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Areas.Critic.Models.DTOs;
using WebAppUI.Areas.Critic.Models.ViewModels;
using WebAppUI.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.Entities;
namespace WebAppUI.Profiles;
public class ShopCartProfile : Profile
{
    public ShopCartProfile()
    {
        CreateMap<ShopCart, ShopCartVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.Food!.Offer != null ? src.Food!.Offer.NewPrice : src.Food!.Price))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.Food!.Offer != null ? src.Food!.Offer.NewPrice*(decimal)src.Quantity : src.Food!.Price*(decimal)src.Quantity));

        CreateMap<NewShopCartDto, ShopCart>();

        CreateMap<ShopCart, ExistentShopCartDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.CustomerId));

        CreateMap<ShopCart, ShopCartDto>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.CustomerId))
            .ForMember(d => d.Quantity, s => s.MapFrom(src => src.Quantity));

        CreateMap<ShopCart, Models.ViewModels.ShopCartDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.Food!.Offer != null ? src.Food!.Offer.NewPrice : src.Food!.Price))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.Food!.Offer != null ? src.Food!.Offer.NewPrice*(decimal)src.Quantity : src.Food!.Price*(decimal)src.Quantity));
    }
}