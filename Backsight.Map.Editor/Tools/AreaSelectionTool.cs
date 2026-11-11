using System;
using System.Collections.Generic;
using Avalonia.Input;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

/// <written by="Steve Stanton" on="01-NOV-1999" was="CeSelection" />
/// <summary>
/// Performs line-based and area-based selection of spatial features.
/// This basically farms out the more complex selection stuff so that
/// the controller class is a bit less cluttered.
/// </summary>
internal class AreaSelectionTool
{
    private readonly MapEditorViewModel _mapEditor;

    /// <summary>
    /// The current selection obtained via _limit. This will be included
    /// in the selection when the limit line is completed. Never null.
    /// </summary>
    private readonly List<IMapObject> _limitSelection = new();

    /// <summary>
    /// The points defining the limits for a selection area (points have defined a closed shape).
    /// A new instance is created when a CTRL+MouseDown event initially occurs.
    /// On each further CTRL+MouseDown, a further position is appended to the
    /// list, and features are selected from the map. The features don't get
    /// instantly added to the "official" selection, since it's possible
    /// that further limit line positions will invalidate some of the initially
    /// selected features (consider a situation where the user ends up defining
    /// a concave shape). Instead, we hold on to the features until the limit
    /// line has been completed (which occurs when the CTRL key is released).
    /// </summary>
    private List<IPosition>? _limit;

    /// <summary>
    /// The last mouse position.
    /// </summary>
    private IPosition? _lastPosition;

    internal AreaSelectionTool(MapEditorViewModel mapEditor)
    {
        _mapEditor = mapEditor;
    }

    /// <summary>
    /// The cursor that should be used when using the <c>SelectionTool</c> class.
    /// </summary>
    internal static Cursor Cursor => EditingCursors.AreaSelectionCursor;

    /// <summary>
    /// The features inside the limit line
    /// </summary>
    internal Selection Selection => new Selection(_limitSelection);

    /// <summary>
    /// Accepts a position that the user specified via a left click,
    /// while holding the CTRL key down.
    /// </summary>
    /// <param name="pos">Where did the user click?</param>
    internal void CtrlMouseDown(IPosition pos)
    {
        // If we don't have any positions, just remember the supplied position
        // and create an empty limit line selection.
        if (_limit is null)
        {
            _limit = new List<IPosition>();
            _limit.Add(pos);
            _limitSelection.Clear();
        }
        else
        {
            // Append the new position to our list.
            _limit.Add(pos);

            // Select stuff within the current limit.
            SelectLimit();

            // Ensure the current selection is highlighted.
            _mapEditor.RefreshMapDisplay();
        }
    }

    /// <summary>
    /// Accepts a mouse position while the user has the CTRL key pressed down.
    /// </summary>
    /// <param name="pos">The position of the mouse</param>
    internal void MouseMove(IPosition pos)
    {
        // Just return if a left click hasn't been done yet.
        if (_limit is null)
            return;

        // Hold on to the current mouse position.
        _lastPosition = pos;

        // Ensure current position is (apparently) rubber banded
        // TODO: still needed?
        _mapEditor.RefreshMapDisplay();
    }

    /// <summary>
    /// Selects stuff within the current limit line. This makes a private selection
    /// over and above any previous selection.
    /// </summary>
    private void SelectLimit()
    {
        // Nothing to do if there is no limit line
        if (_limit is null)
            return;

        // Empty out the current limit selection
        _limitSelection.Clear();

        // Nothing to do if there's only one position
        if (_limit.Count <= 1)
            return;

        // If we have just 2 positions, select everything that
        // intersects the line. Otherwise select inside the shape

        try
        {
            // Close the limit line
            _limit.Add(_limit[0]);

            // Select only lines if the limit line consists of only 2 points (otherwise select
            // whatever is currently visible on the active display)
            SpatialType types = _limit.Count == 2 ? SpatialType.Line : _mapEditor.VisibleFeatureTypes;

            // Make the selection
            var index = _mapEditor.Store!.Model.Index;
            var res = new FindOverlapsQuery(index, [.. _limit], types).Result;
            Console.WriteLine("found " + res.Count);
            _limitSelection.AddRange(res);
        }

        catch
        {
            // do nothing
        }

        finally
        {
            // Remove the closing point
            int lastIndex = _limit.Count - 1;
            _limit.RemoveAt(lastIndex);
        }
    }

    /// <summary>
    /// Draws the limit line (if it contains at least 2 positions).
    /// </summary>
    /// <param name="canvas">The display to draw to.</param>
    internal void Render(MapCanvas canvas)
    {
        if (_limit is null || _limit.Count == 0 || _lastPosition is null)
            return;

        // Draw dotted line from the last point on the limit line to the last known mouse position
        IPosition last = _limit[^1];

        var dottedLine = new PaintStyle
        {
            Color = SKColors.Gray,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2,
            Dashed = true
        };
        canvas.DrawLine(last, _lastPosition, dottedLine);

        // If we have two or more positions, draw an additional dotted line to the start of the limit line
        if (_limit.Count >= 2)
            canvas.DrawLine(_lastPosition, _limit[0], dottedLine);

        // Draw the limit line
        if (_limit.Count > 1)
            canvas.DrawPath(_limit, dottedLine);

        // Draw any limit line selection
        var selectionStyle = new PaintStyle
        {
            Color = SKColors.Red,
            Style = SKPaintStyle.Fill
        };

        foreach (var item in _limitSelection)
        {
            if (item is Model.PointFeature pt)
                canvas.Highlight(pt);
            else if (item is LineFeature line)
                canvas.Highlight(line, false);
            else if (item is TextFeature text)
                canvas.DrawText(text.TextGeometry, selectionStyle);
        }
    }
}