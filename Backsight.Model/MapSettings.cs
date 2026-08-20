using System.Text.Json.Serialization;
using Backsight.Database;
using Backsight.Environment;

namespace Backsight.Model;

/// <summary>
/// Editing and display preferences for a map.
/// </summary>
public class MapSettings
{
    // State...
    
    /// <summary>
    /// Have changes been made to the values stored in this instance?
    /// </summary>
    private bool _isDirty;

    // Display preferences...
    
    /// <summary>
    /// The area where the map was last drawn.
    /// </summary>
    private WorkingArea _workingArea;

    /// <summary>
    /// Current display units.
    /// </summary>
    private DistanceUnitType _displayUnit;

    /// <summary>
    /// Scale denominator at which labels (text) will start to be drawn.
    /// </summary>
    private double _showLabelScale;

    /// <summary>
    /// Scale denominator at which points will start to be drawn.
    /// </summary>
    private double _showPointScale;

    /// <summary>
    /// Height of point symbols, in meters on the ground.
    /// </summary>
    private double _pointHeight;

    /// <summary>
    /// Should intersection points be drawn? Relevant only if points
    /// are drawn at the current display scale (see the <see cref="ShowPointScale"/>
    /// property).
    /// </summary>
    private bool _areIntersectionsDrawn;

    /// <summary>
    /// The style for annotating lines with distances (and angles)
    /// </summary>
    private LineAnnotationStyle _annotationStyle;

    // Data entry preferences...
    
    /// <summary>
    /// Current data entry units.
    /// </summary>
    private DistanceUnitType _entryUnit;

    /// <summary>
    /// Should feature IDs be assigned automatically? (false if the user must specify).
    /// </summary>
    private bool _autoNumber;

    /// <summary>
    /// The nominal map scale, for use in converting the size of fonts.
    /// </summary>
    private uint _nominalMapScale;

    /// <summary>
    /// The ID of the map layer that was last activated (0 if the map has never been opened).
    /// </summary>
    /// <remarks>This must correspond to one of the items in <see cref="_layerDefaults"/>.</remarks>
    private int _activeLayer;

    /// <summary>
    /// The number of items that have been saved as part of the map.
    /// </summary>
    private uint _savedItemCount;

    /// <summary>
    /// Defaults that should be used when working with different map layers.
    /// </summary>
    private List<LayerDefaults> _layerDefaults = new();
    
    public MapSettings()
    {
        // Display
        _workingArea = new WorkingArea(0.0, 0.0, 0.0);
        _displayUnit = DistanceUnitType.AsEntered;
        _showLabelScale = 2000.0;
        _showPointScale = 2000.0;
        _pointHeight = 2.0;
        _areIntersectionsDrawn = false;
        _nominalMapScale = 2000;
        _annotationStyle = new LineAnnotationStyle();
        
        // Data entry
        _entryUnit = DistanceUnitType.Meters;
        _autoNumber = true;
        _activeLayer = 0;

        // State
        _isDirty = false;
    }

    /// <summary>
    /// Method called whenever values of this class are changed. This just ensures
    /// that <see cref="_isDirty"/> gets set.
    /// </summary>
    /// <typeparam name="T">The type of value that's being changed</typeparam>
    /// <param name="value">The value to assign</param>
    /// <returns>The supplied value</returns>
    private T Set<T>(T value)
    {
        _isDirty = true;
        return value;
    }

    /// <summary>
    /// Information about the area that was last drawn.
    /// </summary>
    public WorkingArea LastDraw
    {
        get => _workingArea;
        set => _workingArea = Set(value);
    }

    /// <summary>
    /// Current display units
    /// </summary>
    public DistanceUnitType DisplayUnit
    {
        get => _displayUnit;
        set => _displayUnit = Set(value);
    }

    /// <summary>
    /// Current data entry units
    /// </summary>
    public DistanceUnitType EntryUnit
    {
        get => _entryUnit;
        set => _entryUnit = Set(value);
    }

    /// <summary>
    /// Should feature IDs be assigned automatically? (false if the user must specify).
    /// </summary>
    public bool AutoNumber
    {
        get => _autoNumber;
        set => _autoNumber = Set(value);
    }

    /// <summary>
    /// Scale denominator at which labels (text) will start to be drawn.
    /// </summary>
    public double LabelScale
    {
        get => _showLabelScale;
        set => _showLabelScale = Set(value);
    }

    /// <summary>
    /// Scale denominator at which points will start to be drawn.
    /// </summary>
    public double PointScale
    {
        get => _showPointScale;
        set => _showPointScale = Set(value);
    }

