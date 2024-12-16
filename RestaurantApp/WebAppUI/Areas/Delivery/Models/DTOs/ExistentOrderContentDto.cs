using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Delivery.Models.DTOs;

public class ExistentOrderContentDto
{
    public int OrderId { get; set; }
    public int FoodId { get; set; }
    [Display(Name = "Food item")]
    public string Food { get; set; } = string.Empty;
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    [Quantity]
    public required int Quantity { get; set; }
    [Price]
    [Display(Name = "Unit Price")]
    public required decimal UnitPrice { get; set; }
    public void ToEntity(ref OrderContent ExistentOrderContent)
    {
        if (ExistentOrderContent.FoodId == FoodId && ExistentOrderContent.OrderId == OrderId)
        {
            ExistentOrderContent.Quantity = Quantity;
            ExistentOrderContent.UnitPrice = UnitPrice;
        }
    }
}
