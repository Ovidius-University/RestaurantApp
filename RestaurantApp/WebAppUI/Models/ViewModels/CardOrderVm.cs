using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardOrderVm
{
    [Key]
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int WorkerId { get; set; }
    public string DeliveryName { get; set; } = string.Empty;
    [Display(Name = "Is it a delivery to you?")]
    public bool IsDelivery { get; set; } = true;
    public string Address { get; set; } = string.Empty;
    [Tip]
    public decimal Tip { get; set; }
    [Display(Name = "Paying Method")]
    public int PayingMethodId { get; set; }
    [Display(Name = "Paying Method")]
    public string PayingMethod { get; set; } = string.Empty;
    [Display(Name = "Order Time")]
    public DateTime OrderTime { get; set; }
    [Display(Name = "Arrival Time")]
    public DateTime ArrivalTime { get; set; }
    [Display(Name = "Delivery Time")]
    public DateTime DeliveryTime { get; set; }
    [Display(Name = "Was the order received?")]
    public bool IsFinal { get; set; }
    [Price]
    [Display(Name = "Cost (without tip)")]
    public decimal Cost { get; set; }
}