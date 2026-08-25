using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Backsight.Database;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Models;
using Backsight.Map.Editor.Tools;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Rendering.Skia.Extensions;

namespace Backsight.Map.Editor.Windows;

internal interface IMapEditorViewModel
{
    /// <summary>
    /// The identifier of the custom renderer that should be used for the map display.
    /// </summary>
    /// <seealso cref="Backsight.Map.Editor.Mapping.Renderer"/>
    string RendererName => "backsight-map-renderer";
    
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

    /// <summary>
    /// The current selection (may be empty).
    /// </summary>
    IMapSelection Selection { get; }

    /// <summary>
    /// The current data entry command (if any).
    /// </summary>
    CommandTool? CurrentCommand { get; }
    
    /// <summary>
    /// The current map scale (0 if there is no open map).
    /// </summary>
    double MapScale { get; }
}

// Responsible for:
// 1. expose visible spatial objects
// 2. selection state
// 3. styling decisions or style keys
// 4. commands
// 5. viewport state

// should probably implement IProvider (or delegate to something that does)
/// <summary>
/// An implementation of a view model for <see cref="MapEditorWindow"/>.
/// </summary>
public partial class MapEditorViewModel : ViewModelBase, IMapEditorViewModel
{
    /// <summary>
    /// The cursor to display over the map.
    /// </summary>
    [ObservableProperty]
    private Cursor _mapCursor = Cursor.Default;

    [ObservableProperty]
    private Avalonia.Controls.Controls _overlayChildren = new();
    
    /// <summary>
    /// The current map navigation tool (if any).
    /// </summary>
    private MapDisplayTool? _mapTool = null;

    /// <summary>
    /// The current data entry tool (if any).
    /// </summary>
    private CommandTool? _commandTool = null;

    /// <summary>
    /// Modeless dialog used to perform inverse calculations (null if dialog
    /// is not currently displayed).
    /// </summary>
    //private InverseWindow? _inverseCalculator = null;
    
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
    private readonly Mapsui.Map _mapData;

    /// <summary>
    /// The current scale of the map that is currently open (0 if there is no open map).
    /// </summary>
    private double _mapScale;

    /// <summary>
    /// The current selection (never null, but may be empty).
    /// </summary>
    private Selection _selection = new();

    /// <summary>
    /// Is auto-highlight enabled?
    /// </summary>
    private bool _autoSelect;

    /// <summary>
    /// Service class for displaying dialogs.
    /// </summary>
    private readonly IDialogService _dialogService;

    public MapEditorViewModel() : this(new DesignMapEditorModel(), null!)
    {
    }

    public MapEditorViewModel(IMapEditorModel model, IDialogService dialogService)
    {
        _model = model;
        _dialogService = dialogService;

        _mapData = new Mapsui.Map
        {
            BackColor = Mapsui.Styles.Color.Khaki
        };

        // Ensure the map stays in position on a mouse drag (user needs to explicitly say they want to drag)
        _mapData.Navigator.PanLock = true;

        _mapData.Navigator.ViewportChanged += OnViewportChanged;
    }
    
    
    internal IEnvironmentRepository Environment => _model.Environment;
    internal IMapEditorModel Model => _model;
    
    /// <inheritdoc />
    Mapsui.Map IMapEditorViewModel.MapData => _mapData;
    
    /// <inheritdoc />
    IMapSelection IMapEditorViewModel.Selection => _selection;
    
    /// <inheritdoc />
    double IMapEditorViewModel.MapScale => _model.Store is null ? 0 : _mapScale;

    /// <inheritdoc cref="IMapEditorViewModel.Settings" />
    public MapSettings? Settings => _model.Store?.Settings;
    
    /// <inheritdoc cref="IMapEditorViewModel.Store" />
    public IMapStore? Store => _model.Store;

    /// <inheritdoc cref="IMapEditorViewModel.GetTypesAtCurrentScale" />
    public SpatialType GetTypesAtCurrentScale()
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
        //Console.WriteLine("Viewport changed => scale: " + _mapScale);
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
        Console.WriteLine("Opening " + mapName);
        
        if (String.IsNullOrWhiteSpace(mapName))
            throw new ArgumentException("Map name cannot be empty.", nameof(mapName));

        // Ensure we've closed any map that's currently open
        CloseMap();
        
