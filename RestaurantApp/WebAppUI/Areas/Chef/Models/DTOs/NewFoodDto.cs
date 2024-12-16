using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Chef.Models.DTOs;
public class NewFoodDto
{
    [Required, MaxLength(400)]
    public required string Title { get; set; }
    [Required, MaxLength(400)]
    public required string Description { get; set; }
    [Required, Display(Name = "Category")]
    public required int CategoryId { get; set; }
    [DisplayFormat(DataFormatString = "{0:###0.00}")]
    [Display(Name = "Price"), Price]
    public decimal Price { get; set; }
    [Display(Name = "Weight"), Weight]
    public decimal Weight { get; set; }
    [Display(Name = "Calories"), Calories]
    public int Calories { get; set; }
    [Display(Name = "Stock"), Stock]
    public int Stock { get; set; }

}
