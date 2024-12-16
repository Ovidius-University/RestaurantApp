namespace WebAppUI.Models.ViewModels;

public class IndexOrdersVm
{
    public int? id {  get; set; }
    public required string Customer { get; set; }
    public required int CustomerId { get; set; }
    public List<OrderVm>? ListOrders { get; set; }
    public CardFoodVm? Suggestion { get; set; }
}
