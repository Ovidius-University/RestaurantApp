using System.ComponentModel.DataAnnotations;
using WebAppUI.Areas.Manager.Models.ViewModels;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Manager.Models.DTOs;

public class ExistentFoodDto
{
    public int Id { get; set; }
    [Required, MaxLength(400)]
    public required string Title { get; set; }
    [Required, MaxLength(400)]
    public required string Description { get; set; }
    [Required, Display(Name = "Category")]
    public required int CategoryId { get; set; }
    [DisplayFormat(DataFormatString = "{0:##0.00}")]
    [Required, Display(Name = "Price"), Price]
    public decimal Price { get; set; }
    [Required, Display(Name = "Calories"), Calories]
    public int Calories { get; set; }
    [Required, Display(Name = "Weight"), Weight]
    public decimal Weight { get; set; }
    [Required, Display(Name = "Stock (Setting it to 0 will remove it from everyone's shopping cart)"), Stock]
    public int Stock { get; set; }
    [Display(Name = "Is It Final? (Making it not final will remove it from everyone's shopping cart)")]
    public bool IsFinal { get; set; }
    public void ToEntity(ref Food ExistentFood)
    {
        if (ExistentFood.Id == Id)
        {
            ExistentFood.Title = Title;
            ExistentFood.Description = Description;
            ExistentFood.CategoryId = CategoryId;
            ExistentFood.Price = Price;
            ExistentFood.Stock = Stock;
            ExistentFood.IsFinal = IsFinal;
            ExistentFood.Weight = Weight;
            ExistentFood.Calories = Calories;
        }
    }
}

