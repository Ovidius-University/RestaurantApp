using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class ReviewerVm
{
    public int ReviewerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Reviews { get; set; }
}
