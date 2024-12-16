using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ReviewerCriticDto
{
    [Key]
    public int ReviewerId { get; set; }
    public string Reviewer { get; set; } = string.Empty;
    [Display(Name ="Critic")]
    public int CriticId { get; set; }
}
