using System.ComponentModel.DataAnnotations;
using WebAppUI.Models.ViewModels;
using WebAppUI.Validators;
namespace WebAppUI.Models.DTOs;

public class NewOrderDto
{
    [Required]
    public required int CustomerId { get; set; }
    [Required] 
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Is it a delivery to you? If you'll pick it up then uncheck the box")]
    public bool IsDelivery { get; set; } = true;
    [Required, MaxLength(100), Display(Name = "Customer name (for the order)")]
    public string Name { get; set; } = string.Empty;
    [Required(AllowEmptyStrings = true), DisplayFormat(ConvertEmptyStringToNull = false), MaxLength(100), Display(Name = "Address with street, building, apartment etc. (if it's a pickup, enter a space)")]
    public string? Address { get; set; } = " ";
    [Required, Display(Name = "Paying Method")]
    public int PayingMethodId { get; set; }
    [DisplayFormat(DataFormatString = "{0:###0.00}"), Tip]
    public decimal Tip { get; set; } = 0;
    [Required, Display(Name = "Wanted Arrival Time (Either for us, or for you; It needs to be within our work schedule)"), Order("IsDelivery"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy HH:mm:ss}")]
    public DateTime ArrivalTime { get; set; } = DateTime.Now.AddHours(1);
    public List<ShopCartVm>? ShopCart { get; set; }
    public CardFoodVm? Suggestion { get; set; }
}
