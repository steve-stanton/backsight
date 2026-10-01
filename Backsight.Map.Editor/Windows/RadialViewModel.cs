using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Avalonia.Input;
using Backsight.Environment;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Observations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

public partial class RadialViewModel : DialogViewModel
{
    private enum PickTarget
    {
        None,
        Backsight,
        Angle,
        Offset,
    }

    private readonly RadialTool _tool;
    private readonly PointFeature _fromPoint;

    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    [ObservableProperty] private decimal? _length;

    /// <summary>
    /// A point that defines the distance to the sideshot point.
    /// </summary>
    /// <remarks>
    /// When defined, the _length property should also be defined to hold the distance
    /// between the from-point and the offset point.
    /// </remarks>
    private PointFeature? _lengthOffset;

    /// <summary>
    /// True if a line should be added too.
    /// </summary>
    [ObservableProperty] private bool _wantLine = true;

    [ObservableProperty] private IEntity _selectedPointType;
    
    [ObservableProperty] private DisplayId[] _pointIds;
    
    [ObservableProperty] private DisplayId? _selectedPointId;

    [ObservableProperty] private PointFeature? _selectedBacksight;

    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    [NotifyDataErrorInfo]
    [AngleFormat]
    [ObservableProperty] private string? _angleText;
    
    /// <summary>
    /// Is the angle expected to be a parallel direction?
    /// </summary>
    [ObservableProperty] private bool _isParallel = false;
    
    /// <summary>
    /// Is angle clockwise (relevant only if a backsight has been specified).
    /// </summary>
    [ObservableProperty] private bool _isClockwise = true;
    
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

    /// <summary>
    /// The point defining the first point of a parallel direction. 
    /// </summary>
    private PointFeature? _par1;

    /// <summary>
    /// The point defining the second point of a parallel direction. 
    /// </summary>
    private PointFeature? _par2;

    /// <summary>
    /// Any circles incident on the from point.
    /// </summary>
    private readonly List<Circle> _circles;
    
    internal bool HasCircles => _circles.Count > 0;

    /// <summary>
    /// Has the backsight been defined as a circle center point?
    /// </summary>
    [ObservableProperty] private bool _usingArcCenter = false;
    
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
    
    internal RadialViewModel(RadialTool tool, PointFeature fromPoint)
    {
        _tool = tool;
        _fromPoint = fromPoint;

        // Get the units hint for the extension length
        var entryUnit = tool.Store.Settings.EntryUnit;
        Units = DistanceUnit.GetUnit(entryUnit).Abbreviation + " ";
 
        // Fill the combo with the entity types for the sideshot point
        PointTypes = tool.ViewModel.Environment.EntityTypes
            .Where(x => x.IsPointValid && x.Id != 0)
            .OrderBy(x => x.Name)
            .ToArray();

        _selectedPointType = tool.Store.DefaultPointType;

        // Load the ID combo and select the first available ID
        _pointIds = GetPointIds(tool.Store, _selectedPointType);
        if (_pointIds.Length > 0)
            _selectedPointId = _pointIds[0];
        
        // If we are auto-numbering, disable the combo.
        AllowIdSelection = !tool.Store.Settings.AutoNumber;

        // Sometimes the BC or EC is apparently not EXACT when the data
        // arrives from a foreign source, so allow 1mm on the ground.
        _circles = tool.Store.Model.FindCircles(_fromPoint, new Length(0.001));
    }

