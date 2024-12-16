using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class BirthdayAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is DateTime birthday)
        {
            if (birthday >= DateTime.Now)
            {
                return new ValidationResult("Birthday can not be from the future");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Birthday is not valid");
    }
}