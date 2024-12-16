using System.ComponentModel.DataAnnotations.Schema;
using WebAppUI.Models.CustomIdentity;

namespace WebAppUI.Models.Entities;

[Table("Allergen")]
public class Allergen
{
    [Column("AllergenId")]
    public int Id { get; set; }
    [Column("Name")]
    public string Name { get; set; }=string.Empty;
    public ICollection<AllergenFood>? Foods { get; set; }
}
