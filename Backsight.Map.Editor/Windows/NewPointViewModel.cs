using System;
using System.Linq;
using Backsight.Environment;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// See <see cref="NewPointWindow"/>.
/// </summary>
/// <seealso cref="NewPointTool" />
internal partial class NewPointViewModel : DialogViewModel
{
    private IMapStore Store { get; }
    
    /// <summary>
    /// The entity types that can be used for points.
    /// </summary>
    internal IEntity[] PointTypes { get; }

    /// <summary>
    /// The currently selected point type.
    /// </summary>
    [ObservableProperty] private IEntity _selectedPointType;

    /// <summary>
    /// The available IDs for the selected point type.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private DisplayId[] _pointIds;
    
    /// <summary>
    /// The currently selected point ID.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private DisplayId? _selectedPointId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private decimal? _northing;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private decimal? _easting;

    [ObservableProperty]
    private decimal? _elevation;
    
    internal NewPointViewModel(MapEditorViewModel vm)
    {
        Store = vm.Store ?? throw new ArgumentNullException(nameof(IMapStore));
        
        // Fill the combo with the entity types for the sideshot point
        PointTypes = vm.Environment.EntityTypes
            .Where(x => x.IsPointValid && x.Id != 0)
            .OrderBy(x => x.Name)
            .ToArray();

        _selectedPointType = Store.DefaultPointType;

        // Load the ID combo and select the first available ID
        _pointIds = Store.GetPointIds(_selectedPointType);
        if (_pointIds.Length > 0)
            _selectedPointId = _pointIds[0];
    }
        
    protected override bool CanExecuteOk()
    {
        if (PointIds.Length > 0 && SelectedPointId is null)
            return false;
        
        // Could perhaps check whether the entered values are "reasonable" (e.g. somewhere
        // inside the current map extent). Though the reason for not allowing it would likely need
        // some sort of additional visual feedback.
        
        return Northing.HasValue && Easting.HasValue;
    }
    
    /// <summary>
    /// The position of the new point (null if the position has not been fully specified).
    /// </summary>
    internal IPosition? Position
    {
        get
        {
            if (Northing.HasValue && Easting.HasValue)
            {
                var y = Decimal.ToDouble(Northing.Value);
                var x = Decimal.ToDouble(Easting.Value);

                if (Elevation.HasValue)
                    return new Position3D(x, y, Decimal.ToDouble(Elevation.Value));
                else
                    return new Position(x, y);
            }

            return null;
        }
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
            PointIds = Store.GetPointIds(value);

            if (PointIds.Length == 0)
                SelectedPointId = null;
            else
                SelectedPointId = _pointIds[0];
        }
    }
}