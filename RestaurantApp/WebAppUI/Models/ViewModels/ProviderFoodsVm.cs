namespace WebAppUI.Models.ViewModels;
public class ProviderFoodsVm
{
    public CardProviderVm? ProviderDetails { get; set; }
    public List<CardFoodVm>? Foods { get; set; }
    public List<CardIngredientVm>? Ingredients { get; set; }
}