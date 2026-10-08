using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

public partial class ParallelTerminalViewModel : PageModel
{
    private readonly ParallelViewModel _parentModel;

    /// <summary>
    /// Should the terminal be floating in space (i.e. not be attached to any line).
    /// </summary>
    [ObservableProperty] private bool _isFloating = false;

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

    /// <summary>
    /// The position of the parallel where it intersects the selected line (null if no line is selected).
    /// </summary>
    internal IPosition? Position { get; private set; }
    
    /// <summary>
    /// Creates the view model for the terminal of a parallel line.
    /// </summary>
    /// <param name="parentModel">The view model that contains this model.</param>
    internal ParallelTerminalViewModel(ParallelViewModel parentModel)
    {
        _parentModel = parentModel;
    }

    partial void OnIsFloatingChanged(bool dontTerminateOnAnyLine)
    {
        SelectedLine = dontTerminateOnAnyLine ? null : FindTerminalLine();
    }

    partial void OnSelectedLineChanged(LineFeature? value)
    {
        Position = _parentModel.FindTerminalPosition(value);
        BadSelectedLine = value is not null && Position is null;
        Tool.RefreshMapDisplay();
    }
    
    internal bool PickLine(IPosition p)
    {
        if (IsFloating)
            return false;
        
        // The tolerance used here is the same tolerance normally used when
        // selecting stuff (c.f. MapEditorViewModel.SelectObject)
        var mapScale = (Tool.ViewModel as IMapEditorViewModel).MapScale;
        ILength tol = new Length(0.001 * mapScale);
        var line = Tool.Store.Model.QueryClosest(p, tol, SpatialType.Line) as LineFeature;
        
        if (!ReferenceEquals(line, SelectedLine))
            SelectedLine = line;

        return line is not null;
    }

    internal override void OnBack()
    {
        // If the user goes back to the previous page, it's possible that they will go on to change
        // the offset of the parallel line. In that case, the selected line may no longer be valid.

        // Setting it to null here will lead to OnSelectedLineChanged, which will therefore null out
        // the Position property and set BadSelectedLine to false.
        
        if (SelectedLine is not null)
            SelectedLine = null;
    }

    internal override bool ValidatePage() => !BadSelectedLine;

    internal override void OnNavigatedTo()
    {
        SelectedLine = FindTerminalLine();
    }

    private LineFeature? FindTerminalLine()
    {
        // Find a line that's closest to the end of the parallel
        var (line, position) = _parentModel.FindTerminal();

        // Return the line only if it actually intersects the line we found (otherwise the user
        // will see a validation message that says the selected line does not intersect)
        return position is null ? null : line;
    }
}