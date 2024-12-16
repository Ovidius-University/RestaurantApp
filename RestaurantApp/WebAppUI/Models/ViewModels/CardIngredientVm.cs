using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardIngredientVm
{
    [Key]
    public int IngredientId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
    // public CardIngredientVm(Ingredient Ingredient)
    // {
    //     IngredientId = Ingredient.Id;
    //     Name = Ingredient.Name;
    // }
}