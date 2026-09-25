using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Converter to return a string representing an integer value +1.
/// </summary>
public class AddOneConverter : IValueConverter
{
    public static readonly AddOneConverter Instance = new();

    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int i)
            return (i + 1).ToString();
        return value?.ToString() ?? string.Empty;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        throw new NotSupportedException();
}