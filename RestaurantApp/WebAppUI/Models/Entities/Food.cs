using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using WebAppUI.Models.CustomIdentity;
namespace WebAppUI.Models.Entities;
[Table("Food")]
public class Food
{
    [Column("FoodId")]
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    [ForeignKey(nameof(CategoryId))]
    public Category? Category { get; set; }
    [Column(TypeName = "decimal(7,2)"), Price]
    public decimal Price { get; set; }
    [Column(TypeName = "decimal(7,2)"), Weight]
    public decimal Weight { get; set; }
    public int ChefId { get; set; }
    [ForeignKey(nameof(ChefId))]
    public AppUser? Chef { get; set; }
    public int ProviderId { get; set; }
    [ForeignKey(nameof(ProviderId))]
    public Provider? Provider { get; set; }
    [Calories]
    public int Calories { get; set; }
    [Stock]
    public int Stock {  get; set; }
    public bool IsFinal { get; set; } = false;
    public bool IsHomeMade { get; set; } = true;
    //public Image Image { get; set; }
    public FoodOffer? Offer { get; set; }
    public ICollection<IngredientFood>? Ingredients { get; set; }
    public ICollection<AllergenFood>? Allergens { get; set; }
}