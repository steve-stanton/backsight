using System;
using System.Text;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

internal partial class PathAngleViewModel : DialogViewModel
{
    [NotifyDataErrorInfo]
    [AngleFormat]
    [ObservableProperty]
    private string? _angleText;

    [ObservableProperty]
    private bool _isClockwise = true;

    [ObservableProperty]
    private bool _isDeflection = false;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string _entryString = "";

    protected override bool CanExecuteOk()
    {
        return !String.IsNullOrWhiteSpace(EntryString);
    }
    
    partial void OnAngleTextChanged(string? value)
    {
        if (value is not null)
        {
            // Text that starts with a "-" character is a shortcut for a counter-clockwise angle.
            // If that's the case, ensure the clockwise flag is false.
            if (value.StartsWith("-") && IsClockwise)
                IsClockwise = false;
        }

        RefreshPreview();
    }
    
    partial void OnIsClockwiseChanged(bool value)
    {
        RefreshPreview();
    }

    partial void OnIsDeflectionChanged(bool value)
    {
        RefreshPreview();
    }

    private void RefreshPreview()
    {
        EntryString = GetEntryString();
    }

    private string GetEntryString()
    {
        if (AngleText is null)
            return String.Empty;

        if (!RadianValue.TryParse(AngleText, out var radians))
            return String.Empty;
        
        var sb = new StringBuilder();

        if (!IsClockwise)
            sb.Append('-');
        
        sb.Append(RadianValue.AsShortString(Math.Abs(radians)));

        if (IsDeflection)
            sb.Append('d');
        
        return sb.ToString();
    }
}