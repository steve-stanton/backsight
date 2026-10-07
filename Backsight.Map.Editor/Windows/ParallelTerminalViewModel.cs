using System;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class ParallelTerminalViewModel : PageModel
{
    private readonly ParallelViewModel _parentModel;
    private LineFeature? _line;

    /// <summary>
    /// Should the terminal be floating in space (i.e. not be attached to any line).
    /// </summary>
    [ObservableProperty] private bool _isFloating = false;

    /// <summary>
    /// Creates the view model for the terminal of a parallel line.
    /// </summary>
    /// <param name="parentModel">The view model that contains this model.</param>
    internal ParallelTerminalViewModel(ParallelViewModel parentModel)
    {
        _parentModel = parentModel;
    }

    private ParallelTool Tool => _parentModel.Tool;
    
    /// <summary>
    /// The line that is currently selected (does not intersect the parallel if
    /// <see cref="BadSelectedLine"/> is true).
    /// </summary>
    [ObservableProperty] private LineFeature? _selectedLine;
    
    /// <summary>
    /// The <see cref="SelectedLine"/> property has been defined, but it doesn't intersect the parallel.
    /// </summary>
    [ObservableProperty] private bool _badSelectedLine;

    partial void OnIsFloatingChanged(bool value)
    {
        if (value)
        {
            _line = null;
            Tool.RefreshMapDisplay();
        }
    }
    
    internal bool PickLine(IPosition p)
    {
        if (IsFloating)
            return false;
        
        var mapScale = (Tool.ViewModel as IMapEditorViewModel).MapScale;
        ILength tol = new Length(0.001 * mapScale);
        var line = Tool.Store.Model.QueryClosest(p, tol, SpatialType.Line) as LineFeature;
        
        // If a line has just been selected, confirm that the parallel actually intersects it
        if (!ReferenceEquals(line, SelectedLine))
        {
            SelectedLine = line;
            BadSelectedLine = DoesLineIntersectParallel(line);
            _parentModel.OnTerminalChanged(this);
        }
        
        return line is not null;
    }

    private bool DoesLineIntersectParallel(LineFeature? line)
    {
        if (line is null)
            return false;
        
        return true;
    }

    internal override void OnBack()
    {
        // If the user goes back to the previous page, it's possible that they will go on to change
        // the offset of the parallel line. In that case, the selected line may no longer be valid.
        
        if (SelectedLine is not null)
        {
            BadSelectedLine = false;
            SelectedLine = null;
            Tool.RefreshMapDisplay();
        }
    }

    internal override bool ValidatePage() => !BadSelectedLine;
}