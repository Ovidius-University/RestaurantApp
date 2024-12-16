using WebAppUI.Models.CustomIdentity;
using WebAppUI.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Models.Entities;
[Table("ShopCart")]
[PrimaryKey(nameof(CustomerId), nameof(FoodId))]
public class ShopCart
{
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public AppUser? Customer { get; set; }
    public int FoodId { get; set; }
    [ForeignKey(nameof(FoodId))]
    public Food? Food { get; set; }
    [Quantity]
    public int Quantity { get; set; }
}
