using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Models.Entities;

[Table ("Provider")]
public class Provider
{
    [Column("ProviderId")]
    public int Id { get; set; }
    public string Name { get; set; }=string.Empty;
    public ICollection<Ingredient>? Ingredients { get; set; }
    //public ICollection<Food>? Foods { get; set; }
    public ICollection<ProviderManager>? Managers { get; set; }
}