    // TODO: Exact copy of what's in LineExtensionViewModel => make this an extension method of the store
    private static DisplayId[] GetPointIds(IMapStore store, IEntity entityType)
    {
        var session = store.Model.WorkingSession;
        Debug.Assert(session is not null);
        
        var idMan = store.Model.IdManager;
        var idGroup = idMan.GetGroup(entityType);
        if (idGroup is null)
            return [];

        // Get the available IDs for the group
        uint[] avail = idGroup.GetAvailIds(); 

        // If we didn't find any, obtain an extra allocation
        if (avail.Length == 0)
        {
            idGroup.GetAllocation(session);
            // TODO: Tell the user that we've made an ID allocation
            
            avail = idGroup.GetAvailIds();
            if (avail.Length == 0)
                throw new ApplicationException("Cannot obtain ID allocation");
        }
        
        return avail.Select(x => new DisplayId(idGroup, x)).ToArray();
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
            PointIds = GetPointIds(_tool.Store, value);

            if (PointIds.Length == 0)
                SelectedPointId = null;
            else
                SelectedPointId = _pointIds[0];
        }
    }
    
    partial void OnLengthChanged(decimal? value)
    {
        // The displayed length may be changed while picking an offset point. But if the
        // user has picked, they might go on to change things - in that case, the offset
        // point is no longer valid
        if (_activePickTarget != PickTarget.Offset)
            _lengthOffset = null;
        
        _tool.RefreshMapDisplay();
    }

    partial void OnWantLineChanged(bool value)
    {
        _tool.RefreshMapDisplay();
    }

    partial void OnAngleTextChanged(string? text)
    {
        if (text is not null)
        {
            // Text that starts with a "-" character is a shortcut for a counter-clockwise angle.
            // If that's the case, ensure the clockwise flag is false.
            if (text.StartsWith("-") && IsClockwise)
                IsClockwise = false;
        }
            
        _tool.RefreshMapDisplay();
    }

    partial void OnIsClockwiseChanged(bool value)
    {
        _tool.RefreshMapDisplay();
    }
    
    internal PointFeature From => _fromPoint;
    internal PointFeature? Parallel1 => _par1;
    internal PointFeature? Parallel2 => _par2;
    internal PointFeature? LengthOffset => _lengthOffset;
    
    protected override bool CanExecuteOk()
    {
        return Length > 0 && IsAngleDefined;
    }
    
    private bool IsAngleDefined
    {
        get
        {
            if (_activePickTarget == PickTarget.Angle)
                return false;

            return !String.IsNullOrWhiteSpace(AngleText);
        }
    }

    internal override bool HandleCloseRequested(DialogResult result)
    {
        Console.WriteLine("Radial requesting close with result: " + result);

        if (result == DialogResult.OK)
        {
            // return if fails validation
            //Debug.Assert(CanExecuteOk());
            
            _tool.FinishDataEntry(this);
        }
        else
        {
            _tool.CancelDataEntry(this);
        }

        return true;
    }
    
    [RelayCommand]
    private void SetOffset()
    {
        Console.WriteLine("Radial setting offset");
    }
    
    [RelayCommand]
    private void PickBacksight()
    {
        ClearBacksight();
        _activePickTarget = PickTarget.Backsight;
        _tool.ViewModel.MapCursor = EditingCursors.PickCursor;
    }
    
    [RelayCommand]
    private void PickParallel()
    {
        _activePickTarget = PickTarget.Angle;
        _par1 = _par2 = null;
        _tool.ViewModel.MapCursor = EditingCursors.PickCursor;
        AngleText = "...";
        
        ClearBacksight();
        IsParallel = true;
    }

    /// <summary>
    /// Clears a previously selected backsight, ensuring that the map display
    /// gets refreshed to redraw it in its normal color.
    /// </summary>
    private void ClearBacksight()
    {
        if (SelectedBacksight is not null)
        {
            // If the user previously said the backsight should be a circle center point, clear it now
            UsingArcCenter = false;
            
            SelectedBacksight = null;
            _tool.ViewModel.RefreshMapDisplay();
        }
    }

    [RelayCommand]
    private void PickOffset()
    {
        _activePickTarget = PickTarget.Offset;
        _lengthOffset = null;
        _tool.ViewModel.MapCursor = EditingCursors.PickCursor;
        ShowLengthOffset(null);
    }
        
    internal PointFeature? PickingBacksight => _activePickTarget == PickTarget.Backsight ? _pickingPoint : null;
    internal PointFeature? PickingParallel => _activePickTarget == PickTarget.Angle ? _pickingPoint : null;
    internal PointFeature? PickingOffset => _activePickTarget == PickTarget.Offset ? _pickingPoint : null;
    
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
        
        if (_activePickTarget == PickTarget.Backsight)
        {
            SelectedBacksight = pt;
            StopPicking();
        }
        else if (_activePickTarget == PickTarget.Offset)
        {
            // Adjust the displayed length while the pick target remains active (otherwise _lengthOffset
            // will get cleared via OnLengthChanged)
            ShowLengthOffset(pt);
            StopPicking();
            _lengthOffset = pt;
        }
        else if (_activePickTarget == PickTarget.Angle)
        {
            if (pt is null)
            {
                // Ensure the first parallel point gets cleared when the second click was in open space (the
                // 2nd point can't be defined - otherwise the pick operation should have already finished)
                _par1 = null;
                Debug.Assert(_par2 is null);

                IsParallel = false;
                AngleText = null;
            }
            else
            {
                if (_par1 is null)
                    _par1 = pt;
                else
                    _par2 = pt;

                if (_par1 is not null && _par2 is not null)
                {
                    AngleText = $"+{_par1.FormattedKey} +{_par2.FormattedKey}";
                }
                else
                {
                    Debug.Assert(_par1 is not null);
                    AngleText = $"+{_par1.FormattedKey} ...";
                }
            }

            // We're done if two points have now been picked
            if (_par1 is not null && _par2 is not null)
                StopPicking();
        }

        return pt is not null;
    }

    /// <summary>
    /// Ensures that a request to pick a point from the map has been stopped.
    /// </summary>
    private void StopPicking()
    {
        _activePickTarget = PickTarget.None;
        _pickingPoint = null;
        _tool.ViewModel.MapCursor = Cursor.Default;
    }

    private void ShowLengthOffset(PointFeature? offsetPoint)
    {
        if (offsetPoint is null)
        {
            Length = null;
        }
        else
        {
            // Get the length on the mapping place
            var len = BasicGeom.Distance(_fromPoint, offsetPoint);
            
            // Express as a distance on the ground
            var scaleFactor = _fromPoint.SpatialSystem.GetLineScaleFactor(_fromPoint, offsetPoint);
            len /= scaleFactor;
            
            // The displayed length needs to be in the current data entry units, and with
            // the default number of decimal places for that unit type.
            var entryUnit = DistanceUnit.GetUnit(_tool.Store.Settings.EntryUnit);
            len = entryUnit.FromMetric(len);
 
            Length = Decimal.Round((decimal)len, entryUnit.DisplayPrecision);
        }
    }

    private PointFeature? QueryPoint(IPosition p)
    {
        ILength size = new Length(_tool.Store.Settings.PointHeight * 0.5);
        return _tool.Store.Model.Index.QueryClosest(p, size, SpatialType.Point) as PointFeature;
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
            _tool.ViewModel.RefreshMapDisplay();

            if (_activePickTarget == PickTarget.Offset)
                ShowLengthOffset(pt);
        }
    }

    /// <summary>
    /// The distance to the sideshot point (null if not entered).
    /// </summary>
    /// <returns>The observed distance to the sideshot point (could be an offset point).</returns>
    internal Observation? GetLengthObservation()
    {
        if (_lengthOffset is not null)
            return new OffsetPoint(_lengthOffset);
        
        if (Length is null)
            return null;

        var len = Decimal.ToDouble(Length.Value);
        var entryUnit = _tool.Store.Settings.EntryUnit;
        return new Distance(len, DistanceUnit.GetUnit(entryUnit));
    }

    [RelayCommand]
    private void UseArcCenter()
    {
        // The option to use arc center should have been hidden when no circles in sight
        if (_circles.Count == 0)
            return;

        // If the user has launched the backsight picker, clear it now (the click to use
        // the arc center takes priority). Also stop if the parallel picker is ongoing (since
        // backsights are n/a when the angle is defined as a parallel)
        if (_activePickTarget is PickTarget.Backsight or PickTarget.Angle)
        {
            if (_activePickTarget == PickTarget.Angle)
                _par1 = null;
            
            StopPicking();
        }
     
        if (UsingArcCenter)
        {
            if (_circles.Count == 1)
                SelectedBacksight = _circles[0].Center as PointFeature;
            else
                Console.WriteLine("TODO: Handle more than 1 circle");
            
            // Ensure the newly assigned backsight is visible on the map
            _tool.ViewModel.RefreshMapDisplay();
        }
        else
        {
            ClearBacksight();
        }
    }
}