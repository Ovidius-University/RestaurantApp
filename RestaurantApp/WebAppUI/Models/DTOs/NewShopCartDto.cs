using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Models.DTOs;

public class NewShopCartDto
{
    [Display(Name = "Food item")]
    public required int FoodId { get; set; }
    public required int CustomerId { get; set; }
    [Quantity]
    public required int Quantity { get; set; }
}
