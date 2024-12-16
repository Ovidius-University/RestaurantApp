using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardProviderVm
{
    [Key]
    public int ProviderId { get; set; }
    public string Name { get; set; } = string.Empty;
    // public CardProviderVm(Provider Provider)
    // {
    //     ProviderId = Provider.Id;
    //     Name = Provider.Name;
    // }
}