using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardReviewerVm
{
    [Key]
    public int ReviewerId { get; set; }
    public string Name { get; set; } = string.Empty;
    // public CardReviewerVm(Reviewer Reviewer)
    // {
    //     ReviewerId = Reviewer.Id;
    //     Name = Reviewer.Name;
    // }
}