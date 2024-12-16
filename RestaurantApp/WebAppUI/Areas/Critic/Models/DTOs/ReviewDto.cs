using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Critic.Models.DTOs;
public class ReviewDto
{
    public int FoodId { get; set; }
    public int ReviewerId { get; set; }
    public required string Content { get; set; }
}
