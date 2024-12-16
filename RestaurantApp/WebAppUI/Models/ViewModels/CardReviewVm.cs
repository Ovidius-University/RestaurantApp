namespace WebAppUI.Models.ViewModels;
public class CardReviewVm
{
    public int FoodId { get; set; }
    public int ReviewerId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string Food { get; set; } = string.Empty;
    public string Reviewer { get; set; } = string.Empty;
    // public CardReviewVm()
    // {
    // }
    // public CardReviewVm(Review Review)
    // {
    //     if(Review is not null)
    //     {
    //         Id=Review.FoodId;
    //         Content=Review.Title;
    //         Reviewer=string.Join(", ",Review.Reviewer!.Select(a=>$"{a.Reviewer!.Name}"));
    //     }
    // }
}