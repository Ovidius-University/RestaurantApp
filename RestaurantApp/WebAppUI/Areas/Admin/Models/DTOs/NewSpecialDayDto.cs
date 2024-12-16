using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Areas.Admin.Models.DTOs;
public class NewSpecialDayDto
{
    [Required]
    public int Day { get; set; }
    [Required, SpecialDay("Day")]
    public int Month { get; set; }
    [Required, Display(Name = "Start Hour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan StartHour { get; set; }
    [Required, Display(Name = "End Hour"), BiggerHour("StartHour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan EndHour { get; set; }
    [Display(Name = "Is it a work day?")]
    public bool IsWorkDay { get; set; }
}
