using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Critic.Models.DTOs;

public class NewReviewDto
{
    [Display(Name = "Food item")]
    public required int FoodId { get; set; }
    public required int ReviewerId { get; set; }

    [Required, MaxLength(100)]
    public required string Content { get; set; }
}
