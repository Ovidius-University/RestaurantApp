using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Cashier.Models.ViewModels;
public class FoodVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Chef { get; set; } = string.Empty;
    [Price]
    public decimal Price { get; set; }
    [Weight]
    public decimal Weight { get; set; }
    [Stock]
    public int Stock { get; set; }
    [Display(Name = "Is it final?")]
    public bool IsFinal { get; set; } = false;
    public List<ShortIngredientVm>? ListIngredients { get; set; }
}
