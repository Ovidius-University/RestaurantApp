using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class FoodProviderVm
{
    public int ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Ingredients { get; set; }
    public int Foods { get; set; }
}
