using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.DTOs;
public class ExistentReviewerCriticDto
{
    public int CriticId { get; set; }
    public int ReviewerId { get; set; }
}