using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ExistentReviewerDto
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public List<ExistentCriticDto>? Critics { get; set; }
}
