using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class ShopCartDetailsVm
{
    public int FoodId { get; set; }
    public int CustomerId { get; set; }
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
    [Display(Name = "Total Price")]
    public decimal TotalPrice { get; set; }
    public required string Customer { get; set; }
    public required string Food { get; set; }
}
