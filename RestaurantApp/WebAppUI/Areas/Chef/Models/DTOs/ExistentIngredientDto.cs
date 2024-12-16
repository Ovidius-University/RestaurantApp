using System.ComponentModel.DataAnnotations;
using WebAppUI.Models.Entities;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Chef.Models.DTOs;
public class ExistentIngredientDto
{
    public int IngredientId { get; set; }
    [MaxLength(50),Required, Display(Name = "Name")]
    public string Name { get; set; } = string.Empty;
    public void ToEntity(ref Ingredient ExistentIngredient)
    {
        if(ExistentIngredient.Id == IngredientId)
        {
            ExistentIngredient.Name = Name;
        }
    }
}
