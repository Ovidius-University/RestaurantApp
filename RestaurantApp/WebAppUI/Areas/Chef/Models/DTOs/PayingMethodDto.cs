using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Chef.Models.DTOs;
public class PayingMethodDto
{
    [Display(Name = "Paying Method")]
    public required int PayingMethodId { get; set; }
    public required string Name { get; set; }
}
