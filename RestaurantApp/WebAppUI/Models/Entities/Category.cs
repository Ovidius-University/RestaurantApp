using System.ComponentModel.DataAnnotations.Schema;

namespace WebAppUI.Models.Entities
{
    [Table("Category")]
    public class Category
    {
        [Column("CategoryId")]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Food>? Foods { get; set; }
    }
}
