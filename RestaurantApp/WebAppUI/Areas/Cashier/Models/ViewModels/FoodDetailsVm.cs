using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Cashier.Models.ViewModels;
public class FoodDetailsVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Chef { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }
    [Display(Name = "Is it final?")]
    public bool IsFinal { get; set; }
    public required string Ingredients { get; set; }
    public required string Category { get; set; }
}
