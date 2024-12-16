using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Critic.Models.ViewModels;
public class ShortFoodVm
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;    
}
