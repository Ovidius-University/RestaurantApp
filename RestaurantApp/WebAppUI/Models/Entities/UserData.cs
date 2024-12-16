using WebAppUI.Models.CustomIdentity;
using WebAppUI.Validators;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("UserData")]
[PrimaryKey(nameof(UserId))]
public class UserData
{
    public int UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public AppUser? User { get; set; }
    public int GenderId { get; set; }
    [ForeignKey(nameof(GenderId))]
    public Gender? Gender { get; set; }
    [Birthday]
    public DateTime Birthday { get; set; }
}
