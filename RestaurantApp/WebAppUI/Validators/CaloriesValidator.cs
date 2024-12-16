using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class CaloriesAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int calories)
        {
            if (calories <= 0)
            {
                return new ValidationResult("Calories can not be zero or negative!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Calories are not valid!");
    }
}