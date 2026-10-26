using System;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

internal partial class PathDistanceViewModel : DialogViewModel
{
    [ObservableProperty]
    private decimal? _distance;
    
    [ObservableProperty]
    private bool _createLine = true;
    
    [ObservableProperty]
    private bool _createPoint = true;

    [ObservableProperty]
    private bool _makeUnitsNewDefault = false;
    
    [ObservableProperty]
    private DistanceUnitType _entryUnit;

    internal int RepeatCount { get; private set; } = 1;

    [RelayCommand]
    private void Repeat(string number)
    {
        RepeatCount = Int32.Parse(number);
    }

    protected override bool CanExecuteOk()
    {
        return Distance is not null;
    }
}