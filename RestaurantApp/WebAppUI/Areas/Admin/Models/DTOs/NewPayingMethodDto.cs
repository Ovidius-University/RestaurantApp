using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;
public class NewPayingMethodDto
{
    [MaxLength(50)]
    public string Name { get; set; }=string.Empty;
}
