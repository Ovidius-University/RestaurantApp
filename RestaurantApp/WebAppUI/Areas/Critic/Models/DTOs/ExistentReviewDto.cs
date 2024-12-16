using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Critic.Models.DTOs;

public class ExistentReviewDto
{
    public int FoodId { get; set; }
    public string Food {  get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string Reviewer { get; set; } = string.Empty;
    [Required, MaxLength(100)]
    public required string Content { get; set; }
    [Display(Name = "Is it final?")]
    public bool IsFinal { get; set; }    
    public void ToEntity(ref Review ExistentReview)
    {
        if (ExistentReview.FoodId == FoodId && ExistentReview.ReviewerId == ReviewerId)
        {
            ExistentReview.Content = Content;
            ExistentReview.IsFinal = IsFinal;
        }
    }
}
