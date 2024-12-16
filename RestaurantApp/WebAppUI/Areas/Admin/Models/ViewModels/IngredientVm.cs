using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class IngredientVm
{
    public int Id { get; set; }
    [Display(Name = "Ingredient")]
    public string Name{ get; set; } = string.Empty;
}
