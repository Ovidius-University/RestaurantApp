using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WebAppUI.Models.Entities;
[Table("FoodOffer")]
public class FoodOffer
{
    [Column("FoodOfferId")]
    public int Id { get; set; }
    [ForeignKey(nameof(Id))]
    public Food? Food { get; set; }
    public string PromoText { get; set; } = string.Empty;
    [Column(TypeName = "decimal(7,2)")]
    public decimal NewPrice { get; set; }
}