using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Backsight.Editor.Forms;
using Backsight.Geometry;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Manipulations;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.Extensions;
using SkiaSharp;
using ContextMenu = Avalonia.Controls.ContextMenu;
using Cursor = Avalonia.Input.Cursor;
using KeyEventArgs = Avalonia.Input.KeyEventArgs;
using MenuItem = Avalonia.Controls.MenuItem;

namespace Backsight.Editor.Map;

public partial class MapWindow : Avalonia.Controls.Window
{
    private readonly EditingController _controller;
    private readonly CadastralMapProvider _provider = new();
    private readonly Mapsui.Map _map;

    /// <summary>
    /// The viewport prior to execution of a display tool (null if there is no active display tool).
    /// TODO: should now be obs
    /// </summary>
    private Viewport? _previousViewport;

    /// <summary>
    /// The ID of the last display tool (if any).
    /// TODO: Is this needed?
    /// </summary>
    private DisplayToolId? _mapToolId;

    /// <summary>
    /// A long-lived display tool (involving the cursor). 
    /// </summary>
    private MapWindowTool? _mapTool;
    
    /// <summary>
    /// History of explicit user-initiated draws.
    /// </summary>
    /// <remarks>
    /// This should exclude draws done while mouse wheeling (a record of the draw should
    /// only get appended after mouse wheeling has stopped).
    /// </remarks>
    private readonly DrawHistory _drawHistory = new();
    
    /// <summary>
    /// The size to draw point features (in pixels).
    /// </summary>
    private float _pointSize = 10f;

    private readonly DispatcherTimer _mouseWheelStoppedTimer;
    private bool _isMouseWheeling;

    /*
    public static readonly StyledProperty<bool> IsContextVisibleProperty =
        AvaloniaProperty.Register<MapEditor, bool>(
            nameof(IsContextVisible),
            defaultValue: false);

    public bool IsContextVisible
    {
        get => GetValue(IsContextVisibleProperty);
        set => SetValue(IsContextVisibleProperty, value);
    }
    */
    public bool IsContextVisible { get; set; }

    //SKFontManager FontManager { get; } = SKFontManager.Default;

    internal MapWindow(EditingController controller)
    {
        InitializeComponent();
        DataContext = this;

        // Using a DispatcherTimer because it runs on the UI thread
        _mouseWheelStoppedTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(1000)
        };

        _mouseWheelStoppedTimer.Tick += (_, _) =>
        {
            _mouseWheelStoppedTimer.Stop();

            if (_isMouseWheeling)
            {
                _isMouseWheeling = false;
                OnMouseWheelStopped();
            }
        };
        
        //new OverlayLayer()
        //new AdornerLayer();
        //VisualLayerManager.
/*
        AddHandler(
            KeyDownEvent,
            OnKeyDown,
            RoutingStrategies.Tunnel | RoutingStrategies.Bubble,
            handledEventsToo: true);
*/
        _controller = controller;
        _controller.SetMapWindow(this);
        Closing += (_, _) => _controller.SetMapWindow(null);

        _map = new Mapsui.Map()
        {
            BackColor = Mapsui.Styles.Color.Khaki
        };

        var layer = new Mapsui.Layers.Layer("Map")
        {
            DataSource = _provider,
            CustomLayerRendererName = "backsight-renderer"
        };
        _map.Layers.Add(layer);
        MapRenderer.RegisterLayerRenderer("backsight-renderer", DrawMap);

        // Logging to map window
        /*
        LoggingWidget.ShowLoggingInMap = ActiveMode.Yes;
        Logger.Settings.LogMapEvents = true;
        Logger.Settings.LogWidgetEvents = true;
        */

        /*
        // The PerformanceWidget is created as part of the map.
        var performanceWidget = _map.Widgets.OfType<PerformanceWidget>().First();
        // The default is ActiveMode.OnlyInDebugMode, which is usually the best option.
        performanceWidget.Performance.IsActive = ActiveMode.Yes;
        performanceWidget.BackColor = Mapsui.Styles.Color.WhiteSmoke;
        performanceWidget.Opacity = 1;
        */

