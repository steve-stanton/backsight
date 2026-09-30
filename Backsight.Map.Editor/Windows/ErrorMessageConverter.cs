using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Backsight.Map.Editor.Windows;

public class ErrorMessageConverter : IValueConverter
{
    public static readonly ErrorMessageConverter Instance = new();

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is null)
            return null;

        if (value is IEnumerable rawErrors and not string)
        {
            var messages = new List<string>();
            foreach (var item in rawErrors)
            {
                if (item is ValidationResult vr && !string.IsNullOrWhiteSpace(vr.ErrorMessage))
                    messages.Add(vr.ErrorMessage);
                else if (item is Exception ex)
                    messages.Add(ex.Message);
                else if (item != null)
                    messages.Add(item.ToString() ?? "");
            }

            return messages.Count > 0 ? string.Join(System.Environment.NewLine, messages) : null;
        }

        return value.ToString();
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}