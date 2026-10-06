using System;
using Avalonia.Input;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Observations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class ParallelOffsetViewModel : ViewModelBase
{
    private readonly ParallelViewModel _parentModel;
    
    /// <summary>
    /// The units to display for the offset length.
    /// </summary>
    internal string Units { get; }

    /// <summary>
    /// The distance to the parallel (null if not entered).
    /// </summary>
    [ObservableProperty] private decimal? _offsetDistance;

    /// <summary>
    /// Can the user add the parallel on the other side of the reference line (applies
    /// only if the offset is specified as a distance).
    /// </summary>
    [ObservableProperty] private bool _allowOtherWay = true;

    /// <summary>
    /// True if the parallel should be generated on the non-default side of the reference line.
    /// </summary>
    private bool _useNegatedOffset = false;
    
    /// <summary>
    /// A point that defines the distance to the parallel line.
    /// </summary>
    /// <remarks>
    /// When defined, the _offset property should also be defined to hold the offset distance.
    /// </remarks>
    private PointFeature? _offsetPoint;

    /// <summary>
    /// Is the user currently picking an offset point?
    /// </summary>
    private bool _picking = false;

    /// <summary>
    /// The point that the mouse is hovering over while actively picking a point.
    /// Null if not currently picking, or the last mouse position does not overlap any point. 
    /// </summary>
    private PointFeature? _pickingPoint;
    
    internal ParallelOffsetViewModel(ParallelViewModel parentModel)
    {
        _parentModel = parentModel;
        
        // Get the units hint for the offset distance
        var entryUnit = parentModel.Tool.Store.Settings.EntryUnit;
        Units = DistanceUnit.GetUnit(entryUnit).Abbreviation + " ";
    }

    private ParallelTool Tool => _parentModel.Tool;
    
    [RelayCommand]
    private void PickOffset()
    {
        _picking = true;
        _offsetPoint = null;
        AllowOtherWay = false;
        Tool.ViewModel.MapCursor = EditingCursors.PickCursor;
        ShowOffsetDistance(null);
    }
    
    partial void OnOffsetDistanceChanged(decimal? value)
    {
        // The displayed offset may be changed while picking an offset point. But if the
        // user has picked, they might go on to change things - in that case, the offset
        // point is no longer valid
        if (!_picking)
        {
            _offsetPoint = null;
            AllowOtherWay = true;
        }

        // TODO: If an offset point was previously defined and it was not on the
        // default side of the reference line, ensure we stay on the same side
        
        var offset = GetOffsetObservation();
        Tool.OnOffsetChanged(offset);
    }

    internal PointFeature? OffsetPoint => _offsetPoint;
    internal PointFeature? PickingOffsetPoint => _picking ? _pickingPoint : null;
    
    /// <summary>
    /// Attempts to pick a point from the map.
    /// </summary>
    /// <param name="p">The selected map position.</param>
    /// <returns>True if a point was selected.</returns>
    internal bool PickPoint(IPosition p)
    {
        if (!_picking)
            return false;
        
        var pt = QueryPoint(p);
        
        // Adjust the displayed length while picking remains active (otherwise _offsetPoint
        // will get cleared via OnOffsetDistanceChanged)
        _offsetPoint = pt;
        ShowOffsetDistance(pt);
        
        StopPicking();

        return pt is not null;
    }

    private PointFeature? QueryPoint(IPosition p)
    {
        ILength size = new Length(Tool.Store.Settings.PointHeight * 0.5);
        return Tool.Store.Model.QueryClosest(p, size, SpatialType.Point) as PointFeature;
    }
    
    internal void MouseMove(IPosition p)
    {
        if (!_picking)
        {
            _pickingPoint = null;
            return;
        }
        
        var pt = QueryPoint(p);

        if (!ReferenceEquals(pt, _pickingPoint))
        {
            _pickingPoint = pt;
            Tool.ViewModel.RefreshMapDisplay();
            ShowOffsetDistance(pt);
        }
    }
    
    /// <summary>
    /// Ensures that a request to pick a point from the map has been stopped.
    /// </summary>
    private void StopPicking()
    {
        _picking = false;
        _pickingPoint = null;
        AllowOtherWay = _offsetPoint is null;
        Tool.ViewModel.MapCursor = Cursor.Default;
    }
    
    /// <summary>
    /// Displays an offset distance that corresponds to the position of an offset point
    /// (or displays nothing if the offset point is not defined).
    /// </summary>
    /// <param name="offsetPoint">The offset point (if any). Can either refer to <see cref="_offsetPoint"/>
    /// or <see cref="_pickingPoint"/>.</param>
    private void ShowOffsetDistance(PointFeature? offsetPoint)
    {
        if (offsetPoint is null)
        {
            OffsetDistance = null;
        }
        else
        {
            // Get the offset distance on the mapping plane
            var len = GetPlanerDistance(offsetPoint);

            // Express as a distance on the ground (assume the start of the reference line is good enough)
            var refStart = _parentModel.ReferenceLine.StartPoint;
            var scaleFactor = offsetPoint.SpatialSystem.GetLineScaleFactor(refStart, offsetPoint);
            len /= scaleFactor;

            // The displayed length needs to be in the current data entry units, and with
            // the default number of decimal places for that unit type.
            var entryUnit = DistanceUnit.GetUnit(Tool.Store.Settings.EntryUnit);
            len = entryUnit.FromMetric(len);

            OffsetDistance = Decimal.Round((decimal)len, entryUnit.DisplayPrecision);
        }
    }

    /// <summary>
    /// Calculates the perpendicular distance (in meters, on the mapping plane) between the
    /// reference line and an offset point.
    /// </summary>
    /// <param name="p">The offset point.</param>
    /// <returns>The corresponding offset distance (in meters on the mapping plane).</returns>
    private double GetPlanerDistance(PointFeature p)
    {
        var refLine = _parentModel.ReferenceLine;
        var arc = refLine.GetArcBase()?.Geometry;

        if (arc is not null)
        {
            // Get the curve info
            IPosition center = arc.Circle.Center;

            // Get the distance from the centre of the circle to the offset point.
            double distanceToCenter = BasicGeom.Distance(p, center);
    
            // Return an offset depending on whether the offset point is inside or outside the circle.
            if (distanceToCenter < arc.Circle.Radius)
                return arc.Circle.Radius - distanceToCenter;
            else
                return distanceToCenter - arc.Circle.Radius;
        }
        
        // Assume a straight line
        var s = refLine.StartPoint;
        var e = refLine.EndPoint;
        double bearing = BasicGeom.BearingInRadians(s, e);
        return Math.Abs(BasicGeom.SignedDistance(s.X, s.Y, bearing, p.X, p.Y));
    }
    
    /// <summary>
    /// The distance to the parallel (null if not entered).
    /// </summary>
    /// <returns>The observed distance to the parallel (could be an offset point).</returns>
    private Observation? GetOffsetObservation()
    {
        var opt = _picking ? PickingOffsetPoint : OffsetPoint;
        if (opt is not null)
            return new OffsetPoint(opt);
        
        if (OffsetDistance is null)
            return null;

        var len = Decimal.ToDouble(OffsetDistance.Value);
        
        if (_useNegatedOffset)
            len = -len;
        
        var entryUnit = Tool.Store.Settings.EntryUnit;
        return new Distance(len, DistanceUnit.GetUnit(entryUnit));
    }

    [RelayCommand]
    private void OtherWay()
    {
        _useNegatedOffset = !_useNegatedOffset;
        Tool.OnOffsetChanged(GetOffsetObservation());
    }
}