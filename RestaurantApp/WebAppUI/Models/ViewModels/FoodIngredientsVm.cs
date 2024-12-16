namespace WebAppUI.Models.ViewModels;
public class FoodIngredientsVm
{
    public CardFoodVm? FoodDetails { get; set; }
    public List<CardIngredientVm>? Ingredients { get; set; }
}