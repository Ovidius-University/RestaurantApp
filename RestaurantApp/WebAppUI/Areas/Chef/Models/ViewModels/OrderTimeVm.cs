namespace WebAppUI.Areas.Chef.Models.ViewModels;

public class OrderTimeVm
{
    public required int id { get; set; }
    public List<OrderVm>? ListOrders { get; set; }
}
