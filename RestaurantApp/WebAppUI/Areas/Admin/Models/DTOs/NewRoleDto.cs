using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Admin.Models.DTOs;
public class NewRoleDto
{
    [Key]
    public string Name { get; set; } = string.Empty;
}
