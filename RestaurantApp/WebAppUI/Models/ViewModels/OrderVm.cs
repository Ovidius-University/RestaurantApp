﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppUI.Validators;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Models.ViewModels;
public class OrderVm
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string Customer { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int? WorkerId { get; set; }
    [Display(Name = "Is it a delivery?")]
    public bool IsDelivery { get; set; }
    public string Address { get; set; } = string.Empty;
    [Tip]
    public decimal Tip { get; set; }
    [Display(Name = "Paying Method")]
    public int PayingMethodId { get; set; }
    [Display(Name = "Paying Method")]
    public string PayingMethod { get; set; } = string.Empty;
    [Display(Name = "Order Time")]
    public DateTime OrderTime { get; set; }
    [Display(Name = "Picked Arrival Time")]
    public DateTime ArrivalTime { get; set; }
    [Display(Name = "Delivery Time")]
    public DateTime DeliveryTime { get; set; }
    [Display(Name = "Was the order received?")]
    public bool IsFinal { get; set; }
    [Price, Display(Name = "Cost (without tip)")]
    public decimal Cost { get; set; }
    public List<OrderContentVm>? ListOrderContents { get; set; }
}
