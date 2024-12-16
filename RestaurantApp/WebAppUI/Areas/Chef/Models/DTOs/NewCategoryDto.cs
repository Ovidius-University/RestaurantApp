using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Chef.Models.DTOs;
public class NewCategoryDto
{
    [MaxLength(50)]
    public string Name { get; set; }=string.Empty;
}
