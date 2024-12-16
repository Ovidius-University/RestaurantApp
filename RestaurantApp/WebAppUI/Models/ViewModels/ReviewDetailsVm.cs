namespace WebAppUI.Models.ViewModels;
public class ReviewDetailsVm
{
    public int FoodId { get; set; }
    public string Food { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int ReviewerId { get; set; }
    public string Reviewer { get; set; } = string.Empty;
}