using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class PriceAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is decimal price)
        {
            if (price <= 0m)
            {
                return new ValidationResult("Price can not be zero or negative!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Price is not valid!");
    }
}