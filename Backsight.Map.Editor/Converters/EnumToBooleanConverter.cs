using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Backsight.Map.Editor.Converters;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null && parameter != null && value.Equals(parameter);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return (value is true && parameter != null) ? parameter : BindingOperations.DoNothing;
    }
}