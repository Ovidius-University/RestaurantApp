using System.ComponentModel.DataAnnotations;
using WebAppUI.Areas.Manager.Models.ViewModels;
namespace WebAppUI.Areas.Manager.Models.DTOs;
public class AllergenAddFoodDto
{
    public int AllergenId { get; set; }
    public string Allergen { get; set; } = string.Empty;
    [Display(Name ="Food item")]
    public int FoodId { get; set; }
}
