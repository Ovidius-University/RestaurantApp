namespace WebAppUI.Areas.Manager.Models.ViewModels;

public class IndexSalesVm
{
    public required string Provider { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
