namespace WebAppUI.Areas.Critic.Models.ViewModels;

public class IndexReviewsVm
{
    public required string Reviewer { get; set; }
    public List<ReviewVm>? ListReviews { get; set; }
}
