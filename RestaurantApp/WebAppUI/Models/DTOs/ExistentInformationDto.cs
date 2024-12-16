using WebAppUI.Models.Entities;
using WebAppUI.Validators;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.DTOs;

public class ExistentInformationDto
{
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Description { get; set; } = string.Empty;
    [Required, MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    [Required, MinLength(10), MaxLength(10), Display(Name = "Phone number"), Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required, MaxLength(250), EmailAddress]
    public string Email { get; set; } = string.Empty;

    public void ToEntity(ref Information ExistentInformation)
    {
     ExistentInformation.Name = Name;
     ExistentInformation.Description = Description;
     ExistentInformation.Location = Location;
     ExistentInformation.PhoneNumber = PhoneNumber;
     ExistentInformation.Email = Email;
    }
}
