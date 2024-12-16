using WebAppUI.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("Information")]
public class Information
{
    public int Id { get; set; }
    [Column("Name")]
    public string Name { get; set; } = string.Empty;
    [Column("Description")]
    public string Description { get; set; } = string.Empty;
    [Column("Location")]
    public string Location { get; set; } = string.Empty;
    [Column("PhoneNumber"), MinLength(10), MaxLength(10), Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    [Column("Email"), EmailAddress]
    public string Email { get; set; } = string.Empty;
}
