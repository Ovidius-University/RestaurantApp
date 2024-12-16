using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Manager.Models.ViewModels;
public class FoodDetailsVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Calories { get; set; }
    public decimal Weight { get; set; }
    public int Stock { get; set; }
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; }
    public required string Ingredients { get; set; }
    public required string Category { get; set; }
}
