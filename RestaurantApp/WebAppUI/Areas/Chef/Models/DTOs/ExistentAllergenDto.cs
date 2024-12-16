using System.ComponentModel.DataAnnotations;
using WebAppUI.Models.Entities;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Chef.Models.DTOs;
public class ExistentAllergenDto
{
    public int AllergenId { get; set; }
    [MaxLength(50),Required, Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;
    public void ToEntity(ref Allergen ExistentAllergen)
    {
        if(ExistentAllergen.Id == AllergenId)
        {
            ExistentAllergen.Name = Name;
        }
    }
}
