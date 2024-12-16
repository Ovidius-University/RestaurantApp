using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("Reviewer")]
public class Reviewer
{
    [Column("ReviewerId")]
    public int Id { get; set; }
    public string Name { get; set; }=string.Empty;
    public ICollection<Review>? Reviews { get; set; }
    public ICollection<ReviewerCritic>? Critics { get; set; }
}
