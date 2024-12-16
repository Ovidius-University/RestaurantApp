using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Models.Entities;
[Table("FoodIngredient")]
[PrimaryKey(nameof(FoodId), nameof(IngredientId))]
public class IngredientFood
{
    public int FoodId { get; set; }
    [ForeignKey(nameof(FoodId))]
    public Food? Food { get; set; }
    public int IngredientId { get; set; }
    [ForeignKey(nameof(IngredientId))]
    public Ingredient? Ingredient { get; set; }
}