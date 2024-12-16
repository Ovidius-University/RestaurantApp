using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("PayingMethod")]
public class PayingMethod
{
    [Column("PayingMethodId")]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Order>? Orders { get; set; }
}
