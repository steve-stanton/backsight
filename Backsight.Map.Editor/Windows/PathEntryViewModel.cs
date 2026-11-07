using System;
using System.Threading.Tasks;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class PathEntryViewModel : PageModel
{
    private readonly PathViewModel _parentModel;
    private DistanceUnit _entryUnit;
    
    [ObservableProperty]
    private string? _text;

    internal PathEntryViewModel(PathViewModel parentModel)
    {
        _parentModel = parentModel;
        
        var unitType = parentModel.Tool.ViewModel.Settings?.EntryUnit ?? DistanceUnitType.Meters;
        _entryUnit = DistanceUnit.GetUnit(unitType);

        Text = _entryUnit.Abbreviation + "... ";
    }

    partial void OnTextChanged(string? value)
    {
        _parentModel.FinishCommand.NotifyCanExecuteChanged();
    }

    internal override bool ValidatePage() => !String.IsNullOrWhiteSpace(Text);

    [RelayCommand]
    private async Task Distance()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new PathDistanceWindow(_entryUnit);
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
        {
            if (dialog.ViewModel.MakeUnitsNewDefault)
                _entryUnit = DistanceUnit.GetUnit(dialog.ViewModel.EntryUnit);
            
            Text += $" {dialog.ViewModel.EntryString}";
        }
    }

    [RelayCommand]
    private async Task Angle()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new PathAngleWindow();
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
        {
            Text += $" {dialog.ViewModel.EntryString}";
        }
    }

    [RelayCommand]
    private async Task Curve()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new PathArcWindow(_entryUnit);
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
        {
            Text += $" {dialog.ViewModel.EntryString}";
        }
    }

    [RelayCommand]
    private void CuldeSac()
    {
        
    }

    [RelayCommand]
    private void EndCurve()
    {
        Text += ")";
    }
}