        /*
        // Try a custom widget
        MapRenderer.RegisterWidgetRenderer(typeof(TestWidget), new TestWidgetSkiaRenderer());

        var testWidget = new TestWidget
        {
            VerticalAlignment = Mapsui.Widgets.VerticalAlignment.Top,
            HorizontalAlignment = Mapsui.Widgets.HorizontalAlignment.Left
        };
        _map.Widgets.Add(testWidget);

        // Hide it
        testWidget.Enabled = false;
        */

        // Ensure the map stays in position on a mouse drag (user needs to explicitly say they want to drag)
        _map.Navigator.PanLock = true;

        _provider.FetchWindowChanged += OnFetchWindowChanged;
        _map.Navigator.ViewportChanged += NavigatorOnViewportChanged;

        //_map.Tapped += MapOnTapped;

        MapControl.Map = _map;
        MapControl.PointerPressed += OnPointerPressed;
        MapControl.PointerReleased += OnPointerReleased;
        MapControl.PointerWheelChanged += OnPointerWheelChanged;
        MapControl.PointerMoved += OnPointerMoved;

        // The default is false. When you zoom out, the exposed area is briefly blank.
        // When true, the provider gets asked to do a fetch on each mousewheel increment.
        //MapControl.UseContinuousMouseWheelZoom = true;
        
        KeyDown += OnKeyDown;
        
