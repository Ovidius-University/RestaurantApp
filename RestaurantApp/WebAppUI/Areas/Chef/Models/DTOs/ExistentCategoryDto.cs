using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Chef.Models.DTOs;

public class ExistentCategoryDto
{
    [Display(Name = "Category")]
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}
