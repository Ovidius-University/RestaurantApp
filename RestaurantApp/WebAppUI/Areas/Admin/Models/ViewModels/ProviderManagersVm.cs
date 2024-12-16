using WebAppUI.Areas.Admin.Models.DTOs;

namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class ProviderManagersVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ExistentUserDto>? ListManagers { get; set; }
}

