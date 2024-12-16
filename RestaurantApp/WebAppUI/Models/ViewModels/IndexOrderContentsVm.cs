namespace WebAppUI.Models.ViewModels;

public class IndexOrderContentsVm
{
    public required string Customer { get; set; }
    public required int CustomerId { get; set; }
    public required int OrderId { get; set; }
    public List<OrderContentVm>? ListOrderContents { get; set; }
}
