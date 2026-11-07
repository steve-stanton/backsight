using System;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

internal partial class CuldeSacViewModel : DialogViewModel
{
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [NumericFormat]
    private string? _radius;
    
    [ObservableProperty]
    [NotifyDataErrorInfo]
    [AngleFormat(allowNegative: false)]
    private string? _centralAngle;
    
    [ObservableProperty]
    private bool _isClockwise = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private string _entryString = "";
    
    private DistanceUnit EntryUnit { get; }
    
    /// <summary>
    /// The units to display for entry of the radius.
    /// </summary>
    internal string RadiusHeading { get; }

    
    internal CuldeSacViewModel(DistanceUnit entryUnit)
    {
        EntryUnit = entryUnit;
        RadiusHeading = $"Radius ({entryUnit.Abbreviation})";
    }
    
    partial void OnRadiusChanged(string value)
    {
        RefreshPreview();
    }
    
    partial void OnCentralAngleChanged(string value)
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

    private string GetEntryString()
    {
        if (String.IsNullOrWhiteSpace(Radius)) 
            return String.Empty;

        var angleText = "90-0";
        if (!String.IsNullOrWhiteSpace(CentralAngle))
        {
            if (!RadianValue.TryParse(CentralAngle, out var angle))
                return String.Empty;
            
            angleText = RadianValue.AsShortString(angle);
        }
        
        var result = $"({angleText}ca {Radius}";

        if (IsClockwise)
            return result + "/ ";
        else
            return result + " cc/ ";
    }
}