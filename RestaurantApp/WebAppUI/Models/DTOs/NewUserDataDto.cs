using System.ComponentModel.DataAnnotations;
using WebAppUI.Validators;
namespace WebAppUI.Models.DTOs;

public class NewUserDataDto
{
    public required int UserId { get; set; }
    [Display(Name = "Choose a gender:")]
    public required int GenderId { get; set; }
    [Birthday, DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
    public required DateTime Birthday { get; set; }
}