        _model.OpenMap(mapName);
        var store = _model.Store ?? throw new ApplicationException("Open map has no store");
        CurrentMapName = mapName;
        
        // Update the map display by adding a new layer that corresponds to the newly opened map
        var layer = new Layer(store.Name)
        {
            DataSource = new MapProvider(this, store),
            CustomLayerRendererName = (this as IMapEditorViewModel).RendererName
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

    /// <summary>
    /// Creates context menu items (typically in response to a right click on the map display).
    /// </summary>
    /// <returns>The items that the view should present in a context menu (an empty list if
    /// no menu should be shown).</returns>
    internal IReadOnlyList<ContextMenuItem> GetContextMenuItems()
    {
        // c.f. Backsight.Editor.MainForm.CreateContextMenu(ISpatialSelection)
        
        // Handle single-item selections...

        var singleItem = _selection.SingleOrDefault;
        if (singleItem is not null)
        {
            var t = singleItem.SpatialType;

            if (t == SpatialType.Point)
                return GetPointSelectedMenu();

            if (t == SpatialType.Line)
            {
                var items = GetLineSelectedMenu();

                var line = singleItem as LineFeature;
                if (line is null && singleItem is DividerObject d)
                    line = d.Divider.Line;

                if (line?.HasTopology == true)
                {
                    // TODO: do this inside GetLineSelectedMenu
                    var itemIndex = Array.FindIndex(items, x => x.Header == "Polygon Boundary");
                    items[itemIndex] = items[itemIndex] with { IsChecked = true };
                }

                return items;
            }

            if (t == SpatialType.Text)
                return GetTextSelectedMenu();
        }

        if (_selection.Count > 1)
            return GetMultiSelectionMenu();

        // Show the default menu, enabling the "Subdivide Polygon" item if a single polygon
        // is currently selected
        //ctxLineSubdividePolygon.Enabled = singleItem?.SpatialType == SpatialType.Polygon;
        return GetNoSelectionMenu();
    }

    private ContextMenuItem[] GetPointSelectedMenu() =>
    [
        new("Sideshot...", PointSideshotCommand),
        new("Update...", PointUpdateCommand),
        new("Delete", PointDeleteCommand),
        ContextMenuItem.Separator,
        new("Add Straight Line", LineAddStraightLineCommand),
        new("Add Circular Arc", LineAddCircularArcCommand),
        ContextMenuItem.Separator,
        new("Properties", PropertiesCommand)
    ];

    [RelayCommand(CanExecute = nameof(CanStartCommandTool))]
    private void PointNew()
    {
        Console.WriteLine(nameof(PointNew));
    }

    [RelayCommand(CanExecute = nameof(CanStartCommandTool))]
    private void PointAddOnLine()
    {
        Console.WriteLine(nameof(PointAddOnLine));
    }

    [RelayCommand(CanExecute = nameof(CanStartConnectionPath))]
    private void PointConnectionPath()
    {
        Console.WriteLine(nameof(PointConnectionPath));
    }

    private bool CanStartConnectionPath => _commandTool is null && ArePointsDrawn;

    internal bool ArePointsDrawn => GetTypesAtCurrentScale().HasFlag(SpatialType.Point);

    [RelayCommand(CanExecute = nameof(CanStartSideshot))]
    private void PointSideshot()
    {
        Console.WriteLine(nameof(PointSideshot));
    }

    private bool CanStartSideshot => _commandTool is null && _selection.SingleOrDefault is Model.PointFeature;

    [RelayCommand(CanExecute = nameof(CanStartPointUpdate))]
    private void PointUpdate()
    {
        Console.WriteLine(nameof(PointUpdate));
    }
    
    private bool CanStartPointUpdate => _commandTool is null && _selection.SingleOrDefault is Model.PointFeature;

    [RelayCommand(CanExecute = nameof(CanSetDefaultEntity))]
    private void PointDefaultEntity()
    {
        Console.WriteLine(nameof(PointDefaultEntity));
    }

    private bool CanSetDefaultEntity => Store is not null;

    [RelayCommand(CanExecute = nameof(CanStartInverseCalculator))]
    private void PointInverseCalculator()
    {
        Console.WriteLine(nameof(PointInverseCalculator));
    }

    private bool CanStartInverseCalculator => false; //_inverseCalculator is null && ArePointsDrawn;
    
    [RelayCommand(CanExecute = nameof(CanAddFeature))]
    private void LineAddStraightLine()
    {
        StartCommand(new NewLineTool(this, _selection.SingleOrDefault as Model.PointFeature));
    }

    private void StartCommand(CommandTool tool)
    {
        if (_commandTool is not null)
            throw new InvalidOperationException("Command tool already started");
        
        if (_autoSelect)
            _autoSelect = false; // TODO: Should only suspend while command is running

        _commandTool = tool;
        _commandTool.Run();
    }

    internal void ClearSelection()
    {
        _selection = new Selection();
    }
    
    private bool CanAddFeature => _commandTool is null && Store is not null;

    [RelayCommand(CanExecute = nameof(CanAddFeature))]
    private void LineAddCircularArc()
    {
        Console.WriteLine(nameof(LineAddCircularArc));
    }

    [RelayCommand(CanExecute = nameof(CanDeletePoint))]
    private void PointDelete()
    {
        Console.WriteLine(nameof(PointDelete));
    }
    
    private bool CanDeletePoint => _selection.SingleOrDefault is Model.PointFeature;
    
    private ContextMenuItem[] GetLineSelectedMenu() =>
    [
        new("Extend...", LineExtendCommand),
        new("Subdivide", LineSubdivideCommand),
        new("Subdivide (One Distance)...", LineSubdivideOneDistanceCommand),
        new("Parallel", LineParallelCommand),
        new("Update", LineUpdateCommand),
        new("Polygon Boundary", LinePolygonBoundaryCommand),
        new("Delete", LineDeleteCommand),
        new("Trim Dangle", LineTrimDangleCommand),
        ContextMenuItem.Separator,
        new("Properties", PropertiesCommand)
    ];

    [RelayCommand]
    private void LineExtend()
    {
        Console.WriteLine(nameof(LineExtend));
    }
    [RelayCommand]
    private void LineSubdivide()
    {
        Console.WriteLine(nameof(LineSubdivide));
    }
    [RelayCommand]
    private void LineSubdivideOneDistance()
    {
        Console.WriteLine(nameof(LineSubdivideOneDistance));
    }
    [RelayCommand]
    private void LineParallel()
    {
        Console.WriteLine(nameof(LineParallel));
    }
    [RelayCommand]
    private void LineUpdate()
    {
        Console.WriteLine(nameof(LineUpdate));
    }
    [RelayCommand]
    private void LinePolygonBoundary()
    {
        Console.WriteLine(nameof(LinePolygonBoundary));
    }
    [RelayCommand]
    private void LineDelete()
    {
        Console.WriteLine(nameof(LineDelete));
    }
    [RelayCommand]
    private void LineTrimDangle()
    {
        Console.WriteLine(nameof(LineTrimDangle));
    }
    
    private ContextMenuItem[] GetTextSelectedMenu() =>
    [
        new("Move", TextMoveCommand),
        new("Move Polygon Reference Position", TextMovePolygonPositionCommand),
        new("Delete", TextDeleteCommand),
        ContextMenuItem.Separator,
        new("Properties", PropertiesCommand)
    ];

    [RelayCommand]
    private void TextMove()
    {
        Console.WriteLine(nameof(TextMove));
    }

    [RelayCommand]
    private void TextMovePolygonPosition()
    {
        Console.WriteLine(nameof(TextMovePolygonPosition));
    }
    [RelayCommand]
    private void TextDelete()
    {
        Console.WriteLine(nameof(TextDelete));
    }
    
    private ContextMenuItem[] GetMultiSelectionMenu() =>
    [
        new("Delete", MultiDeleteCommand),
        new("Trim Dangles", MultiTrimCommand),
    ];

    [RelayCommand]
    private void MultiDelete()
    {
        Console.WriteLine(nameof(MultiDelete));
    }
    [RelayCommand]
    private void MultiTrim()
    {
        Console.WriteLine(nameof(MultiTrim));
    }
    
    /// <summary>
    /// Gets the context menu to display when nothing is selected.
    /// </summary>
    private ContextMenuItem[] GetNoSelectionMenu() =>
    [
        new("Overview", OverviewCommand),
        new("Zoom In", ZoomInCommand),
        new("Zoom Out", ZoomOutCommand),
        new("Zoom Rectangle", ZoomRectangleCommand),
        new("Draw Scale...", DrawScaleCommand),
        new("Pan", PanCommand),
        new("Refresh", RefreshCommand),
        ContextMenuItem.Separator,
        new("Previous", PreviousCommand),
        new("Next", NextCommand),
    ];

    [RelayCommand]
    private void Overview()
    {
        var extent = _mapData.Extent;
        if (extent is not null)
            _mapData.Navigator.ZoomToBox(extent);
    }

    [RelayCommand]
    private void ZoomIn() => Zoom(-0.2);

    [RelayCommand]
    private void ZoomOut() => Zoom(0.2);

    /// <summary>
    /// Zooms the map display in or out about the center of the current viewport.
    /// </summary>
    /// <param name="factor">The fraction of the current extent to grow by (negative to zoom in).</param>
    private void Zoom(double factor)
    {
        var nav = _mapData.Navigator;
        if (nav.Viewport.HasSize())
        {
            var extent = nav.Viewport.ToExtent();
            var newExtent = extent.Grow(factor * extent.Height);
            nav.ZoomToBox(newExtent);
        }
    }

    [RelayCommand(CanExecute = nameof(CanStartMapDisplayTool))]
    private void ZoomRectangle()
    {
        _mapTool = new ZoomRectangleMapTool(this);
        _mapTool.Start();
    }

    internal void ZoomTo(IWindow extent)
    {
        //_previousViewport = _map.Navigator.Viewport;
        _mapData.Navigator.ZoomToBox(extent.ToMRect());
    }

    [RelayCommand]
    private void DrawScale()
    {
        Console.WriteLine("Click5");
    }

    [RelayCommand(CanExecute = nameof(CanStartMapDisplayTool))]
    private void Pan()
    {
        _mapTool = new PanMapTool(this);
        _mapTool.Start();
    }
    
    private bool CanStartMapDisplayTool => _mapTool is null;
    
    private bool CanStartCommandTool => _commandTool is null;

    internal void FinishTool()
    {
        MapCursor = Cursor.Default;
        _mapTool = null;
    }

    internal void Escape()
    {
        _mapTool?.Escape();
    }
    
    [RelayCommand]
    private void Refresh()
    {
        // Refresh goes back to fetch from the provider, whereas RefreshGraphics just goes to the custom renderer
        _mapData.Refresh(ChangeType.Discrete);
    }

    [RelayCommand]
    private void Previous()
    {
        Console.WriteLine(nameof(Previous));
    }

    [RelayCommand]
    private void Next()
    {
        Console.WriteLine(nameof(Next));
    }

    [RelayCommand]
    private void Properties()
    {
        // TODO: Display the properties of the currently selected feature
    }

    [RelayCommand]
    private void DeleteSelection()
    {
        // TODO: Delete the currently selected feature
    }

    internal void OnMouseDown(IPosition p, MouseButton b, KeySelection ks)
    {
        /*
         c.f. MapControl.mapPanel_MouseDown
            // Ensure focus is with the map panel so that mouse wheel
            // events will be received
            mapPanel.Focus();

            Position p = DisplayToGround(e.Location);

            if (m_Tool==null)
                EditingController.Current.MouseDown(this, p, e.MouseButton);
            else
                m_Tool.MouseDown(p, e.MouseButton);
         */

        if (_mapTool is null)
        {
            /*
             c.f. Backsight.Editor.EditingController.MouseDown

            // If there's no command, or it doesn't handle left clicks...
            else if (m_Command is null || !m_Command.LButtonDown(p))
            {
                bool isMultiSelect = (Control.ModifierKeys & Keys.Shift) != 0;

                // If we're currently auto-highlighting, and the user is doing
                // a multi-select, turn off auto-highlight and get rid of the
                // properties window (confusing).

                // TODO: May want to keep the properties window, but disabled. In the
                // past, it was ok to close because the dialog rested on top of the
                // map. Now, closing the property window causes a redraw, which is
                // a bit unexpected in the middle of a multiselect.

                if (isMultiSelect)
                {
                    m_IsAutoSelect = 0;
                    m_Main.ClosePropertiesWindow();
                }

                if (m_Sel is null)
                    OnSelect(sender.MapScale, p, isMultiSelect);
                else
                    m_Sel.CtrlMouseDown(p);
            }
             */
            bool handledByCommand = _commandTool?.MouseDown(p, b) ?? false;
            if (!handledByCommand)
            {
                bool isMultiSelect = ks.HasFlag(KeySelection.Shift);            
                OnSelect(p, isMultiSelect);
            }
        }
        else
        {
            _mapTool.MouseDown(p, b);
        }
    }

    internal void OnMouseMove(IPosition p, MouseButton b)
    {
        /*
         c.f. EditingController.MouseMove
        
        if (m_Sel is not null) // means the CTRL key is pressed
        {
            m_Sel.CtrlMouseMoveTo(p);
        }
        else
        {
            // The main window of the cadastral editor provides the option to
            // display the current position of the mouse
            m_Main.MouseMove(sender, p, b);

            // Auto-highlight option
            if (m_IsAutoSelect > 0)
                Select(sender.MapScale, p, SpatialType.All);

            m_Command?.MouseMove(p);
        }
         */

        if (_mapTool is not null)
            _mapTool.MouseMove(p, b);
     
        // Ignore the auto-select function when any command tool is running
        if (_commandTool is not null)
            _commandTool.MouseMove(p, MouseButton.None);
        else if (AutoSelect)
            Select(_mapScale, p, SpatialType.All);
    }

    internal void OnMouseUp(IPosition p, MouseButton b)
    {
        if (_mapTool is not null)
            _mapTool.MouseUp(p, b);
    }

    /// <summary>
    /// Tries to select something at the specified position
    /// </summary>
    /// <param name="p">The position where a left-click has occurred</param>
    /// <param name="isMultiSelect">True if performing a multi-select (SHIFT key is pressed)</param>
    private void OnSelect(IPosition p, bool isMultiSelect)
    {
        // Try to select something.
        IMapObject? thing = SelectObject(_mapScale, p, SpatialType.All);

        if (thing is not null)
        {
            // Caution: If we're auto-highlighting, and the thing we've just selected is the thing that's already
            // selected, don't do ANYTHING (not even if the user is apparently doing a multi-select).

            /* TODO
             *
            // Note that if the user IS doing a multi-select, any auto-highlighting is supposed to go
            // away automatically (see OnLButtonDown && OnMouseMove).
            */
            
            if (_autoSelect && ReferenceEquals(thing, _selection.SingleOrDefault))
                return;

            if (isMultiSelect)
            {
                // Add the thing to the selection (or remove it if it's currently selected).
                AddOrRemoveFromSelection(thing);
            }
            else
            {
                SetSelection(new Selection(thing, p));
            }
        }
        else
        {
            // Ensure the selection has been unhighlighted & clear out the selection.
            if (!isMultiSelect)
                SetSelection(null);
        }

        // If we've now got a simple selection, notify any commands
        // that are running so that their stuff will draw on top
        // of the highlighting.
        //OnSelect();

        // If we are doing an inverse dialog, make sure its point
        // coloring remains regardless of what is currently selected.
        //m_Inverse?.Draw();
    }

    void AddOrRemoveFromSelection(IMapObject o)
    {
        var sel = new Selection(_selection.Items);
        if (!sel.Remove(o))
            sel.Add(o);

        SetSelection(sel);
    }
    
    private IMapObject? Select(double mapScale, IPosition p, SpatialType spatialType)
    {
        var o = SelectObject(mapScale, p, spatialType);
        
        if (o is not null)
            SetSelection(new Selection(o, p));
        else
            SetSelection(null);

        return o;
    }

    private IMapObject? SelectObject(double mapScale, IPosition p, SpatialType spatialType)
    {
        if (_model.Store is null)
            return null;
        
        var displayTypes = GetTypesAtCurrentScale();
        var findTypes = spatialType & displayTypes;

        if (findTypes == SpatialType.None)
            return null;
        
        var settings = _model.Store.Settings;
        var storeModel = _model.Store.Model;
        IMapObject? result = null;

        // Try to find a point feature if points are drawn.
        if (findTypes.HasFlag(SpatialType.Point))
        {
            ILength size = new Length(settings.PointHeight * 0.5);
            result = storeModel.QueryClosest(p, size, SpatialType.Point);
            if (result is not null)
                return result;
        }

        ILength tol = new Length(0.001 * mapScale);

        // Try to find a line, using a tolerance of 1mm at the draw scale.
        if (findTypes.HasFlag(SpatialType.Line))
        {
            result = storeModel.QueryClosest(p, tol, SpatialType.Line);
            if (result is not null)
                return result;
        }

        // Try for a text string if text is drawn.
        // The old software handles text by checking that the point is inside
        // the outline, not sure whether the new index provides acceptable alternative.
        if (findTypes.HasFlag(SpatialType.Text))
        {
            result = storeModel.QueryClosest(p, tol, SpatialType.Text);
            if (result is not null)
                return result;
        }

        // Just return if a command dialog is up,
        // since selecting a polygon is distracting at that stage
        // (really, this applies to things like intersect commands).
        // There MIGHT be cases at some later date where we really
        // do want to select pols...
        // For updates, allow polygon selection
/*
        if (IsCommandRunning && m_Command is not UpdateUI)
            return null;
*/
        if (findTypes.HasFlag(SpatialType.Polygon))
        {
            IPointGeometry pg = PointGeometry.Create(p);
            var pol = new FindPointContainerQuery(storeModel.Index, pg).Result;
            if (pol is not null)
                return pol;
        }

        return null;
    }

    /// <summary>
    /// Remembers a new selection
    /// </summary>
    /// <param name="newSel">The new selection (specify null to clear any current selection)</param>
    /// <returns>True if selection changed. False if the selection matches the current selection</returns>
    private bool SetSelection(Selection? newSel)
    {
        var ss = newSel is null ? new Selection() : newSel;
        if (_selection.IsEqual(ss))
            return false;

        _selection = ss;
        _mapData.RefreshGraphics();
        /*
        m_MapControl?.OnSelectionChanged(m_Selection);

        ISpatialObject? item = ss.SingleOrDefault;
        m_Main.SetSelection(item);

        // If a single item has been selected
        if (item is not null)
        {
            if (item is DividerObject d)
                item = d.Divider.Line;

            if (item is PointFeature selPoint)
            {
                if (ArePointsDrawn)
                {
                    m_Inverse?.OnSelectPoint(selPoint);
                    m_Command?.OnSelectPoint(selPoint);
                }
            }
            else if (item is LineFeature selLine)
            {
                if (m_Command is not null)
                {
                    m_Command.OnSelectLine(selLine);
                }
            }

            // 20100709 -- Not sure about this. If the user wants to point at the
            // same point twice in succession, the fact that the point is still
            // selected means the 2nd pointing won't get passed down (we tested
            // for a change above).

            if (m_Command is not null)
            {
                // 20101005 -- Allow highlighting of polygons, since their selection
                // should not interfere with command dialogs (and in things like the
                // update UI, it can be useful to confirm that topology is ok).

                if (item is not Polygon)
                    ClearSelection();
            }
        }

        m_HasSelectionChanged = true;
        */
        return true;
    }

    [RelayCommand]
    private async Task FileNew()
    {
        var dialog = new NewMapWindow(_model);
        var result = await _dialogService.ShowDialog(dialog);
        
        if (result == DialogResult.OK)
        {
            var mapName = dialog.ViewModel.MapName;
            Debug.Assert(mapName is not null);
            OpenMap(mapName);
        }
    }

    [RelayCommand]
    private async Task FileOpen()
    {
        var dialog = new OpenMapWindow(_model.MapRepository);
        var result = await _dialogService.ShowDialog(dialog);
        
        if (result == DialogResult.OK)
        {
            var mapName = dialog.ViewModel.SelectedMapName;
            Debug.Assert(mapName is not null);
            OpenMap(mapName);
        }
    }

    [RelayCommand]
    private async Task FileSave()
    {
        Console.WriteLine(nameof(FileSave));
    }
    
    [RelayCommand(CanExecute = "CommandNotImplemented")]
    private void FileShowChanges()
    {
        Console.WriteLine(nameof(FileShowChanges));
    }
    
    private bool CommandNotImplemented => false;
    
    [RelayCommand(CanExecute = "CommandNotImplemented")]
    private void FileStatistics()
    {
        Console.WriteLine(nameof(FileStatistics));
    }
    
    [RelayCommand]
    private void FileCoordinateSystem()
    {
        Console.WriteLine(nameof(FileCoordinateSystem));
    }
    
    [RelayCommand]
    private async Task FileCheck()
    {
        Console.WriteLine(nameof(FileCheck));
    }

    [RelayCommand]
    private void FileExit()
    {
        CloseMap();
        
        // Alternatively, raise an ExitRequested event and do this in the view
        if (Application.Current?.ApplicationLifetime
            is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.Shutdown();
        }
    }

    public IReadOnlyList<string> RecentMaps => GlobalUserSetting.RecentMaps;

    [RelayCommand]
    private void OpenRecentMap(string mapName)
    {
        OpenMap(mapName);
    }

    [RelayCommand(CanExecute = nameof(IsEditDeleteEnabled))]
    private void EditDelete()
    {
        Console.WriteLine(nameof(EditDelete));
    }

    private bool IsEditDeleteEnabled()
    {
        return _selection.Count > 0;
    }

    [RelayCommand]
    private void EditUndo()
    {
        Console.WriteLine(nameof(EditUndo));
    }

    [RelayCommand]
    private void EditRepeat()
    {
        Console.WriteLine(nameof(EditRepeat));
    }

    [RelayCommand]
    private void EditRecall()
    {
        Console.WriteLine(nameof(EditRecall));
    }

    [RelayCommand]
    private void EditOperationHistory()
    {
        Console.WriteLine(nameof(EditOperationHistory));
    }
    
    [RelayCommand]
    private void EditIdAllocations()
    {
        Console.WriteLine(nameof(EditIdAllocations));
    }

    [RelayCommand]
    private void EditAutoNumber()
    {
        Console.WriteLine(nameof(EditAutoNumber));
    }

    [RelayCommand]
    private void EditPreferences()
    {
        Console.WriteLine(nameof(EditPreferences));
    }

    [RelayCommand(CanExecute = nameof(IsEditAutoHighlightEnabled))]
    private void EditAutoHighlight()
    {
        AutoSelect = !AutoSelect;
    }

    public bool AutoSelect
    {
        get => _autoSelect;
        private set => SetProperty(ref _autoSelect, value);
    }

    private bool IsEditAutoHighlightEnabled()
    {
        return _model.Extent is not null;
    }

    internal ScreenPosition WorldToScreen(IPosition p)
    {
        return _mapData.Navigator.Viewport.WorldToScreen(p.X, p.Y);
    }
    
    internal void AbortCommand(CommandTool cmd)
    {
        if (!ReferenceEquals(cmd, _commandTool))
            throw new InvalidOperationException();

        // Make sure the normal cursor is on screen.
        MapCursor = Cursor.Default;

        /*
        cmd.ActiveMap.RestoreLastDraw();
        RedrawSelection();

        // Re-enable auto-highlighting if it was on before.
        if (m_IsAutoSelect<0)
            m_IsAutoSelect = -m_IsAutoSelect;

        cmd.ActiveMap.PaintNow();
        */
        
        cmd.Dispose();
        _commandTool = null;
    }

    internal void FinishCommand(CommandTool cmd)
    {
        if (!ReferenceEquals(cmd, _commandTool))
            throw new InvalidOperationException();
        
        // Make sure the normal cursor is on screen.
        MapCursor = Cursor.Default;

        /*
        // Refresh everything from the model. This may seem a bit of an effort, considering
        // that many edits don't do much to the display (some don't do anything). However,
        // it's fast and keeps things clean in more complex cases. Do it before saving the
        // map model, since it gives the impression that things are more responsive than
        // they actually are!
        RefreshAllDisplays();

        // Notify any check dialog (re-check all potential problems).
        // And repaint immediately to avoid flicker (icons wouldn't otherwise be repainted
        // until the idle handler gets called)
        if (m_Check is not null)
        {
            m_Check.OnFinishOp();
            ActiveMap.PaintNow();
        }

        // Re-enable auto-highlighting if it was on before.
        if (m_IsAutoSelect<0)
            m_IsAutoSelect = -m_IsAutoSelect;
*/

        cmd.Dispose();
        _commandTool = null;
    }
    
    CommandTool? IMapEditorViewModel.CurrentCommand => _commandTool;

    /// <summary>
    /// Initializes the application by ensuring that the user has selected a map (potentially
    /// a brand new map).
    /// </summary>
    /// <returns>True if the startup succeeded, false if the user did not select any map.</returns>
    internal async Task<bool> Startup()
    {
        var startup = new StartupWindow(_model, _dialogService);
        var result = await _dialogService.ShowDialog(startup);

        if (result != DialogResult.OK)
            return false;

        Debug.Assert(startup.ViewModel.MapName is not null);
        OpenMap(startup.ViewModel.MapName);
        return true;
    }
}
