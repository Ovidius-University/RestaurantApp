using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ExistentProviderDto
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public List<ExistentManagerDto>? Managers { get; set; }
}
