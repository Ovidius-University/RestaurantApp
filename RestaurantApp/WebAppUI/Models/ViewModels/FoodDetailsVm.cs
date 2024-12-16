namespace WebAppUI.Models.ViewModels;
public class FoodDetailsVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string Category { get; set; } = string.Empty;
    public int ChefId { get; set; }
    public string Chef { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string HomeMade { get; set; } = string.Empty;
    public List<CardIngredientVm>? ListIngredients { get; set; }
    public string Ingredients { get; set; } = string.Empty;
    public List<CardAllergenVm>? ListAllergens { get; set; }
    public string Allergens { get; set; } = string.Empty;
    public List<CardReviewerVm>? ListReviewers { get; set; }
    public string Reviewers { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public decimal NewPrice { get; set; }
    public int Stock { get; set; }
    public int Calories { get; set; }
    public string PromoText { get; set; } = string.Empty;
}