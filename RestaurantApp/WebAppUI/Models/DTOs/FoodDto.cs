using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.DTOs;
public class FoodDto
{
    public int FoodId { get; set; }
    public required string Title { get; set; }
    public required int Stock { get; set; }
}
