using Avalonia.Input;
using Avalonia.Media;
using Backsight.Map.Editor.Tools;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal class PickPointViewModel : PageModel
{
    private CommandTool Tool { get; }
    internal Brush PointFill { get; }
    internal string Prompt { get; }
    internal PointFeature? SelectedPoint { get; private set; }

    /// <summary>
    /// The point that the mouse is hovering over while actively picking a point.
    /// Null if the last mouse position does not overlap any point."/> 
    /// </summary>
    internal PointFeature? PickingPoint { get; private set; }

    internal PickPointViewModel(
        CommandTool tool,
        Color pointColor,
        string prompt)
    {
        Tool = tool;
        Prompt = prompt;
        PointFill = new SolidColorBrush(pointColor);
    }

    /// <summary>
    /// Attempts to pick a point from the map.
    /// </summary>
    /// <param name="p">The selected map position.</param>
    /// <returns>True if a point was selected.</returns>
    internal bool PickPoint(IPosition p)
    {
        SelectedPoint = QueryPoint(p);
        return SelectedPoint is not null;
    }

    internal void MouseMove(IPosition p)
    {
        var pt = QueryPoint(p);

        if (!ReferenceEquals(pt, PickingPoint))
        {
            PickingPoint = pt;
            Tool.ViewModel.RefreshMapDisplay();
        }
    }

    private PointFeature? QueryPoint(IPosition p)
    {
        ILength size = new Length(Tool.Store.Settings.PointHeight * 0.5);
        return Tool.Store.Model.QueryClosest(p, size, SpatialType.Point) as PointFeature;
    }
    
    internal override bool ValidatePage() => SelectedPoint is not null;

    internal override void OnBack()
    {
        Tool.ViewModel.MapCursor = Cursor.Default;
    }

    internal override void OnNext()
    {
        Tool.ViewModel.MapCursor = Cursor.Default;
    }

    internal override void OnNavigatedTo()
    {
        Tool.ViewModel.MapCursor = EditingCursors.PickCursor;
    }
}