using System;
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
using NetTopologySuite.Operation.Overlay.Validate;

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
        // If a backsight was already picked, ensure it gets re-rendered in its normal color
        // (not to be confused with a different point the user is hovering over)
        if (SelectedBacksight is not null)
        {
            SelectedBacksight = null;
            _tool.ViewModel.RefreshMapDisplay();
        }

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
        IsParallel = true;
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
    
    internal bool PickPoint(IPosition p)
    {
        if (_activePickTarget == PickTarget.Backsight)
        {
            // Cancel the pick if the user clicked in open space
            SelectedBacksight = QueryPoint(p);
            _activePickTarget = PickTarget.None;
            _pickingPoint = null;
            _tool.ViewModel.MapCursor = Cursor.Default;
            return true;
        }

        if (_activePickTarget == PickTarget.Offset)
        {
            // Adjust the displayed length while the pick target remains active (otherwise _lengthOffset
            // will get cleared via OnLengthChanged)
            var pt = QueryPoint(p);
            ShowLengthOffset(pt);
            
            _activePickTarget = PickTarget.None;
            _pickingPoint = null;
            _tool.ViewModel.MapCursor = Cursor.Default;
            _lengthOffset = pt;
            return true;
        }
        
        if (_activePickTarget == PickTarget.Angle)
        {
            var pt = QueryPoint(p);

            if (pt is not null)
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

            // We're done if the user clicked in open space, or two points have now been picked
            if (pt is null || (_par1 is not null && _par2 is not null))
            {
                _activePickTarget = PickTarget.None;
                _tool.ViewModel.MapCursor = Cursor.Default;

                if (pt is null)
                {
                    IsParallel = false;
                    AngleText = null;
                }
            }

            return true;
        }

        return false;
    }

    private void ShowLengthOffset(PointFeature? offsetPoint)
    {
        if (offsetPoint is null)
        {
            Length = null;
        }
        else
        {
            var len = BasicGeom.Distance(_fromPoint, offsetPoint);
            
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
}