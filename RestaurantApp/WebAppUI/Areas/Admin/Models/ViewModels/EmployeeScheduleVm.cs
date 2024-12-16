using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Areas.Admin.Models.ViewModels;
public class EmployeeScheduleVm
{
    public int EmployeeId { get; set; }
    public string Employee { get; set; } = string.Empty;
    public int DayId { get; set; }
    public string Day { get; set; } = string.Empty;
    [Required, Display(Name = "Start Hour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan StartHour { get; set; }
    [Required, Display(Name = "End Hour"), BiggerHour("StartHour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan EndHour { get; set; }
}
