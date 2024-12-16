namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class IndexFoodReviewerVm
{
    public string Name { get; set; } = string.Empty;
    public List<ReviewVm>? ListReviews { get; set; }
}
