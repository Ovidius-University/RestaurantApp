using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class SaleVm
{
    public int FoodId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Display(Name = "Total Sales")]
    public decimal TotalSales { get; set; }
}
