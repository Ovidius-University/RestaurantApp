using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class BiggerHourAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public BiggerHourAttribute(string otherProperty)
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

        if (value is null)
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} needs to be filled.");
        }

        var otherValue = anotherProperty.GetValue(validationContext.ObjectInstance);

        if (otherValue is null)
        {
            return new ValidationResult(ErrorMessage ?? $"{_otherProperty} needs to be filled.");
        }

        var val = (TimeSpan)value;

        var otval = (TimeSpan)otherValue;

        if (value != null && otherValue != null && val <= otval)
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} needs to be after Start Hour.");
        }

        return ValidationResult.Success!;
    }
}