        var extent = GetCurrentExtent();
        if (extent is not null)
            _map.Navigator.ZoomToBox(extent);
    }

    private void OnMouseWheelStopped()
    {
        Console.WriteLine("Mouse wheel stopped");
        
        // Record the current viewport
        // The MapScale should have been set by the NavigatorOnViewportChanged handler 
        var v = _map.Navigator.Viewport;
        var drawInfo = new DrawInfo(v.CenterX, v.CenterY, _provider.MapScale);
        _drawHistory.AddDraw(drawInfo);
    }

    private void OnPointerWheelChanged(object? sender, PointerWheelEventArgs e)
    {
        _isMouseWheeling = true;
        _mouseWheelStoppedTimer.Stop();
        _mouseWheelStoppedTimer.Start();
    }

    private void OnFetchWindowChanged(object? sender, FetchWindowChangedEventArgs e)
    {
        var c = e.NewExtent.Center;
        if (c is null)
            return;
        
        // Do nothing when mouse wheeling (we'll record the viewport extent via OnMouseWheelStopped)
        if (_isMouseWheeling)
        {
            Console.WriteLine("Ignoring fetch window change while mouse wheeling");
            return;
        }

        // If the user has went back to an old draw, then IsNextEnabled will be true - so we shouldn't
        // record the same draw history again.
        // TODO: But what if the user went to an old draw, then did a ZoomIn or ZoomOut? In that case,
        // we DO want to append to the history. Probably need to set something when the user initiates
        // a map display action (though mouse wheels should be handled already)
 
        if (_drawHistory.IsNextEnabled)
        {
            Console.WriteLine("Ignoring fetch window change because it's an old draw");
        }
        else
        {
            Console.WriteLine("adding draw history");
            var drawInfo = new DrawInfo(c.X, c.Y, e.MapScale);
            _drawHistory.AddDraw(drawInfo);
        }
        /*
        if (_mapToolId is null)
            Console.WriteLine("FetchExtentChanged with fetch count " + _provider.FetchCount);
        else
            Console.WriteLine($"{_mapToolId}: FetchExtentChanged with fetch count " + _provider.FetchCount);
            */
    }

    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        // TODO: The Esc key doesn't work in a WinForms+Avalonia app (but it does in a pure Avalonia app)
        //const Key escapeKey = Key.Escape;
        const Key escapeKey = Key.LeftCtrl;
        
        Console.WriteLine($"MapWindow Key={e.Key} {e.KeyModifiers}");

        if (e.KeyModifiers == KeyModifiers.Alt)
        {
            bool redraw = false;
            
            if (e.Key == Key.Left)
                redraw = _drawHistory.SetPrevious();
            else if (e.Key == Key.Right)
                redraw = _drawHistory.SetNext();

            if (redraw)
                DrawExtent();
        }

        if (e.Key == escapeKey && _mapTool is not null)
            _mapTool.Escape();
    }

    private MRect? GetCurrentExtent()
    {
        var extent = _controller.ActiveMap.Extent;
        if (extent is null)
            return null;

        return new MRect(extent.Min.X, extent.Min.Y, extent.Max.X, extent.Max.Y);
    }

    // This gets called a lot on mouse wheels, and the change in map scale is often tiny...
    // is there a way to ignore fetch requests in that case? We could either tell the provider
    // that it's ok to ignore fetch requests (or the provider itself could listen for
    // viewport changes)
    private void NavigatorOnViewportChanged(object sender, ViewportChangedEventArgs e)
    {
        // The viewport resolution should == width of the viewport in ground units / width in pixels
        // ...so is the viewport resolution the same as the map scale denominator??... no, I would need
        // to know the physical size of a pixel in meters to get that, which may be difficult to obtain
        // (depending on the platform)
        // assume 1 pixel = 0.28mm (approx 90 DPI)

        _pointSize = (float)(_controller.PointHeight.Meters / e.Viewport.Resolution);

        var groundRect = e.Viewport.ToExtent();
        var screenRect = e.Viewport.ToSkiaRect();

        const double inchesToMeters = 0.0254;
        var width = (screenRect.Width / 96.0) * inchesToMeters;
        var scale = groundRect.Width / width;
        //Console.WriteLine($"Scale={scale}");

        _provider.MapScale = scale;

        // The viewport.Width adds 10% all round, whereas ToExtent and ToSkiaRect appear to be tight fitting
        // Either way, you end up with the same map scale

        if (_mapToolId is not null)
        {
            // The viewport changes MANY times during a pan. If you move just a little, the MapControl
            // doesn't reach out to the provider to do a new fetch - presumably because the original
            // fetch included a small buffer around the viewport. But if you pan a lot, the exposed
            // map area remains blank until you release the mouse - and it does a fetch at that time.
            
            // ...The fetch happens sometime after the viewport has been changed, so I need to listen
            // for something like a FetchExtentChanged event.
            
            //Console.WriteLine($"{_mapToolId}: viewport changed with fetch count " + _provider.FetchCount);
        }
        
        // If the change event was in response to a display request, remember it in the draw history
        if (_mapToolId is not null && _previousViewport is not null && e.PreviousViewport == _previousViewport)
        {
            Console.WriteLine($"{_mapToolId} done");
/*
            if (_mapToolId is not (DisplayToolId.MapRefresh or DisplayToolId.Next or DisplayToolId.Previous))
            {
                Console.WriteLine($"Adding draw to history with scale {scale:f1}");
                var drawInfo = new DrawInfo(e.Viewport.CenterX, e.Viewport.CenterY, scale);
                _drawHistory.AddDraw(drawInfo);
            }
*/
            _previousViewport = null;
            _mapToolId = null; // we could be in the middle of a pan
        }
    }

    // Custom layer renderer
    void DrawMap(SKCanvas canvas, Viewport viewport, Mapsui.Layers.ILayer layer, RenderService renderService)
    {
        var sel = _controller.Selection;
        var selIds = new HashSet<uint>(sel.Items
            .Where(x => x is Feature)
            .Cast<Feature>()
            .Select(x => x.InternalId.ItemSequence));

        int n = 0;

        // Draws happen much more frequently than fetches from the provider. So it wouldn't be a good
        // idea to generate NTS geometries here. Probably better to have a CadastralMapLayer class
        // that caches IFeature for the whole map, with refresh on completion of each edit.

        // Given that the maps are likely small, could just have a list and cycle the complete
        // list doing a bounding box check on each. If that sounds too easy, could think about
        // using an index like that used by GeoJsonProvider

        // ...but do we even have to attach NTS geometry to each IFeature? If I set IFeature.Data
        // to refer to the Backsight feature, I could pull that out here and render it more directly
        // (e.g. circular arcs could be drawn without having to generate a LineString approximation).

        SKPoint ToScreenPoint(IPosition p)
        {
            var (sx, sy) = viewport.WorldToScreenXY(p.X, p.Y);
            return new SKPoint((float)sx, (float)sy);
        }

        SKRect ToScreenRect(ArcGeometry arc)
        {
            var cx = CircleGeometry.GetExtent(arc.Circle);
            var (left, bottom) = viewport.WorldToScreenXY(cx.Min.X, cx.Min.Y);
            var (right, top) = viewport.WorldToScreenXY(cx.Max.X, cx.Max.Y);
            return new SKRect((float)left, (float)top, (float)right, (float)bottom);
        }

        var pointOffset = _pointSize * 0.5f;

        // Layer.GetFeatures just returns what it has cached (the supplied extent and resolution seem to be ignored)
        // Layer.FetchAsync retrieves from the provider and saves it as the cache - it's called from Layer.FetchAsync
        foreach (var feature in layer.GetFeatures(viewport.ToExtent(), viewport.Resolution))
        {
            n++;

            if (feature is Map.Point mapPoint)
            {
                var p = mapPoint.PointFeature;
                var (sx, sy) = viewport.WorldToScreenXY(p.X, p.Y);
                var pid = p.InternalId.ItemSequence;

                using var paint = new SKPaint
                {
                    Color = selIds.Contains(pid) ? SKColors.Red : SKColors.Black,
                    IsAntialias = true
                };

                canvas.DrawRect((float)sx - pointOffset, (float)sy - pointOffset, _pointSize, _pointSize, paint);
            }
            else if (feature is Map.Line mapLine)
            {
                var line = mapLine.LineFeature;
                var isSelected = selIds.Contains(line.InternalId.ItemSequence);
                var color = isSelected ? SKColors.Red : SKColors.Black;
                var width = isSelected ? 5f : 1f;

                var geom = line.LineGeometry;
                if (geom is SectionGeometry section)
                    geom = section.Make();

                if (geom is SegmentGeometry seg)
                {
                    var ps = ToScreenPoint(seg.Start);
                    var pe = ToScreenPoint(seg.End);
                    using var paint = new SKPaint
                    {
                        Color = color,
                        IsAntialias = true,
                        StrokeWidth = width
                    };

                    canvas.DrawLine(ps, pe, paint);
                }
                else if (geom is MultiSegmentGeometry multiSeg)
                {
                    using var path = new SKPath();
                    path.MoveTo(ToScreenPoint(multiSeg.Start));

                    foreach (var p in multiSeg.Data.Skip(1))
                        path.LineTo(ToScreenPoint(p));

                    using var paint = new SKPaint
                    {
                        Color = color,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = width
                    };

                    canvas.DrawPath(path, paint);
                }
                else if (geom is ArcGeometry arc)
                {
                    using var paint = new SKPaint
                    {
                        Color = color,
                        IsAntialias = true,
                        Style = SKPaintStyle.Stroke,
                        StrokeWidth = width
                    };

                    var rect = ToScreenRect(arc);
                    var startAngle = arc.StartBearingInRadians * MathConstants.RADTODEG - 90.0;
                    var sweepAngle = arc.SweepAngleInRadians * MathConstants.RADTODEG;
                    canvas.DrawArc(rect, (float)startAngle, (float)sweepAngle, false, paint);
                }
            }
            else if (feature is Map.Text mapText)
            {
                var text = mapText.TextFeature;
                var isSelected = selIds.Contains(text.InternalId.ItemSequence);
                var color = isSelected ? SKColors.Red : SKColors.Black;

                using var paint = new SKPaint { Color = color, IsAntialias = true };

                // Create the font
                var geom = text.TextGeometry;
                var t = geom.Text;
                using var font = CreateFont(geom);
                ScaleFontToRequiredDimensions(font, t, geom, viewport);

                // The text position is the top-left corner, but DrawText wants a Y position
                // on the baseline of the text.
                // TODO: The geom.Height may not be exactly equivalent to font.Metrics.Ascent

                double topToBottomBearing = geom.Rotation.Radians + MathConstants.PI;
                var bottomLeft = Geom.Polar(geom.Position, topToBottomBearing, geom.Height);
                var screenPosition = viewport.WorldToScreen(bottomLeft.X, bottomLeft.Y);
                var skPosition = new SKPoint((float)screenPosition.X, (float)screenPosition.Y);

                try
                {
                    canvas.Save();
                    canvas.Translate(skPosition);
                    canvas.RotateDegrees((float)geom.Rotation.Degrees);
                    canvas.DrawText(t, SKPoint.Empty, SKTextAlign.Left, font, paint);
                }
                finally
                {
                    canvas.Restore();
                }
            }
        }

        // Should have extension method for this...
        var drawExtent = viewport.ToExtent();
        var drawWindow = new Window(drawExtent.MinX, drawExtent.MinY, drawExtent.MaxX, drawExtent.MaxY);

        // Highlight any selected polygons
        foreach (var pol in _controller.Selection.Items.Where(x => x is Polygon).Cast<Polygon>())
        {
            // While SKPath does have an ArcTo method that lets you include circular arcs in the
            // path, that tends to complicate things here - just approximate arcs on each ring
            // (since that's the way it worked in the past).

            var outlines = pol.GetRingOutlines(_provider.MapScale, drawWindow);

            using var path = new SKPath();

            foreach (var outline in outlines)
            {
                path.MoveTo(ToScreenPoint(outline[0]));

                foreach (var p in outline.Skip(1))
                    path.LineTo(ToScreenPoint(p));
            }

            using var paint = new SKPaint
            {
                Color = SKColors.LightSalmon,
                IsAntialias = true,
                Style = SKPaintStyle.Fill
            };

            // TODO: Can the path be clipped (e.g. street polygons that go everywhere)? Is it worth it?
            canvas.DrawPath(path, paint);
        }

        //Console.WriteLine("Draw " + n);
    }

    private static SKFont CreateFont(TextGeometry textGeom)
    {
        var font = textGeom.Font;
        if (font is null)
            return new SKFont();

        var typeface = SKFontManager.Default.MatchFamily(font.TypeFace);
        return new SKFont(typeface);
    }

    private void ScaleFontToRequiredDimensions(SKFont font, string text, TextGeometry textGeom, Viewport viewport)
    {
        // Work with an arbitrary size of 100 (while documentation says this is in "points" (1/72nd of an inch),
        // AI suggests that Skia really expects pixel size). It doesn't actually matter since the required
        // dimensions are in ground units, so we can use the viewport resolution to scale things.
        font.Size = 100f;

        // How big would that make the text (in screen units)
        var skBounds = new SKRect();
        font.MeasureText(text, out skBounds);
        if (skBounds.Height <= 0)
            throw new NotImplementedException("SKFont.MeasureText() returned unexpected height");

        // What's that in ground units?
        var ght = skBounds.Height * viewport.Resolution;
        var gwd = skBounds.Width * viewport.Resolution;

        // Figure out the font size we need that will yield the required ground height
        var scaleY = textGeom.Height / ght;
        font.Size = (float)(100.0 * scaleY);

        // Assuming that the calculated size will alter the width proportionally, what width (on the ground) would we get?
        gwd *= scaleY;

        // So how much do we need to scale in X to give us the required width?
        font.ScaleX = (float)(textGeom.Width / gwd);
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.Properties.IsRightButtonPressed)
        {
            Console.WriteLine("Right click");
            /*
            var test = _map.Widgets.Where(x => x is TestWidget).Cast<TestWidget>().FirstOrDefault();
            if (test is not null)
            {
                test.Enabled = !test.Enabled;
                var screenPosition = e.GetPosition(MapControl);
                var p = _map.Navigator.Viewport.ScreenToWorld(screenPosition.X, screenPosition.Y);
                test.Position = p;
                _map.RefreshGraphics();
            }
            */

            //MapControl.ContextMenu?.Open(MapControl);
            //MapControl.ContextFlyout?.ShowAt(MapControl);
            //IsContextVisible = !IsContextVisible;

            MapControl.ContextMenu = new ContextMenu()
            {
                Items = { new MenuItem { Header = "Command 1x" }, new MenuItem { Header = "Command 2x" } }
            };
            //MapControl.ContextMenu.Open(MapControl);

            MapControl.ContextFlyout = new MenuFlyout
            {
                Items = { new MenuItem { Header = "Command 1" }, new MenuItem { Header = "Command 2" } }
            };
            Dispatcher.UIThread.Post(() => { MapControl.ContextFlyout.ShowAt(MapControl); });

/*
            if (MapControl.ContextMenu is { } menu)
            {
                menu.PlacementTarget = MapControl;
                menu.Open(MapControl);
            }
            */
            /*
            var nItem = MapControl.ContextMenu?.ItemCount ?? 0;
            Console.WriteLine("nItem=" + nItem);
            //MapControl.ContextMenu?.Open();
            if (nItem > 0)
            {
                Dispatcher.UIThread.Post(() =>
                {
                    MapControl.ContextMenu?.Open(MapControl);
                });
            }
            */
        }
        else
        {
            /*
            // Do nothing if we've just clicked on the TestWidget... do this better
            var test = _map.Widgets.Where(x => x is TestWidget).Cast<TestWidget>().FirstOrDefault();
            if (test?.Enabled == true)
                return;
*/
            var screenPosition = e.GetPosition(MapControl);
            var (gx, gy) = _map.Navigator.Viewport.ScreenToWorldXY(screenPosition.X, screenPosition.Y);
            var p = new Position(gx, gy);

            if (_mapTool is not null)
            {
                _mapTool.MouseDown(p, MouseButton.Left);
            }
            else
            {
                //EditingController.Current.MouseDown(this, p, MouseButton.Left);
                
                _controller.Select(_provider.MapScale, p, SpatialType.All);

                // Refresh goes back to fetch from the provider, RefreshGraphics just goes to the custom renderer
                //e.Map.Refresh();
                _map.RefreshGraphics();
            }
        }

        e.Handled = true;

    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_mapTool is not null)
        {
            var screenPosition = e.GetPosition(MapControl);
            var (gx, gy) = _map.Navigator.Viewport.ScreenToWorldXY(screenPosition.X, screenPosition.Y);
            var p = new Position(gx, gy);
            _mapTool.MouseUp(p, MouseButton.Left);
        }
    }

    private void OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (_mapTool is not null)
        {
            var screenPosition = e.GetPosition(MapControl);
            var (gx, gy) = _map.Navigator.Viewport.ScreenToWorldXY(screenPosition.X, screenPosition.Y);
            var p = new Position(gx, gy);
            _mapTool.MouseMove(p, MouseButton.Left);
        }
    }

    private void OnClick1(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Click1");
    }

    private void OnClick2(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Click2");
    }

    private bool ZoomIn()
    {
        return Zoom(-0.2);
    }

    private bool ZoomOut()
    {
        return Zoom(0.2);
    }

    private bool Zoom(double factor)
    {
        //var extent = _map.Navigator.Viewport.ToExtent();
        var extent = _controller.ActiveMap.Extent;
        if (extent is null)
            return false;

        var newExtent = new Window(extent);
        newExtent.Expand(factor);

        _previousViewport = _map.Navigator.Viewport;
        _map.Navigator.ZoomToBox(ToMRect(newExtent));
        return true;
    }

    bool ZoomRectangle()
    {
        //	If we are currently auto-highlighting, temporarily disable
        //	for the duration of the zoom, and ensure that any currently
        //	highlighted features are drawn normally.
        //if ( m_AutoHighlight>0 ) m_AutoHighlight = -m_AutoHighlight;
        //m_Sel.RemoveSel();
        
        _mapTool = new ZoomRectangleMapTool(this);
        return _mapTool.Start();
    }    
    // TODO: Probably better as extension method
    private static MRect ToMRect(IWindow extent)
    {
        return new MRect(extent.Min.X, extent.Min.Y, extent.Max.X, extent.Max.Y);
    }

    private bool NewCenter()
    {
        _mapTool = new NewCenterMapTool(this);
        return _mapTool.Start();
    }
    
    internal void SetCursor(Cursor cursor)
    {
        // Ensure we have focus, since we may need to recognize a key
        // stroke (the ESC key) to subsequently cancel the current display tool.
        //Focusable = true;
        //Dispatcher.UIThread.Post(() => Focus());
        
        //MapControl.Focus();

        Cursor = cursor;
    }

    /// <summary>
    /// Redraws at a new center point.
    /// </summary>
    /// <param name="p">The position for the new center.</param>
    public void SetCenter(IPosition p)
    {
        var draw = new DrawInfo(p.X, p.Y, _provider.MapScale);
        SetCenterAndScale(draw);
    }

    public void Finish(ISpatialDisplayTool tool)
    {
        // Don't clear _displayToolId until NavigatorOnViewportChanged 
        //Debug.Assert(_mapToolId is not null && tool.Id == (int)_mapToolId);
        //_displayToolId = null;
        //Cursor = Cursor.Default;
        Escape(tool);
    }

    public void Escape(ISpatialDisplayTool tool)
    {
        if (ReferenceEquals(_mapTool, tool))
        {
            Cursor = Cursor.Default;
            _mapToolId = null;
            _mapTool = null;
        }
    }

    bool Pan()
    {
        _mapTool = new PanMapTool(this);
        return _mapTool.Start();
    }

    internal bool PanLock
    {
        get => _map.Navigator.PanLock;
        set => _map.Navigator.PanLock = value;
    }

    private bool MapRefresh()
    {
        _map.Refresh();
        return true;
    }

    private bool Previous()
    {
        if (_drawHistory.SetPrevious())
            DrawExtent();

        return true;
    }

    private bool Next()
    {
        if (_drawHistory.SetNext())
            DrawExtent();

        return true;
    }

    /// <summary>
    /// Redraws the current draw extent.
    /// </summary>
    private void DrawExtent()
    {
        var info = _drawHistory.GetCurrentDraw();
        if (info is null)
            return;

        SetCenterAndScale(info.Value);
    }

    private void SetCenterAndScale(DrawInfo info)
    {
        var xc = info.CenterX;
        var yc = info.CenterY;
        var scale = info.MapScale;
        
        // Get the screen dimensions of the client area, in meters
        var extent = _map.Navigator.Viewport.ToExtent();
        var width = extent.Width / _provider.MapScale;
        var height = extent.Height / _provider.MapScale;

        // Figure out the ground dimension based on the supplied scale.
        var dx = 0.5 * (width * scale);
        var dy = 0.5 * (height * scale);

        // Define a window based on the supplied centre.
        _previousViewport = _map.Navigator.Viewport;
        var newExtent = new MRect(xc - dx, yc - dy, xc + dx, yc + dy);
        _map.Navigator.ZoomToBox(newExtent);
    }

    internal void ZoomTo(IWindow extent)
    {
        _previousViewport = _map.Navigator.Viewport;
        _map.Navigator.ZoomToBox(ToMRect(extent));
    }

    private void EscapeCurrentTool()
    {
        if (_mapTool is not null)
        {
            _mapTool.Escape();
            _mapTool = null;
        }

        _mapToolId = null;
    }
    
    internal bool Do(DisplayToolId id)
    {
        EscapeCurrentTool();
        _mapToolId = id;

        switch (id)
        {
            case DisplayToolId.Overview:
            {
                var extent = _provider.GetExtent();
                if (extent is null)
                    return false;

                _previousViewport = _map.Navigator.Viewport;
                _map.Navigator.ZoomToBox(extent);
                return true;
            }

            case DisplayToolId.ZoomIn:
                return ZoomIn();

            case DisplayToolId.ZoomOut:
                return ZoomOut();

            case DisplayToolId.ZoomRectangle:
                return ZoomRectangle();

            case DisplayToolId.DrawScale:
                return false; // DrawScale();

            case DisplayToolId.NewCentre:
                return NewCenter();

            case DisplayToolId.Pan:
                return Pan();

            case DisplayToolId.MapRefresh:
                return MapRefresh();

            case DisplayToolId.Previous:
                return Previous();

            case DisplayToolId.Next:
                return Next();
        }

        return false;
    }
    
    internal ScreenPosition WorldToScreen(IPosition p)
    {
        return _map.Navigator.Viewport.WorldToScreen(p.X, p.Y);
    }
}