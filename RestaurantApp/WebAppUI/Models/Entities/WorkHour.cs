using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities;

[Table("WorkHour")]
public class WorkHour
{
    [Column("DayId")]
    public int Id { get; set; }
    [Column("DayName")]
    public string Name { get; set; } = string.Empty;
    public TimeSpan StartHour { get; set; }
    public TimeSpan EndHour { get; set; }
    public bool IsWorkDay { get; set; } = true;
}
