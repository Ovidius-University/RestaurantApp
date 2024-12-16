using System.ComponentModel.DataAnnotations.Schema;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace WebAppUI.Models.Entities;
[Table("FeedBack")]
public class FeedBack
{
    [Column("OrderId")]
    public int Id { get; set; }
    [ForeignKey(nameof(Id))]
    public Order? Order { get; set; }
    public string Comment { get; set; } = string.Empty;
}