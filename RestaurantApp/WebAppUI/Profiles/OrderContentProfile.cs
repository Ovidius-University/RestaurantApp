using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class OrderContentProfile : Profile
{
    public OrderContentProfile()
    {
        CreateMap<ShopCartVm, OrderContent>()
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.Quantity, s => s.MapFrom(src => src.Quantity))
            .ForMember(d => d.Food, s => s.Ignore());

        CreateMap<OrderContent, Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<OrderContent, Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Admin.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Chef.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Manager.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Cashier.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Delivery.Models.ViewModels.OrderContentVm>()
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));
        
        CreateMap<Areas.Admin.Models.ViewModels.OrderContentVm, Areas.Admin.Models.ViewModels.SaleVm>()
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.TotalSales, s => s.MapFrom(src => src.TotalPrice));

        CreateMap<Areas.Chef.Models.ViewModels.OrderContentVm, Areas.Chef.Models.ViewModels.SaleVm>()
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.TotalSales, s => s.MapFrom(src => src.TotalPrice));

        CreateMap<Areas.Manager.Models.ViewModels.OrderContentVm, Areas.Manager.Models.ViewModels.SaleVm>()
            .ForMember(d => d.Title, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.TotalSales, s => s.MapFrom(src => src.TotalPrice));

        CreateMap<Areas.Admin.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<Areas.Chef.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<Areas.Manager.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<Areas.Cashier.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<Areas.Delivery.Models.DTOs.NewOrderContentDto, OrderContent>();

        CreateMap<OrderContent, Areas.Admin.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Chef.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Manager.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Cashier.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Areas.Delivery.Models.DTOs.ExistentOrderContentDto>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId));

        CreateMap<OrderContent, Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Admin.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Chef.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Manager.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Cashier.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));

        CreateMap<OrderContent, Areas.Delivery.Models.ViewModels.OrderContentDetailsVm>()
            .ForMember(d => d.Food, s => s.MapFrom(src => src.Food!.Title))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Order!.Customer!.UserName))
            .ForMember(d => d.UnitPrice, s => s.MapFrom(src => src.UnitPrice))
            .ForMember(d => d.FoodId, s => s.MapFrom(src => src.FoodId))
            .ForMember(d => d.CustomerId, s => s.MapFrom(src => src.Order!.CustomerId))
            .ForMember(d => d.TotalPrice, s => s.MapFrom(src => src.UnitPrice * (decimal)src.Quantity));
    }
}
