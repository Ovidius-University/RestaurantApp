using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Models.ViewModels;
public class ShopCartVm
{
    public int CustomerId { get; set; }
    public int FoodId { get; set; }
    [Display(Name = "Unit Price"), Price]
    public decimal UnitPrice { get; set; }
    [Quantity]
    public int Quantity { get; set; }
    [Display(Name = "Total Price"), Price]
    public decimal TotalPrice { get; set; }
    public ShortFoodVm? Food { get; set; }
}
