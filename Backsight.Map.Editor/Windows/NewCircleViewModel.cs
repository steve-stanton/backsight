using Avalonia.Input;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

internal partial class NewCircleViewModel : ToolViewModel<NewCircleTool>
{
    [ObservableProperty]
    private PointFeature? _selectedCenter;
    
    [ObservableProperty]
    private PointFeature? _selectedRadius;

    internal NewCircleViewModel(NewCircleTool tool) : base(tool)
    {
        
    }
    
    [RelayCommand]
    private void PickCenter()
    {
        
    }

    [RelayCommand]
    private void PickRadius()
    {
        
    }

    /// <summary>
    /// Attempts to pick a point from the map.
    /// </summary>
    /// <param name="p">The selected map position.</param>
    /// <returns>True if a point was selected.</returns>
    internal bool PickPoint(IPosition p)
    {
        return false;
    }

    /// <summary>
    /// Ensures that a request to pick a point from the map has been stopped.
    /// </summary>
    private void StopPicking()
    {
        /*
        _activePickTarget = PickTarget.None;
        _pickingPoint = null;
        */
        Tool.ViewModel.MapCursor = Cursor.Default;
    }

    internal void MouseMove(IPosition p)
    {
        /*
        if (_activePickTarget == PickTarget.None)
        {
            _pickingPoint = null;
            return;
        }

        var pt = QueryPoint(p);

        if (!ReferenceEquals(pt, _pickingPoint))
        {
            _pickingPoint = pt;
            _tool.ViewModel.RefreshMapDisplay();

            if (_activePickTarget == PickTarget.Offset)
                ShowLengthOffset(pt);
        }
        */
    }
}