namespace WebAppUI.Models.ViewModels;
public class ReviewerReviewsVm
{
    public CardReviewerVm? ReviewerDetails { get; set; }
    public List<CardReviewVm>? Reviews { get; set; }
}