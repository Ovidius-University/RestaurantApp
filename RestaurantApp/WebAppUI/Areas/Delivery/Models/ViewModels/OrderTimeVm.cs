namespace WebAppUI.Areas.Delivery.Models.ViewModels;

public class OrderTimeVm
{
    public required int id { get; set; }
    public List<OrderVm>? ListOrders { get; set; }
}
