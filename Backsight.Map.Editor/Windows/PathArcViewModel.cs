using System;
using System.Text;
using Backsight.Model;
using Backsight.Model.Observations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

internal partial class PathArcViewModel : DialogViewModel
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NumericFormat]
    private decimal? _radius;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [AngleFormat]
    private string _entryAngle = "";

    [ObservableProperty]
    [NotifyDataErrorInfo]
    [AngleFormat]
    private string _exitAngle = "";
    
    private DistanceUnit EntryUnit { get; }
    
    /// <summary>
    /// The units to display for entry of the radius.
    /// </summary>
    internal string UnitText { get; }
    
    [ObservableProperty]
    private bool _isClockwise = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string _entryString = "";

    internal PathArcViewModel(DistanceUnit entryUnit)
    {
        EntryUnit = entryUnit;
        UnitText = entryUnit.Abbreviation + " ";
    }
    
    partial void OnRadiusChanged(decimal? value)
    {
        RefreshPreview();
    }
    
    partial void OnEntryAngleChanged(string value)
    {
        RefreshPreview();
    }
    
    partial void OnExitAngleChanged(string value)
    {
        RefreshPreview();
    }
    
    partial void OnIsClockwiseChanged(bool value)
    {
        RefreshPreview();
    }

    protected override bool CanExecuteOk()
    {
        return !String.IsNullOrWhiteSpace(EntryString);
    }

    private void RefreshPreview()
    {
        EntryString = GetEntryString();
    }

    private string? GetAngle(string angleText, string? defaultResult)
    {
        if (String.IsNullOrWhiteSpace(angleText))
            return defaultResult;

        if (!RadianValue.TryParse(angleText, out var angle))
            return null;
        
        return RadianValue.AsShortString(angle);
    }
    
    private string GetEntryString()
    {
        if (Radius is null)
            return String.Empty;

        var entryText = GetAngle(EntryAngle, "90-0");
        var exitText = GetAngle(ExitAngle, entryText);

        if (entryText is null || exitText is null)
            return String.Empty;
        
        var result = $"({Radius} {entryText}";
        
        if (exitText != entryText)
            result += $" {exitText}";

        if (IsClockwise)
            return result + "/ ";
        else
            return result + " CC/ ";
    }
}