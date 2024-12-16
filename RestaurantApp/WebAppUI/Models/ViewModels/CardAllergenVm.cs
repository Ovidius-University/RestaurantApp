using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardAllergenVm
{
    [Key]
    public int AllergenId { get; set; }
    public string Name { get; set; } = string.Empty;
    // public CardAllergenVm(Allergen Allergen)
    // {
    //     AllergenId = Allergen.Id;
    //     Name = Allergen.Name;
    // }
}