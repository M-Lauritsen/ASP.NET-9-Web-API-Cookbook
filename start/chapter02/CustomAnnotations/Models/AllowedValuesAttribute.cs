using System.ComponentModel.DataAnnotations;

namespace CustomAnnotations.Models;

public class AllowedValuesAttribute(params string[] allowedValues) : ValidationAttribute
{
    private readonly List<string> _allowedValues = allowedValues?.ToList() ?? [];

    protected override ValidationResult? IsValid(object value, ValidationContext validationContext)
    {
        if (value == null || !_allowedValues.Contains(value.ToString()!))
        {
            return new ValidationResult($"The field {validationContext.DisplayName} must be one of the following values: {string.Join(", ", _allowedValues)}.");
        }
        return ValidationResult.Success ?? null;
    }

    
}
