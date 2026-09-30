using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Backsight.Map.Editor.Windows;

public class FeatureIdConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        //Console.WriteLine(value?.GetType().Name ?? "null");
        if (value is Model.Feature f)
            return $"+{f.FormattedKey}";

        return String.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return BindingOperations.DoNothing;
    }
}