using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ExistentRole
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
