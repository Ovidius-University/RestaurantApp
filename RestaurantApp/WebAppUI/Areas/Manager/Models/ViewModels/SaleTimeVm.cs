namespace WebAppUI.Areas.Manager.Models.ViewModels;

public class SaleTimeVm
{
    public required int id { get; set; }
    public required string Provider { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
