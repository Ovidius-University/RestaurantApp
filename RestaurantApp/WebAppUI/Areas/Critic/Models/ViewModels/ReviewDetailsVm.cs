using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Critic.Models.ViewModels;
public class ReviewDetailsVm
{
    public int FoodId { get; set; }
    public int ReviewerId { get; set; }
    public string Content { get; set; } = string.Empty;
    [Display(Name = "Is it final?")]
    public bool IsFinal { get; set; }
    public required string Reviewer { get; set; }
    public required string Food { get; set; }
}
