using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Manager.Models.DTOs;
public class FoodDto
{
    public int FoodId { get; set; }
    public required string Title { get; set; }
}
