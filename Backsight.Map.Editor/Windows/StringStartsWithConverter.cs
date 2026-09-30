using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Evaluates whether a string starts with a specified prefix and yields
/// a customizable boolean outcome.
/// </summary>
public class StringStartsWithConverter : IValueConverter
{
    public static readonly StringStartsWithConverter TrueWhenStarts = new() { ReturnOnMatch = true };
    public static readonly StringStartsWithConverter FalseWhenStarts = new() { ReturnOnMatch = false };

    /// <summary>
    /// When true (default), returns true if the string starts with the prefix.
    /// When false, returns false if the string starts with the prefix (inverts the result).
    /// </summary>
    public bool ReturnOnMatch { get; set; } = true;

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool startsWith = false;

        if (value is string text && parameter != null)
        {
            var prefix = parameter.ToString();
            if (!string.IsNullOrEmpty(prefix))
            {
                startsWith = text.TrimStart().StartsWith(prefix, StringComparison.Ordinal);
            }
        }

        return ReturnOnMatch ? startsWith : !startsWith;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}