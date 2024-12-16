using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Chef.Models.DTOs;
public class CategoryDto
{
    [Display(Name = "Category")]
    public required int CategoryId { get; set; }
    public required string Name { get; set; }
}
