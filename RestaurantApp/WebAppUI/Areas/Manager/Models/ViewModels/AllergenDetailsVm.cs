using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Manager.Models.ViewModels;
public class AllergenDetailsVm
{
    public int Id { get; set; }
    [Display(Name = "Allergen")]
    public string Name{ get; set; }=string.Empty;
}
