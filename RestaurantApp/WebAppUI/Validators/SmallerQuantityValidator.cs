using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class SmallerQuantityAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public SmallerQuantityAttribute(string otherProperty)
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

        if (value != null && otherValue != null && (int)value > (int)otherValue)
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} = {value} needs to be smaller than or equal to {_otherProperty} = {otherValue}.");
        }

        return ValidationResult.Success!;
    }
}