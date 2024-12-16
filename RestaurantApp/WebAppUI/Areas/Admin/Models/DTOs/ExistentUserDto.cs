using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;
public class ExistentUserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    [Display(Name = "Administrator?")]
    public bool IsAdmin { get; set; } = false;
    [Display(Name ="Manager?")]
    public bool IsManager { get; set; } = false;
    [Display(Name = "Critic?")]
    public bool IsCritic { get; set; } = false;
    [Display(Name = "Chef?")]
    public bool IsChef { get; set; } = false;
    [Display(Name = "Delivery worker?")]
    public bool IsDelivery { get; set; } = false;
    [Display(Name = "Cashier?")]
    public bool IsCashier { get; set; } = false;
}
