using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class QuantityAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int quantity)
        {
            if (quantity <= 0)
            {
                return new ValidationResult("Quantity can not be zero or negative!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Quantity is not valid!");
    }
}