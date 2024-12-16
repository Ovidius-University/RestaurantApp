using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.DTOs;
public class ShopCartDto
{
    public int FoodId { get; set; }
    public int CustomerId { get; set; }
    [Quantity]
    public required int Quantity { get; set; }
}
