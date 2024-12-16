namespace WebAppUI.Areas.Chef.Models.ViewModels;

public class IndexFoodsVm
{
    public required string Chef { get; set; }
    public List<FoodVm>? ListFoods { get; set; }
}
