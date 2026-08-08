using System;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Models;
using Backsight.Model;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Rendering.Skia.Extensions;

namespace Backsight.Map.Editor.ViewModels;

public interface IMapEditorViewModel
{
    /// <summary>
    /// The data for a map display.
    /// </summary>
    Mapsui.Map MapData { get; }
    
    string? CurrentMapName { get; }
    void OpenMap(string mapName);
    
    /// <summary>
    /// Closes any map that is currently open.
    /// </summary>
    /// <returns>True if a map was closed, or false if there was no open map.</returns>
    bool CloseMap();

    /// <summary>
    /// Gets the types of spatial objects that should be rendered at the current map scale.
    /// </summary>
    /// <returns>The relevant spatial types.</returns>
    SpatialType GetTypesAtCurrentScale();
    
    /// <summary>
    /// The settings for the map that is currently open (null if there is no open map).
    /// </summary>
    MapSettings? Settings { get; }
    
    /// <summary>
    /// The storage for the map that is currently open (null if there is no open map).
    /// </summary>
    IMapStore? Store { get; }
}

// Responsible for:
// 1. expose visible spatial objects
// 2. selection state
// 3. styling decisions or style keys
// 4. commands
// 5. viewport state

// should probably implement IProvider (or delegate to something that does)
/// <summary>
/// An implementation of a view model for <see cref="Backsight.Map.Editor.Views.MapEditorWindow"/>.
/// </summary>
public partial class MapEditorViewModel : ViewModelBase, IMapEditorViewModel
{
    /// <summary>
    /// The application model.
    /// </summary>
    private readonly IMapEditorModel _model;
    
    /// <summary>
    /// The map data for the map display.
    /// </summary>
    /// <remarks>
    /// This acts like a helper that feeds a Mapsui map control that should be present inside
    /// the map editor view. The map control should automatically pick up changes made via this
    /// instance, so it acts kind of like an inner view model.
    /// <para/>
    /// Meanwhile, the <c>MapEditorViewModel</c> class as a whole is expected to expose only those
    /// properties that the enclosing <c>MapEditorWindow</c> can bind to.
    /// </remarks>
    //[ObservableProperty]
    private readonly Mapsui.Map _mapData;

    /// <summary>
    /// The current scale of the map that is currently open (0 if there is no open map).
    /// </summary>
    private double _mapScale;
    
    //internal double MapScale => _model.Store is null ? 0 : _mapScale;

    public MapEditorViewModel() : this(new DesignMapEditorModel())
    {
    }

    public MapEditorViewModel(IMapEditorModel model)
    {
        _model = model;

        _mapData = new Mapsui.Map
        {
            BackColor = Mapsui.Styles.Color.Khaki
        };

        // Ensure the map stays in position on a mouse drag (user needs to explicitly say they want to drag)
        _mapData.Navigator.PanLock = true;

        _mapData.Navigator.ViewportChanged += OnViewportChanged;
        
    }
    
    /// <inheritdoc />
    Mapsui.Map IMapEditorViewModel.MapData => _mapData;

    /// <inheritdoc />
    MapSettings? IMapEditorViewModel.Settings => _model.Store?.Settings;
    
    /// <inheritdoc />
    IMapStore? IMapEditorViewModel.Store => _model.Store;

    /// <inheritdoc />
    SpatialType IMapEditorViewModel.GetTypesAtCurrentScale()
    {
        var result = SpatialType.None;
        var store = _model.Store;

        if (store is null)
            return result;

        // Lines and polygons are always visible
        result |= SpatialType.Line;
        result |= SpatialType.Polygon;
        
        if (_mapScale < store.Settings.PointScale)
            result |= SpatialType.Point;
        
        if (_mapScale < store.Settings.LabelScale)
            result |= SpatialType.Text;

        if (_mapScale < store.Settings.LineAnnotation.ShowScale)
            result |= SpatialType.Annotation;
        
        return result;
    }

    /// <summary>
    /// Handler for the <see cref="Navigator.ViewportChanged"/> event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>
    /// This gets called a lot on mouse wheels, and the change in map scale is often tiny.
    /// </remarks>
    private void OnViewportChanged(object sender, ViewportChangedEventArgs e)
    {
        if (_model.Store is null)
        {
            _mapScale = 0;
            return;
        }
        
        var groundRect = e.Viewport.ToExtent();
        var screenRect = e.Viewport.ToSkiaRect();

        const double inchesToMeters = 0.0254;
        var width = (screenRect.Width / 96.0) * inchesToMeters;
        _mapScale = groundRect.Width / width;
        Console.WriteLine("Viewport changed => scale: " + _mapScale);
    }

    public string? CurrentMapName
    {
        get;
        private set
        {
            if (SetProperty(ref field, value))
                OnPropertyChanged(nameof(WindowTitle));
        }
    }

    public string WindowTitle =>
        String.IsNullOrWhiteSpace(CurrentMapName) ? "Map Editor" : CurrentMapName;

    /// <inheritdoc />
    /// <exception cref="ArgumentException">Map name cannot be empty.</exception>
    public void OpenMap(string mapName)
    {
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name cannot be empty.", nameof(mapName));

        _model.OpenMap(mapName);
        var store = _model.Store ?? throw new ApplicationException("Open map has no store");
        CurrentMapName = mapName;
        
        // Update the map display by adding a new layer that corresponds to the newly opened map
        var layer = new Layer(store.Name)
        {
            DataSource = new MapProvider(this, store),
            CustomLayerRendererName = Renderer.RendererName
        };
        _mapData.Layers.Add(layer);
        _mapData.Navigator.ZoomToBox(_mapData.Extent);
    }

    /// <inheritdoc />
    public bool CloseMap()
    {
        if (CurrentMapName is null)
            return false;
        
        if (_model.RequiresSave)
        {
            // TODO: Prompt if changes need to be saved
            Console.WriteLine("Map changes were not saved");
        }
        
        // Ensure the map display no longer contains a layer corresponding to the closed map
        _mapData.Layers.Remove(x => x.Name == CurrentMapName);
        CurrentMapName = null;
        _model.CloseMap();
        return true;
    }
}