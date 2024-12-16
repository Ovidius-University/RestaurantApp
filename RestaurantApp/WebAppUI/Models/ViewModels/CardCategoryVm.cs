using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardCategoryVm
{
    [Key]
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    // public CardCategoryVm(Category Category)
    // {
    //     CategoryId = Category.Id;
    //     Name = Category.Name;
    // }
}