namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class IndexChefFoodVm
{
    public string Name { get; set; } = string.Empty;
    public List<FoodVm>? ListFoods { get; set; }
}
