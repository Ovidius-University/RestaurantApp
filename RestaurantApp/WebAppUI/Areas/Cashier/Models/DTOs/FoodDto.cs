using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Cashier.Models.DTOs;
public class FoodDto
{
    public int FoodId { get; set; }
    public required string Title { get; set; }
    public required int Stock { get; set; }
}