    /// <summary>
    /// Height of point symbols, in meters on the ground.
    /// </summary>
    public double PointHeight
    {
        get => _pointHeight;
        set => _pointHeight = Set(value);
    }

    /// <summary>
    /// Should intersection points be drawn? Relevant only if points
    /// are drawn at the current display scale (see the <see cref="ShowPointScale"/> property).
    /// </summary>
    public bool IntersectionsDrawn
    {
        get => _areIntersectionsDrawn;
        set => _areIntersectionsDrawn = Set(value);
    }

    /// <summary>
    /// The nominal map scale, for use in converting the size of fonts.
    /// </summary>
    public uint NominalMapScale
    {
        get => _nominalMapScale;
        set => _nominalMapScale = Set(value);
    }

    /// <summary>
    /// The style for annotating lines with distances (and angles)
    /// </summary>
    public LineAnnotationStyle LineAnnotation
    {
        get => _annotationStyle;
        set => _annotationStyle = Set(value);
    }

    /// <summary>
    /// The ID of the map layer that was last activated (0 if the map has never been opened).
    /// </summary>
    public int ActiveLayer
    {
        get => _activeLayer;
        set => _activeLayer = Set(value);
    }

    /// <summary>
    /// The number of items that have been saved as part of the map.
    /// </summary>
    public uint SavedItemCount
    {
        get => _savedItemCount;
        set => _savedItemCount = Set(value);
    }

    /// <summary>
    /// Has the information recorded in this instance been changed since it was created?
    /// </summary>
    [JsonIgnore]
    public bool IsDirty
    {
        get => _isDirty;
        set => _isDirty = value;
    }

    /// <summary>
    /// The defaults for the currently active map layer (null if defaults have not been attached).
    /// </summary>
    public LayerDefaults? Defaults => _layerDefaults.FirstOrDefault(x => x.LayerId == _activeLayer); 
    
    /// <summary>
    /// Gets the defaults for a specific map layer (attaching layer defaults if necessary).
    /// </summary>
    /// <param name="layer">The layer of interest.</param>
    /// <returns>The corresponding defaults.</returns>
    /// <remarks>
    /// If the user specifies any changes to the defaults, these need to be passed back via
    /// a call to <see cref="ReplaceDefaults"/> (the map settings must then be saved).
    /// </remarks>
    public LayerDefaults GetDefaults(ILayer layer)
    {
        var result = _layerDefaults.FirstOrDefault(x => x.LayerId == layer.Id);
        return result ?? AttachDefaults(layer);
    }
    
    /// <summary>
    /// Ensures preferences have been defined to match the defaults for a map layer.
    /// </summary>
    /// <param name="layer">The map layer for the project</param>
    /// <returns>The defaults that have been attached.</returns>
    private LayerDefaults AttachDefaults(ILayer layer)
    {
        var result = new LayerDefaults(
            layer.Id,
            layer.DefaultPointType.Id,
            layer.DefaultLineType.Id,
            layer.DefaultPolygonType.Id,
            layer.DefaultTextType.Id);

        ReplaceDefaults(result);
        return result;
    }
    
    /// <summary>
    /// Replaces preferences for working with a specific map layer.
    /// </summary>
    /// <param name="defaults">The modified defaults.</param>
    /// <remarks>
    /// Having changed the defaults, the map settings must be saved at some point
    /// by calling <see cref="IMapRepository.SaveMapSettings"/>.
    /// </remarks>
    public void ReplaceDefaults(LayerDefaults defaults)
    {
        _layerDefaults.RemoveAll(x => x.LayerId == defaults.LayerId);
        _layerDefaults.Add(defaults);
        _isDirty = true;
    }

    /// <summary>
    /// The default entity type for point features.
    /// </summary>
    internal IEntity DefaultPointType
    {
        get
        {
            int entityId = Defaults?.PointType ?? 0;
            return EnvironmentRepository.FindEntityById(entityId);
        }
    }

    /// <summary>
    /// The default entity type for line features.
    /// </summary>
    internal IEntity DefaultLineType
    {
        get
        {
            int entityId = Defaults?.LineType ?? 0;
            return EnvironmentRepository.FindEntityById(entityId);
        }
    }

    /// <summary>
    /// The default entity type for polygon labels.
    /// </summary>
    internal IEntity DefaultPolygonType
    {
        get
        {
            int entityId = Defaults?.PolygonType ?? 0;
            return EnvironmentRepository.FindEntityById(entityId);
        }
    }

    /// <summary>
    /// The default entity type for miscellaneous text features.
    /// </summary>
    internal IEntity DefaultTextType
    {
        get
        {
            int entityId = Defaults?.TextType ?? 0;
            return EnvironmentRepository.FindEntityById(entityId);
        }
    }
}

