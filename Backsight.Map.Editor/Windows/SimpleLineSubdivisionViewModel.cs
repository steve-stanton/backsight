using System;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Observations;
using Backsight.Model.Operations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

internal partial class SimpleLineSubdivisionViewModel : ToolViewModel<SimpleLineSubdivisionTool>
{
    internal LineFeature Line { get; }
    
    /// <summary>
    /// The units to display for the offset length.
    /// </summary>
    internal string Units { get; }

    /// <summary>
    /// The distance (on the ground) to the split point.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private decimal? _distance;
    
    /// <summary>
    /// Is the entered distance longer than the length of the line?
    /// </summary>
    [ObservableProperty] bool _tooLong;

    /// <summary>
    /// The validation message to display if the entered distance is longer than the length of the line.
    /// </summary>
    internal string TooLongMessage { get; }

    /// <summary>
    /// Is the entered distance from the end of the line?
    /// </summary>
    internal bool FromEnd { get; private set; }
    
    /// <summary>
    /// The calculated position of the split point (null if the distance to the
    /// split point has not been entered, or it is longer than the length of the line).
    /// </summary>
    internal IPosition? SplitPosition { get; private set; }
    
    internal SimpleLineSubdivisionViewModel(SimpleLineSubdivisionTool tool, LineFeature line)
        : base(tool)
    {
        Line = line;
        
        // Get the units hint for the split distance
        var entryUnit = DistanceUnit.GetUnit(tool.Store.Settings.EntryUnit);
        Units = entryUnit.Abbreviation + " ";
        
        // Determine validation message that takes the entry units into account
        var maxLength = line.GroundLength.Meters;
        var entryLength = entryUnit.FromMetric(maxLength);
        var maxDist = new Distance(entryLength, entryUnit).ToString();
        TooLongMessage = $"Too long. The maximum distance is {maxDist}";
    }

    [RelayCommand]
    private void OtherWay()
    {
        FromEnd = !FromEnd;
        CalculateSplitPosition();
        Tool.RefreshMapDisplay();
    }

    partial void OnDistanceChanged(decimal? _)
    {
        CalculateSplitPosition();
        Tool.RefreshMapDisplay();
    }

    private void CalculateSplitPosition()
    {
        var d = GetDistance();
        
        if (d is null)
        {
            TooLong = false;
            SplitPosition = null;
        }
        else
        {
            SplitPosition = SimpleLineSubdivisionOperation.Calculate(Line, d, FromEnd);
            TooLong = SplitPosition is null;
        }
    }

    /// <summary>
    /// Gets the distance (if any) to the split point.
    /// </summary>
    /// <returns>The entered distance (on the ground).</returns>
    internal Distance? GetDistance()
    {
        if (Distance is null)
            return null;
        
        var len = Decimal.ToDouble(Distance.Value);
        var entryUnit = DistanceUnit.GetUnit(Tool.Store.Settings.EntryUnit);
        return new Distance(len, entryUnit);
    }
    
    protected override bool CanExecuteOk()
    {
        return SplitPosition is not null;
    }
}
