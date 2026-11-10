using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Threading;
using Backsight.Model;
using Backsight.Model.Operations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class PathEntryViewModel : PageModel
{
    private readonly PathViewModel _parentModel;
    private readonly DispatcherTimer _previewTimer;
    
    /// <summary>
    /// The initial data entry units. 
    /// </summary>
    private readonly DistanceUnit _initialEntryUnit;

    /// <summary>
    /// The most recent data entry units.
    /// </summary>
    private DistanceUnit _entryUnit;
    
    /// <summary>
    /// The text to display for the most recent data entry units.
    /// </summary>
    [ObservableProperty]
    private string _unitsDefault;
    
    [ObservableProperty]
    private string? _text;
    
    [ObservableProperty]
    private string? _validationError;

    [ObservableProperty]
    private string? _precisionText;

    internal PathEntryViewModel(PathViewModel parentModel, PathOperation? recall)
    {
        _parentModel = parentModel;
        _previewTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1500)
        };
        _previewTimer.Tick += (_, _) =>
        {
            _previewTimer.Stop();
            ShowPreview();
        };

        var unitType = parentModel.Tool.ViewModel.Settings?.EntryUnit ?? DistanceUnitType.Meters;
        _entryUnit = DistanceUnit.GetUnit(unitType);
        _initialEntryUnit = _entryUnit;
        UnitsDefault = _entryUnit.Abbreviation + "...";

        if (recall is not null)
            Text = recall.EntryString;
    }

    partial void OnTextChanged(string? value)
    {
        _parentModel.FinishCommand.NotifyCanExecuteChanged();
        
        ValidationError = null;

        // Call ShowPreview if the preview timer runs to completion
        _previewTimer.Stop();
        _previewTimer.Start();
    }

    private void ShowPreview()
    {
        // Both of these should normally have been defined prior to display of this wizard page
        // (but might not be when recalling an old edit)
        if (_parentModel.StartPoint is null || _parentModel.EndPoint is null)
            return;
        
        var text = Text?.ReplaceLineEndings(" ").Trim() ?? String.Empty;

        try
        {
            // Supply the initial entry units in case the user has removed it from the entered text
            // (don't use the most recent data entry units from the very start)
            var items = PathParser.GetPathItems(text, _initialEntryUnit);

            if (items.Length > 0)
            {
                // Ensure that the most recent data entry units are displayed
                var lastUnits = items.LastOrDefault(x => x.ItemType == PathItemType.Units);
                if (lastUnits is not null && lastUnits.Units != _entryUnit)
                {
                    _entryUnit = lastUnits.Units;
                    UnitsDefault = _entryUnit.Abbreviation + "...";
                }
                
                Leg[] legs = PathParser.CreateLegs(items);
                var pd = new PathInfo(_parentModel.StartPoint, _parentModel.EndPoint, legs);

                double prec = pd.Precision;
                if (Math.Abs(prec) < MathConstants.TINY)
                    PrecisionText = "Precision: Exact fit";
                else
                    PrecisionText = $"Precision: 1:{(uint)prec}";

                // Save the path with the tool (so that it can be rendered)
                _parentModel.Tool.SetCurrentPath(pd);
            }

            ValidationError = null;
        }
        catch (Exception e)
        {
            ValidationError = e.Message;
        }
    }

    internal override bool ValidatePage() => !String.IsNullOrWhiteSpace(Text) && ValidationError is null;

    [RelayCommand]
    private async Task Distance()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new PathDistanceWindow(_entryUnit);
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
            Text += $" {dialog.ViewModel.EntryString}";
    }

    [RelayCommand]
    private async Task Angle()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new PathAngleWindow();
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
            Text += $" {dialog.ViewModel.EntryString}";
    }

    [RelayCommand]
    private async Task Curve()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new PathArcWindow(_entryUnit);
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
            Text += $" {dialog.ViewModel.EntryString}";
    }

    [RelayCommand]
    private async Task CuldeSac()
    {
        var vm = _parentModel.Tool.ViewModel;
        var dialog = new CuldeSacWindow(_entryUnit);
        var result = await vm.ShowDialog(dialog);
        if (result == DialogResult.OK)
            Text += $" {dialog.ViewModel.EntryString}";
    }

    [RelayCommand]
    private void EndCurve()
    {
        Text += ")";
    }
}