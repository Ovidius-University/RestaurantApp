namespace WebAppUI.Areas.Chef.Models.ViewModels;

public class SaleTimeVm
{
    public required int id { get; set; }
    public required string Chef { get; set; }
    public List<SaleVm>? ListSales { get; set; }
}
