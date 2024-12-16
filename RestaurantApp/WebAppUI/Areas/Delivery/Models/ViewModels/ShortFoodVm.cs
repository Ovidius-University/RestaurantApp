using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Delivery.Models.ViewModels;
public class ShortFoodVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Stock {  get; set; }
    public bool IsFinal { get; set; }
}
