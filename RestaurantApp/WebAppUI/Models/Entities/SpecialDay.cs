using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("SpecialDay")]
public class SpecialDay
{
    [Column("DayId")]
    public int Id { get; set; }
    public int Day { get; set; }
    public int Month { get; set; }
    public TimeSpan StartHour { get; set; }
    public TimeSpan EndHour { get; set; }
    public bool IsWorkDay { get; set; } = false;
}
