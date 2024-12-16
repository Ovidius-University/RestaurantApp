using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class ChefVm
{
    public int ChefId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Foods { get; set; }
}
