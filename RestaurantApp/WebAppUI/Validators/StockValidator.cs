using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class StockAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        if (value is int stock)
        {
            if (stock < 0)
            {
                return new ValidationResult("Stock can not be negative!");
            }
            return ValidationResult.Success!;
        }
        return new ValidationResult("Stock is not valid!");
    }
}