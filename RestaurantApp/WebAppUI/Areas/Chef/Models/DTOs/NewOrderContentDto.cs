using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Chef.Models.DTOs;

public class NewOrderContentDto
{
    public required int OrderId { get; set; }
    [Display(Name = "Pick a food item")]
    public required int FoodId { get; set; }
    [Price]
    [Display(Name = "Unit Price")]
    public decimal UnitPrice { get; set; }
    [Quantity]
    public required int Quantity { get; set; }
}
