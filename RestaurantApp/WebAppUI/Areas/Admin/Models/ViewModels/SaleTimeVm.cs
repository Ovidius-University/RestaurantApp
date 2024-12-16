namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class SaleTimeVm
{
    public required int id { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
