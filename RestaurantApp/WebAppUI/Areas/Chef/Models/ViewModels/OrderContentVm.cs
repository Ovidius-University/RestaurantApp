using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Chef.Models.ViewModels;
public class OrderContentVm
{
    public int OrderId { get; set; }
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
