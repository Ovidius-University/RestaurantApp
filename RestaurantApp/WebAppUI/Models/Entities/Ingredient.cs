using System.ComponentModel.DataAnnotations.Schema;
using WebAppUI.Models.CustomIdentity;

namespace WebAppUI.Models.Entities;

[Table("Ingredient")]
public class Ingredient
{
    [Column("IngredientId")]
    public int Id { get; set; }
    [Column("Name")]
    public string Name { get; set; }=string.Empty;
    public int ProviderId { get; set; }
    [ForeignKey(nameof(ProviderId))]
    public Provider? Provider { get; set; }
    public ICollection<IngredientFood>? Foods { get; set; }
}
