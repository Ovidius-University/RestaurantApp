namespace WebAppUI.Areas.Manager.Models.ViewModels;

public class IndexFoodsVm
{
    public required string Provider { get; set; }
    public List<FoodVm>? ListFoods { get; set; }
}
