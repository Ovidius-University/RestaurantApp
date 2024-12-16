using WebAppUI.Models.Entities;
using WebAppUI.Validators;
using Microsoft.CodeAnalysis;
using System.ComponentModel.DataAnnotations;

namespace WebAppUI.Models.DTOs;

public class ExistentPrivacyDto
{
    public int Id { get; set; }
    [Required]
    public string Policy { get; set; } = string.Empty;
    public void ToEntity(ref Privacy ExistentPrivacy)
    {
        ExistentPrivacy.Policy = Policy;
    }
}
