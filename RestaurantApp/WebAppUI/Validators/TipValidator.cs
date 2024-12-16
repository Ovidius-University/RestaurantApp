using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class TipAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is decimal tip)
        {
            if (tip < 0m)
            {
                return new ValidationResult("Tip can not be negative!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Tip is not valid!");
    }
}