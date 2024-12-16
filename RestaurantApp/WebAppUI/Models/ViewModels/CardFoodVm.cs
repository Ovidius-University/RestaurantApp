namespace WebAppUI.Models.ViewModels;
public class CardFoodVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string PromoText { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Chef { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    public string HomeMade { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal Weight { get; set; }
    public decimal NewPrice { get; set; }
    public int Calories { get; set; }
    public int Stock { get; set; }
    // public CardFoodVm()
    // {
    // }
    // public CardFoodVm(Food Food)
    // {
    //     if(Food is not null)
    //     {
    //         Id=Food.Id;
    //         Title=Food.Title;
    //         Price=Food.Price;
    //         NewPrice=Food.Offer?.NewPrice??0;
    //         Ingredients=string.Join(", ",Food.Ingredients!.Select(a=>$"{a.Ingredient!.Name}"));
    //     }
    // }
}