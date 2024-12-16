using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ProviderManagerDto
{
    [Key]
    public int ProviderId { get; set; }
    public string Provider { get; set; } = string.Empty;
    [Display(Name ="Manager")]
    public int ManagerId { get; set; }
}
