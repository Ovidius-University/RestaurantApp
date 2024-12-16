namespace WebAppUI.Areas.Admin.Models.ViewModels;

public class IndexEmpWorkVm
{
    public int WorkerId { get; set; }
    public string Email { get; set; } = string.Empty;
    public int Orders { get; set; }
    public List<OrderVm>? ListOrders { get; set; }
}
