using WebAppUI.Areas.Manager.Models.ViewModels;

namespace WebAppUI.Areas.Manager.Models.DTOs;
public class FoodEditDto
{
    public ExistentFoodDto? ExistentFood { get; set; }
    public List<ShortIngredientVm>? ListIngredients { get; set; }
}
