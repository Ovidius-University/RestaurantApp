using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Models.DTOs;

public class ExistentShopCartDto
{

    public int FoodId { get; set; }
    public string Food { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    [Quantity]
    public required int Quantity { get; set; }
    public void ToEntity(ref ShopCart ExistentShopCart)
    {
        if (ExistentShopCart.FoodId == FoodId && ExistentShopCart.CustomerId == CustomerId)
        {
            ExistentShopCart.Quantity = Quantity;
        }
    }
}
