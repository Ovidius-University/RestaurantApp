using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WebAppUI.Models.Entities;
[Table("Review")]
[PrimaryKey(nameof(FoodId), nameof(ReviewerId))]
public class Review
{
    public int FoodId { get; set; }
    [ForeignKey(nameof(FoodId))]
    public Food? Food { get; set; }
    public int ReviewerId { get; set; }
    [ForeignKey(nameof(ReviewerId))]
    public Reviewer? Reviewer { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsFinal { get; set; } = false;
}