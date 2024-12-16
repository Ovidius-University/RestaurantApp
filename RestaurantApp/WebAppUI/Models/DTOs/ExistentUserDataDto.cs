using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebAppUI.Validators;
using WebAppUI.Models.ViewModels;
namespace WebAppUI.Models.ViewModels;
public class ExistentUserDataDto
{
    public int UserId { get; set; }
    public string User { get; set; } = string.Empty;
    [Display(Name = "Gender")]
    public int GenderId { get; set; }
    public string Gender { get; set; } = string.Empty;
    [Birthday, DataType(DataType.Date), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
    public DateTime Birthday { get; set; }
    public void ToEntity(ref UserData ExistentUserData)
    {
        if (ExistentUserData.UserId == UserId)
        {
            ExistentUserData.GenderId = GenderId;
            ExistentUserData.Birthday = Birthday;
        }
    }
}
