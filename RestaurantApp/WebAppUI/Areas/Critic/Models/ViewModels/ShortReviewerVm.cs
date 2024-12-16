using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Areas.Critic.Models.ViewModels;
public class ShortReviewerVm
{
    public int ReviewerId { get; set; }
    [Display(Name = "Reviewer")]
    public string Name { get; set; }=string.Empty;
    //public ShortReviewerVm(int id, string name)
    //{
    //    Id = id;
    //    FullName = $"{name}" ;
    //}
}
