using WebAppUI.Models.CustomIdentity;
using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebAppUI.Models.Entities;
[Table("Order")]
public class Order
{
    [Column("OrderId")]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    [ForeignKey(nameof(CustomerId))]
    public AppUser? Customer { get; set; }
    public int? WorkerId { get; set; }
    [ForeignKey(nameof(WorkerId))]
    public AppUser? DeliveryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsDelivery { get; set; } = false;
    public string? Address { get; set; } = string.Empty;
    [Column(TypeName = "decimal(7,2)"), Tip]
    public decimal Tip { get; set; }
    public int PayingMethodId { get; set; }
    [ForeignKey(nameof(PayingMethodId))]
    public PayingMethod? PayingMethod { get; set; }
    [Column(TypeName = "datetime2(7)")]
    public DateTime OrderTime { get; set; }
    [Column(TypeName = "datetime2(0)")]
    public DateTime ArrivalTime { get; set; }
    [Column(TypeName = "datetime2(7)")]
    public DateTime DeliveryTime { get; set; }
    public bool IsFinal { get; set; } = false;
    public FeedBack? FeedBack { get; set; }
    public ICollection<OrderContent>? OrderContents { get; set; }
}