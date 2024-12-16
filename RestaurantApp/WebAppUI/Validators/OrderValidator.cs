using System.ComponentModel.DataAnnotations;
namespace WebAppUI.Validators;
public class OrderAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public OrderAttribute(string otherProperty)
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

        if (otherValue is null)
        {
            throw new ArgumentException($"Property {_otherProperty} is null.");
        }

        if (value is DateTime order)
        {
            if (order < DateTime.Now)
            {
                return new ValidationResult("Arrival time needs to be in the future!");
            }
            if ((order - DateTime.Now).TotalDays > 366)
            {
                return new ValidationResult("Arrival time needs to be within the timespan of one year!");
            }
            /*
            if ((order - DateTime.Now).TotalHours < 1 && (bool)otherValue == true)
            {
                return new ValidationResult("Arrival time for a delivery needs to be at least an hour in the future!");
            }
            if ((order - DateTime.Now).TotalMinutes < 30 && (bool)otherValue == false)
            {
                return new ValidationResult("Arrival time for a pick-up needs to be at least 30 minutes in the future!");
            }
            */
            return ValidationResult.Success!;
        }
        return new ValidationResult("Arrival time is not valid!");
    }
}