using WebAppUI.Areas.Admin.Models.DTOs;

namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class ReviewerCriticsVm
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<ExistentUserDto>? ListCritics { get; set; }
}