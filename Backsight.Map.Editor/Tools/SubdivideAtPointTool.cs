using System;
using System.Diagnostics;
using System.Linq;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class SubdivideAtPointTool : CommandTool
{
    /// <summary>
    /// The line to be subdivided.
    /// </summary>
    private readonly LineFeature _line;
    
    /// <summary>
    /// Should subdivision be allowed at line intersections?
    /// </summary>
    private readonly bool _allowIntersections;
    
    /// <summary>
    /// The point that the mouse is currently over (either a PointFeature or an Intersection).
    /// </summary>
    private ITerminal? _currentPoint;
    
    internal SubdivideAtPointTool(MapEditorViewModel viewModel, LineFeature line)
        : base(viewModel, EditingActionId.LineSubdivisionAtPoint)
    {
        _line = line;
        _allowIntersections = viewModel is { ArePointsDrawn: true, Settings.IntersectionsDrawn: true };
    }

    internal override bool Run()
    {
        SetCursor(EditingCursors.SubdivideAtPointCursor);
        return true;
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        var hadPosition = _currentPoint is not null;
        
        // Search for a point that's within 2mm of the line (at the current map scale)
        var scale = (ViewModel as IMapEditorViewModel).MapScale;
        ILength tol = new Length(0.002 * scale);
        ITerminal? splitPosition = null;

        if (_allowIntersections)
        {
            var pg = PointGeometry.Create(p);
            splitPosition = Store.Model.EditingIndex.FindTerminal(pg, tol);
        }
        else
        {
            splitPosition = Store.Model.QueryClosest(p, tol, SpatialType.Point) as PointFeature; 
        }
        
        // Disallow if it coincides with either end point
        if (splitPosition is not null)
        {
            if (splitPosition.IsCoincident(_line.StartPoint) || splitPosition.IsCoincident(_line.EndPoint))
                splitPosition = null;
        }

        // Disallow points that are further away than 0.1mm on the GROUND
        if (splitPosition is not null && _line.Distance(splitPosition).Meters > 0.0001)
            splitPosition = null;

        _currentPoint = splitPosition;

        // Refresh if the position is defined (or was, but is no longer)
        if (_currentPoint is not null || hadPosition)
            ViewModel.RefreshMapDisplay();
    }
    
    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        if (_currentPoint is null)
            return true;

        // Figure out how far along the line the split point is
        uint prat = PositionRatio.GetValue(_line, _currentPoint);

        // Clear the display of the currently selected line (the one we're about to subdivide)
        ViewModel.ClearSelection();

        // Create the two sections
        var op = CreateOperation(prat);
        op.Execute();
        
        // Highlight the created section that is longer (picking the longer section is meant
        // to help in a situation where the user want to go on to split further positions
        // along the length of a line)
        var longerSection = PositionRatio.IsMoreThanMidway(prat) ? op.Section1 : op.Section2;
        Debug.Assert(longerSection is not null);
        ViewModel.Select(longerSection);

        return Finish();
    }

    private LineSubdivisionAtTerminal CreateOperation(uint prat)
    {
        if (_currentPoint is PointFeature pt)
            return new LineSubdivisionAtPointOperation(_line, pt);

        var x = _currentPoint as Intersection;
        Debug.Assert(x is not null);
        
        IDivider[] dividers = x.IncidentDividers();
        if (dividers.Length == 0)
            throw new ApplicationException("Intersection has no incident dividers.");

        var otherLine = dividers.FirstOrDefault(x => !ReferenceEquals(x.Line, _line))?.Line;
        
        // It could conceivably be a self-intersection, which isn't currently supported
        if (otherLine is null)
            throw new ApplicationException("Cannot find intersect with another line");
        
        return new LineSubdivisionAtIntersectionOperation(_line, otherLine, prat);
    }
        
    internal override void Render(MapCanvas canvas)
    {
        if (_currentPoint is not null)
        {
            canvas.DrawPoint(_currentPoint, new PaintStyle
            {
                Color = SKColors.Magenta,
                Style = SKPaintStyle.Fill
            });
        }
    }
}