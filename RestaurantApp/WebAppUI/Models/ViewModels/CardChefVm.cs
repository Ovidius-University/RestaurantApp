using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.ViewModels;
public class CardChefVm
{
    [Key]
    public int ChefId { get; set; }
    public string Name { get; set; } = string.Empty;
    /*
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    [DisplayFormat(DataFormatString = "{0:yyyy}")]
    public DateTime Birthday { get; set; }
    */
    // public CardChefVm(Chef Chef)
    // {
    //     ChefId = Chef.Id;
    //     Name = Chef.Name;
    //     LastName = Chef.LastName;
    //     FirstName = Chef.FirstName;
    //     Birthday = Chef.Birthday;
    // }
}