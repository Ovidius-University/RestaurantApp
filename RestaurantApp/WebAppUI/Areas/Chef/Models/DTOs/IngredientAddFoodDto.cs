using System.ComponentModel.DataAnnotations;
using WebAppUI.Areas.Chef.Models.ViewModels;
namespace WebAppUI.Areas.Chef.Models.DTOs;
public class IngredientAddFoodDto
{
    public int IngredientId { get; set; }
    public string Ingredient { get; set; } = string.Empty;
    [Display(Name ="Food item")]
    public int FoodId { get; set; }
}
