using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Chef.Models.ViewModels;

public class SaleTimeItemVm
{
    public int id { get; set; }
    public int FoodId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Display(Name = "Total Sales")]
    public decimal TotalSales { get; set; }
}
