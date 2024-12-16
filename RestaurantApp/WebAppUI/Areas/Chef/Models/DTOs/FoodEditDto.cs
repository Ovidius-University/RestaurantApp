using WebAppUI.Areas.Chef.Models.ViewModels;

namespace WebAppUI.Areas.Chef.Models.DTOs;
public class FoodEditDto
{
    public ExistentFoodDto? ExistentFood { get; set; }
    public List<ShortIngredientVm>? ListIngredients { get; set; }
}
