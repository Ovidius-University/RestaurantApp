using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class ShortIngredientVm
{
    public int IngredientId { get; set; }
    [Display(Name = "Ingredients")]
    public string Name { get; set; } = string.Empty;
}
