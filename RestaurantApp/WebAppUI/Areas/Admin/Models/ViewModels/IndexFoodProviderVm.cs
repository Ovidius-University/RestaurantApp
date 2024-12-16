namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class IndexFoodProviderVm
{
    public string Name { get; set; } = string.Empty;
    public List<IngredientVm>? ListIngredients { get; set; }
    public List<FoodVm>? ListFoods { get; set; }
}
