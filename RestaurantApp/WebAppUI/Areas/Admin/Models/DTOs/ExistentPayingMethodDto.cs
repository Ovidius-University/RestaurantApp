using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ExistentPayingMethodDto
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    //public List<ExistentOrderDto>? Orders { get; set; }
}
