using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class WeightAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is decimal weight)
        {
            if (weight <= 0m)
            {
                return new ValidationResult("Weight can not be zero or negative!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Weight is not valid!");
    }
}