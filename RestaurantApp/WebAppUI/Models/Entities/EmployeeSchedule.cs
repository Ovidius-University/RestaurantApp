using WebAppUI.Models.CustomIdentity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebAppUI.Models.Entities;

[Table("EmployeeSchedule")]
[PrimaryKey(nameof(EmployeeId), nameof(DayId))]
public class EmployeeSchedule
{
    public int EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public AppUser? Employee { get; set; }
    public int DayId { get; set; }
    [ForeignKey(nameof(DayId))]
    public WorkHour? Day { get; set; }
    public TimeSpan StartHour { get; set; }
    public TimeSpan EndHour { get; set; }
}
