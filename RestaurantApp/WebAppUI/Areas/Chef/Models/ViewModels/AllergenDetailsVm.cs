using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Chef.Models.ViewModels;
public class AllergenDetailsVm
{
    public int Id { get; set; }
    [Display(Name = "Allergen")]
    public string Name{ get; set; }=string.Empty;
}
