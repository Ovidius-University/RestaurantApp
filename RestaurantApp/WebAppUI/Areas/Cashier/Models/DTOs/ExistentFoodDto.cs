using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Cashier.Models.DTOs;

public class ExistentFoodDto
{
    public int Id { get; set; }
    [MaxLength(400)]
    public required string Title { get; set; }
    [DisplayFormat(DataFormatString = "{0:##0.00}")]
    [Display(Name = "Price"), Price]
    public decimal Price { get; set; }
    [Display(Name = "Stock (Setting it to 0 will remove it from everyone's shopping cart)"), Stock]
    public int Stock { get; set; }
    public void ToEntity(ref Food ExistentFood)
    {
        if (ExistentFood.Id == Id)
        {
            ExistentFood.Stock = Stock;
        }
    }
}

