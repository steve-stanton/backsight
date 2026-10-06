using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace Backsight.Map.Editor.Windows;

public class AllTrueConverter : IMultiValueConverter
{
    public static readonly AllTrueConverter Instance = new();

    public object? Convert(
        IList<object?> values,
        Type targetType,
        object? parameter,
        CultureInfo culture)
    {
        return values.All(v => v is true);
    }
}