using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.DTOs;
public class GenderDto
{
    [Display(Name = "Gender")]
    public required int GenderId { get; set; }
    public required string Name { get; set; }
}
