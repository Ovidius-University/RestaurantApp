using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class SmallerPriceAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public SmallerPriceAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
    }

    protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
    {
        var anotherProperty = validationContext.ObjectType.GetProperty(_otherProperty);

        if (anotherProperty is null)
        {
            throw new ArgumentException($"Property {_otherProperty} was not found.");
        }

        var otherValue = anotherProperty.GetValue(validationContext.ObjectInstance);

        if (value != null && otherValue != null && (decimal)value >= (decimal)otherValue)
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} needs to be smaller than {_otherProperty}.");
        }

        return ValidationResult.Success!;
    }
}