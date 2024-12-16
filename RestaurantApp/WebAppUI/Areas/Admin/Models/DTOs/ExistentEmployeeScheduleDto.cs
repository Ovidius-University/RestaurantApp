using WebAppUI.Models.Entities;
using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;

public class ExistentEmployeeScheduleDto
{
    [Display(Name = "Employee")]
    public required int EmployeeId { get; set; }
    public string Employee { get; set; } = string.Empty;
    [Display(Name = "Day")]
    public required int DayId { get; set; }
    public string Day { get; set; } = string.Empty;
    [Required, Display(Name = "Start Hour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan StartHour { get; set; }
    [Required, Display(Name = "End Hour"), BiggerHour("StartHour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan EndHour { get; set; }
    public void ToEntity(ref EmployeeSchedule ExistentEmployeeSchedule)
    {
        if (ExistentEmployeeSchedule.EmployeeId == EmployeeId && ExistentEmployeeSchedule.DayId == DayId)
        {
            ExistentEmployeeSchedule.StartHour = StartHour;
            ExistentEmployeeSchedule.EndHour = EndHour;
        }
    }
}
