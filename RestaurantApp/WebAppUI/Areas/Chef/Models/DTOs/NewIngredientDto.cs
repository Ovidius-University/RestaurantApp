using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Chef.Models.DTOs;
public class NewIngredientDto
{
    [Display(Name = "Name"), MaxLength(50,ErrorMessage ="{0} needs to be at max {1} characters!")]
    public string Name { get; set; } = string.Empty;
}
