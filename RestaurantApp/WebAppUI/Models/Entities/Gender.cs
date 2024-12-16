using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("Gender")]
public class Gender
{
    [Column("GenderId")]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}
