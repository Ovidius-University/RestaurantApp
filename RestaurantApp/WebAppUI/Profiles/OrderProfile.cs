using WebAppUI.Areas.Admin.Models.DTOs;
using WebAppUI.Areas.Admin.Models.ViewModels;
using WebAppUI.Areas.Chef.Models.DTOs;
using WebAppUI.Areas.Chef.Models.ViewModels;
using WebAppUI.Areas.Cashier.Models.DTOs;
using WebAppUI.Areas.Cashier.Models.ViewModels;
using WebAppUI.Areas.Manager.Models.DTOs;
using WebAppUI.Models.DTOs;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Profiles;
public class OrderProfile : Profile
{
    public OrderProfile()
    {
        CreateMap<NewOrderDto, Order>()
            .ForMember(d => d.IsFinal, s => s.MapFrom(src => false))
            .ForMember(d => d.OrderTime, s => s.MapFrom(src => DateTime.Now));

        CreateMap<Order, CardOrderVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Models.ViewModels.OrderDetailsVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Models.ViewModels.OrderVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Models.DTOs.ExistentOrderDto>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<Order, Areas.Admin.Models.ViewModels.OrderDetailsVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Admin.Models.ViewModels.OrderVm>()
            .ForMember(d => d.Email, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Chef.Models.ViewModels.OrderDetailsVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Chef.Models.ViewModels.OrderVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Manager.Models.ViewModels.OrderDetailsVm>()
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Manager.Models.ViewModels.OrderVm>()
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Cashier.Models.ViewModels.OrderDetailsVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Delivery.Models.ViewModels.OrderDetailsVm>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.DeliveryName, s => s.MapFrom(src => src.DeliveryName!.UserName))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Cashier.Models.ViewModels.OrderVm>()
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Delivery.Models.ViewModels.OrderVm>()
            .ForMember(d => d.Comment, s => s.MapFrom(src => src.FeedBack!.Comment))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.DeliveryName, s => s.MapFrom(src => src.DeliveryName!.UserName))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Admin.Models.DTOs.ExistentOrderDto>()
            .ForMember(d => d.Email, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<Order, Areas.Chef.Models.DTOs.ExistentOrderDto>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<Order, Areas.Manager.Models.DTOs.ExistentOrderDto>()
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<Order, Areas.Cashier.Models.DTOs.ExistentOrderDto>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<Order, Areas.Delivery.Models.DTOs.ExistentOrderDto>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.DeliveryName, s => s.MapFrom(src => src.DeliveryName!.UserName))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<Order, Areas.Delivery.Models.DTOs.ExistentDeliveryDto>()
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.PayingMethod, s => s.MapFrom(src => src.PayingMethod!.Name));

        CreateMap<FeedBack, Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id));

        CreateMap<Order, Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Models.DTOs.OrderFeedBackDto, FeedBack>();

        CreateMap<FeedBack, Areas.Admin.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id));

        CreateMap<FeedBack, Areas.Chef.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id));

        CreateMap<FeedBack, Areas.Manager.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id));

        CreateMap<Order, Areas.Admin.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Customer, s => s.MapFrom(src => src.Name))
            .ForMember(d => d.Email, s => s.MapFrom(src => src.Customer!.UserName))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Chef.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<Order, Areas.Manager.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<FeedBack, Areas.Cashier.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id));

        CreateMap<Order, Areas.Cashier.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        CreateMap<FeedBack, Areas.Delivery.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id));

        CreateMap<Order, Areas.Delivery.Models.DTOs.OrderFeedBackDto>()
            .ForMember(d => d.Id, s => s.MapFrom(src => src.Id))
            .ForMember(d => d.Cost, s => s.MapFrom(src => src.OrderContents!.Sum(a => (decimal)a.Quantity * a.UnitPrice)));

        //CreateMap<Areas.Admin.Models.DTOs.OrderFeedBackDto, FeedBack>();
    }
}