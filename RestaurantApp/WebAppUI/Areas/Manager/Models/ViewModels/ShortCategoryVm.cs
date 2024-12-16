using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Manager.Models.ViewModels;
public class ShortCategoryVm
{
    public int CategoryId { get; set; }
    [Display(Name = "Category")]
    public string Name { get; set; }=string.Empty;
    //public ShortCategoryVm(int id, string name)
    //{
    //    Id = id;
    //    Name = name ;
    //}
}
