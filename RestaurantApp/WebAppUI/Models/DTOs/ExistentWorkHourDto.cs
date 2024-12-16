using WebAppUI.Models.Entities;
using WebAppUI.Validators;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.DTOs;

public class ExistentWorkHourDto
{
    public int Id { get; set; }
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required, Display(Name = "Start Hour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan StartHour { get; set; }
    [Required, Display(Name = "End Hour"), BiggerHour("StartHour"), DataType(DataType.Time), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:hh\\:mm}")]
    public TimeSpan EndHour { get; set; }
    [Display(Name = "Is it a work day?")]
    public bool IsWorkDay { get; set; }
    public void ToEntity(ref WorkHour ExistentWorkHour)
    {
        if (ExistentWorkHour.Id == Id)
        {
            ExistentWorkHour.StartHour = StartHour;
            ExistentWorkHour.EndHour = EndHour;
            ExistentWorkHour.IsWorkDay = IsWorkDay;
        }
    }
}
