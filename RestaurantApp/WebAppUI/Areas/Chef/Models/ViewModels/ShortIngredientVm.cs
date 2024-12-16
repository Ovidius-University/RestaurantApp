using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Chef.Models.ViewModels;
public class ShortIngredientVm
{
    public int IngredientId { get; set; }
    [Display(Name = "Ingredients")]
    public string Name { get; set; }=string.Empty;
    //public ShortIngredientVm(int id, string name)
    //{
    //    Id = id;
    //    Name = $"{name}" ;
    //}
}
