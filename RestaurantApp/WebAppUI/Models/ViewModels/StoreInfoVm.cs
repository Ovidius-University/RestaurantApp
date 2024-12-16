using WebAppUI.Models.DTOs;
namespace WebAppUI.Models.ViewModels;

public class StoreInfoVm
{
    public ExistentInformationDto? Information { get; set; }
    public List<ExistentWorkHourDto>? WorkHours { get; set; }
}
