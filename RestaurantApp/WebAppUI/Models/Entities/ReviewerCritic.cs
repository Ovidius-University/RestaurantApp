using WebAppUI.Models.CustomIdentity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;
[Table("ReviewerCritic")]
[PrimaryKey(nameof(CriticId), nameof(ReviewerId))]
public class ReviewerCritic
{
    [Column("UserId")]
    public int CriticId { get; set; }
    public int ReviewerId { get; set; }
    [ForeignKey(nameof(CriticId))]
    public AppUser? Critic { get; set; }
    [ForeignKey(nameof(ReviewerId))]
    public Reviewer? Reviewer { get; set; }
}