using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Cashier.Models.DTOs;
public class CategoryDto
{
    [Display(Name = "Category")]
    public required int CategoryId { get; set; }
    public required string Name { get; set; }
}
