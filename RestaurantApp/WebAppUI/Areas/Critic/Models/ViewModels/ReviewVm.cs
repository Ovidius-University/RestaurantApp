using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Critic.Models.ViewModels;
public class ReviewVm
{
    public int ReviewerId { get; set; }
    public int FoodId { get; set; }
    public string Content { get; set; } = string.Empty;
    [Display(Name = "Is It Final?")]
    public bool IsFinal { get; set; } = false;
    public ShortReviewerVm? Reviewer { get; set; }
    public ShortFoodVm? Food { get; set; }
}
