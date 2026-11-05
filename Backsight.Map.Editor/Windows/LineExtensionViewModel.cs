using System.Linq;
using Backsight.Environment;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Operations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

internal partial class LineExtensionViewModel : ToolViewModel<LineExtensionTool>
{
    /// <summary>
    /// The line that is being extended.
    /// </summary>
    private readonly LineFeature _extendLine;

    /// <summary>
    /// True if extending from the end of the line. False from the start.
    /// </summary>
    [ObservableProperty]
    private bool _isExtendFromEnd = true;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private decimal? _length;

    /// <summary>
    /// True if a line should be added too.
    /// </summary>
    [ObservableProperty]
    private bool _wantLine = true;

    [ObservableProperty]
    private IEntity _selectedPointType;
    
    [ObservableProperty]
    private DisplayId[] _pointIds;
    
    [ObservableProperty]
    private DisplayId? _selectedPointId;
    
    /// <summary>
    /// The units to display for entry of the extension length.
    /// </summary>
    internal string Units { get; }
    
    /// <summary>
    /// The entity types that can be used for extension points.
    /// </summary>
    internal IEntity[] PointTypes { get; }
    
    /// <summary>
    /// True if the user can select an ID for the extension point.
    /// </summary>
    internal bool AllowIdSelection { get; }

    internal LineExtensionViewModel(
        LineExtensionTool tool,
        LineFeature extendLine,
        LineExtensionOperation? recall) : base(tool)
    {
        _extendLine = extendLine;

        // Get the units hint for the extension length
        var entryUnit = DistanceUnit.GetUnit(tool.Store.Settings.EntryUnit);
        Units = entryUnit.Abbreviation + " ";

        // Fill the combo with the entity types for the extension point
        PointTypes = tool.ViewModel.Environment.EntityTypes
            .Where(x => x.IsPointValid && x.Id != 0)
            .OrderBy(x => x.Name)
            .ToArray();

        _selectedPointType = recall?.NewPoint.EntityType ?? tool.Store.DefaultPointType;

        // Load the ID combo and select the first available ID
        _pointIds = tool.Store.GetPointIds(_selectedPointType);
        if (_pointIds.Length > 0)
            _selectedPointId = _pointIds[0];
        
        // If we are auto-numbering, disable the combo.
        AllowIdSelection = !tool.Store.Settings.AutoNumber;
        
        if (recall is not null)
        {
            Length = (decimal)recall.Length.GetDistance(entryUnit, true);
            IsExtendFromEnd = recall.IsExtendFromEnd;
            WantLine = recall.NewLine is not null;
        }
    }
    
    internal LineFeature Line => _extendLine;
    
    partial void OnLengthChanged(decimal? value)
    {
        Tool.RefreshMapDisplay();
    }
    
    [RelayCommand]
    private void OtherEnd()
    {
        IsExtendFromEnd = !IsExtendFromEnd;
    }

    partial void OnIsExtendFromEndChanged(bool value)
    {
        Tool.RefreshMapDisplay();
    }

    partial void OnWantLineChanged(bool value)
    {
        Tool.RefreshMapDisplay();
    }

    partial void OnSelectedPointTypeChanged(IEntity? value)
    {
        if (value is null)
        {
            SelectedPointId = null;
            PointIds = [];
        }
        else if (value.IdGroup.Id != _selectedPointId?.Group.Id)
        {
            PointIds = Tool.Store.GetPointIds(value);

            if (PointIds.Length == 0)
                SelectedPointId = null;
            else
                SelectedPointId = _pointIds[0];
        }
    }
    
    protected override bool CanExecuteOk()
    {
        return Length > 0;
    }
}