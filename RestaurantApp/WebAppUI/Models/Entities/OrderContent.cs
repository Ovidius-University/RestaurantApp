using WebAppUI.Models.CustomIdentity;
using WebAppUI.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Models.Entities;
[Table("OrderContent")]
[PrimaryKey(nameof(OrderId), nameof(FoodId))]
public class OrderContent
{
    public int OrderId { get; set; }
    [ForeignKey(nameof(OrderId))]
    public Order? Order { get; set; }
    public int FoodId { get; set; }
    [ForeignKey(nameof(FoodId))]
    public Food? Food { get; set; }
    [Column(TypeName = "decimal(7,2)"), Price]
    public decimal UnitPrice { get; set; }
    [Quantity]
    public int Quantity { get; set; }
}
