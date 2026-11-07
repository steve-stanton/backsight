using System;
using Backsight.Model;
using Backsight.Model.Observations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

internal partial class PathDistanceViewModel : DialogViewModel
{
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private decimal? _distance;

    [ObservableProperty]
    private string _entryString = "";
    
    [ObservableProperty]
    private bool _createLine = true;
    
    [ObservableProperty]
    private bool _createPoint = true;

    [ObservableProperty]
    private bool _allowMakeNewDefault = false;

    [ObservableProperty]
    private bool _makeUnitsNewDefault = false;
        
    [ObservableProperty]
    private DistanceUnitType _entryUnit;

    [ObservableProperty]
    private int _repeatCount = 1;
    
    private DistanceUnitType _defaultUnit;

    internal PathDistanceViewModel(DistanceUnit defaultUnit)
    {
        _defaultUnit = defaultUnit.UnitType;
        EntryUnit = _defaultUnit;
    }
    
    partial void OnDistanceChanged(decimal? value)
    {
        RefreshPreview();
    }

    partial void OnEntryUnitChanged(DistanceUnitType value)
    {
        AllowMakeNewDefault = _defaultUnit != value;
        
        if (!AllowMakeNewDefault)
            MakeUnitsNewDefault = false;
        
        RefreshPreview();
    }
    
    partial void OnMakeUnitsNewDefaultChanged(bool value)
    {
        RefreshPreview();
    }

    partial void OnCreateLineChanged(bool value)
    {
        // Cover case where user said they don't want a line or a point, then decide they
        // want a line again (in that case, you MUST create a point)
        if (value == true)
            CreatePoint = true;
        
        RefreshPreview();
    }

    partial void OnCreatePointChanged(bool value)
    {
        RefreshPreview();
    } 
    
    [RelayCommand]
    private void Repeat(string number)
    {
        RepeatCount = Int32.Parse(number);
    }

    partial void OnRepeatCountChanged(int value)
    {
        RefreshPreview();
    }

    protected override bool CanExecuteOk()
    {
        return Distance is not null;
    }

    private void RefreshPreview()
    {
        EntryString = GetEntryString();
    }
    
    private string GetEntryString()
    {
        if (Distance is null)
            return String.Empty;
        
        var units = DistanceUnit.GetUnit(EntryUnit);
        var prefix = MakeUnitsNewDefault ? $"{units.Abbreviation}... " : "";

        var value = (double)Distance;
        var d = new Distance(value, units);
        var showUnit = EntryUnit != _defaultUnit && !MakeUnitsNewDefault;
        var text = d.Format(showUnit);
        
        var suffix = RepeatCount > 1 ? $"*{RepeatCount}" : "";

        if (!CreatePoint)
            suffix += "/*";
        else if (!CreateLine)
            suffix += "/-";

        return prefix + text + suffix;
    }
}