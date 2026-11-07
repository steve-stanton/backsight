using System;
using System.ComponentModel.DataAnnotations;

namespace Backsight.Map.Editor.Windows;

public class NumericFormatAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // TODO: This was meant to highlight errors while typing into a NumericUpDown control,
        // but it seems the value that gets passed in here is already a decimal (even if junk
        // chars have been entered). This person tried to get around this, but it's a bit laborious:
        // https://stackoverflow.com/questions/79399611/how-to-only-allow-numbers-while-typing-in-a-numeric-updown
        
        if (value is null)
            return ValidationResult.Success;

        if (Decimal.TryParse(value.ToString(), out decimal v))
            return ValidationResult.Success;

        return new ValidationResult("Value must be numeric");
    }
}