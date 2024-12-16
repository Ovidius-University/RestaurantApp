using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;
[Table("Privacy")]
public class Privacy
{
    public int Id { get; set; }
    [Column("Policy")]
    public string Policy { get; set; } = string.Empty;
}
