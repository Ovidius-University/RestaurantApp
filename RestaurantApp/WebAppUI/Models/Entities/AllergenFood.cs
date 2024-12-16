using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Models.Entities;
[Table("FoodAllergen")]
[PrimaryKey(nameof(FoodId), nameof(AllergenId))]
public class AllergenFood
{
    public int FoodId { get; set; }
    [ForeignKey(nameof(FoodId))]
    public Food? Food { get; set; }
    public int AllergenId { get; set; }
    [ForeignKey(nameof(AllergenId))]
    public Allergen? Allergen { get; set; }
}