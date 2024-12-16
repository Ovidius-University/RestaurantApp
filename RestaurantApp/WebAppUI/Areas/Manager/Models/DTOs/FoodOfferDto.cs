using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Manager.Models.DTOs;
public class FoodOfferDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Ingredients { get; set; }
    [Required, Display(Name ="Promo Text")]
    public required string PromoText { get; set; }
    [Display(Name = "Old Price"), Price]
    public decimal Price { get; set; }
    [Required, Display(Name = "New Price"), SmallerPrice("Price"), Price]
    public decimal NewPrice { get; set; }
    public bool IsNewOffer { get; set; } = true;
}
