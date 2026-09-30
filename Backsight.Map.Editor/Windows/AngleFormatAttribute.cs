using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

public class AngleFormatAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;
        
        string text = value.ToString()?.Trim() ?? string.Empty;
        if (text.Length == 0)
            return ValidationResult.Success;
        
        // No validation if selecting parallel points and nothing has been specified
        if (text.StartsWith("..."))
            return ValidationResult.Success;
        
        // No validation if selecting parallel points (assumes convention of using a + character as
        // a prefix for point IDs)
        if (text.StartsWith("+"))
            return ValidationResult.Success;

        // If the entered angle contains a "d" (anywhere), treat it as a deflection (and strip it out).
        int dindex = text.IndexOf('D', StringComparison.InvariantCultureIgnoreCase);
        if (dindex >= 0)
            text = text.Remove(dindex, 1);
        
        if (text.Length > 0 && !RadianValue.TryParse(text, out var result))
            return new ValidationResult("Unexpected format for angle");
        
        return ValidationResult.Success;
    }
}