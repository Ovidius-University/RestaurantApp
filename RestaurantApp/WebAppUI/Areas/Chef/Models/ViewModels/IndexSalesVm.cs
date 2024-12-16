namespace WebAppUI.Areas.Chef.Models.ViewModels;

public class IndexSalesVm
{
    public required string Chef { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
