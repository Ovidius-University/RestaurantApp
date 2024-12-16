namespace WebAppUI.Areas.Delivery.Models.ViewModels;

public class IndexOrderContentsVm
{
    public required int OrderId { get; set; }
    public List<OrderContentVm>? ListOrderContents { get; set; }
}
