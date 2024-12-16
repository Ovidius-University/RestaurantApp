using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class SpecialDayAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public SpecialDayAttribute(string otherProperty)
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

        if ((int)value < 1 || (int)value > 12)
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} needs to be a proper month (1-12).");
        }

        var otherValue = anotherProperty.GetValue(validationContext.ObjectInstance);

        if (otherValue is null)
        {
            return new ValidationResult(ErrorMessage ?? $"{_otherProperty} needs to be filled.");
        }

        if ((int)otherValue < 1 || (int)otherValue > 31)
        {
            return new ValidationResult(ErrorMessage ?? $"{_otherProperty} needs to be a proper day (1-31).");
        }

        var dval = (int)otherValue;

        var mval = (int)value;

        if ((dval>29 && mval==2) || (dval > 30 && new[] {4,6,9,11}.Contains(mval)) || (dval > 31 && new[] {1,3,5,7,8,10,12}.Contains(mval)))
        {
            return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} and {_otherProperty} need to form a valid date.");
        }

        return ValidationResult.Success!;
    }
}