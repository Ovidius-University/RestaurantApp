namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class IndexOrderContentsVm
{
    public required int OrderId { get; set; }
    public List<OrderContentVm>? ListOrderContents { get; set; }
}
