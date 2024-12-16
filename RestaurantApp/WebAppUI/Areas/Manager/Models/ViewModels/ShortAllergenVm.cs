using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Manager.Models.ViewModels;
public class ShortAllergenVm
{
    public int AllergenId { get; set; }
    [Display(Name = "Allergens")]
    public string Name { get; set; } = string.Empty;
    //public ShortAllergenVm(int id, string name)
    //{
    //    Id = id;
    //    Name = $"{name}" ;
    //}
}
