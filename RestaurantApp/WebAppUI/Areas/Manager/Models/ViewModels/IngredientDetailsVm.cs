using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Manager.Models.ViewModels;
public class IngredientDetailsVm
{
    public int Id { get; set; }
    [Display(Name = "Ingredient")]
    public string Name{ get; set; }=string.Empty;
    [Display(Name = "Provider")]
    public string Provider { get; set; } = string.Empty;
}
