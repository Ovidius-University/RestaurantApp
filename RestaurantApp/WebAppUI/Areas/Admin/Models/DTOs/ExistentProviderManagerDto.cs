using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.DTOs;
public class ExistentProviderManagerDto
{
    public int ManagerId { get; set; }
    public int ProviderId { get; set; }
}