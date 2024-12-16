using WebAppUI.Areas.Cashier.Models.ViewModels;

namespace WebAppUI.Areas.Cashier.Models.DTOs;
public class FoodEditDto
{
    public ExistentFoodDto? ExistentFood { get; set; }
    public List<ShortIngredientVm>? ListIngredients { get; set; }
}
