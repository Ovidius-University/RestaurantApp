using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class WorkVm
{
    public int WorkerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int Orders { get; set; }
}
