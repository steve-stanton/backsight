using Avalonia.Input;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// View model for <see cref="NewCircleWindow"/>.
/// </summary>
/// <seealso cref="NewCircleTool"/>
internal partial class NewCircleViewModel : ToolViewModel<NewCircleTool>
{
    private enum PickTarget
    {
        None,
        Center,
        Radius,
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private PointFeature? _selectedCenter;
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private PointFeature? _selectedRadius;
    
    /// <summary>
    /// The data entry field that the user is filling in via the map.
    /// </summary>
    PickTarget _activePickTarget = PickTarget.None;

    /// <summary>
    /// The point that the mouse is hovering over while actively picking a point.
    /// Null if <see cref="_activePickTarget"/> is <see cref="PickTarget.None"/>,
    /// or the last mouse position does not overlap any point."/> 
    /// </summary>
    private PointFeature? _pickingPoint;

    internal NewCircleViewModel(NewCircleTool tool) : base(tool)
    {
        
    }
    
    protected override bool CanExecuteOk()
    {
        return SelectedCenter is not null && SelectedRadius is not null;
    }
    
    [RelayCommand]
    private void PickCenter()
    {
        if (SelectedCenter is not null)
        {
            SelectedCenter = null;
            Tool.RefreshMapDisplay();
        }

        _activePickTarget = PickTarget.Center;
        Tool.ViewModel.MapCursor = EditingCursors.PickCursor;
    }

    [RelayCommand]
    private void PickRadius()
    {
        if (SelectedRadius is not null)
        {
            SelectedRadius = null;
            Tool.RefreshMapDisplay();
        }

        _activePickTarget = PickTarget.Radius;
        Tool.ViewModel.MapCursor = EditingCursors.PickCursor;
    }

    internal PointFeature? PickingCenter => _activePickTarget == PickTarget.Center ? _pickingPoint : null;
    internal PointFeature? PickingRadius => _activePickTarget == PickTarget.Radius ? _pickingPoint : null;
    
    /// <summary>
    /// Attempts to pick a point from the map.
    /// </summary>
    /// <param name="p">The selected map position.</param>
    /// <returns>True if a point was selected.</returns>
    internal bool PickPoint(IPosition p)
    {
        if (_activePickTarget == PickTarget.None)
            return false;
        
        var pt = QueryPoint(p);
        
        if (_activePickTarget == PickTarget.Center)
            SelectedCenter = pt;
        else if (_activePickTarget == PickTarget.Radius)
            SelectedRadius = pt;

        StopPicking();
        return pt is not null;
    }

    private PointFeature? QueryPoint(IPosition p)
    {
        ILength size = new Length(Tool.Store.Settings.PointHeight * 0.5);
        return Tool.Store.Model.QueryClosest(p, size, SpatialType.Point) as PointFeature;
    }
    
    /// <summary>
    /// Ensures that a request to pick a point from the map has been stopped.
    /// </summary>
    private void StopPicking()
    {
        _activePickTarget = PickTarget.None;
        _pickingPoint = null;
        Tool.ViewModel.MapCursor = Cursor.Default;
    }

    internal void MouseMove(IPosition p)
    {
        if (_activePickTarget == PickTarget.None)
        {
            _pickingPoint = null;
            return;
        }

        var pt = QueryPoint(p);

        if (!ReferenceEquals(pt, _pickingPoint))
        {
            _pickingPoint = pt;
            Tool.ViewModel.RefreshMapDisplay();
        }
    }
}