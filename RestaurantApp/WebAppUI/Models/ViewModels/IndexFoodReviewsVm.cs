namespace WebAppUI.Models.ViewModels;

public class IndexFoodReviewsVm
{
    public string Food { get; set; } = string.Empty;
    public List<CardReviewVm>? ListReviews { get; set; }
}
