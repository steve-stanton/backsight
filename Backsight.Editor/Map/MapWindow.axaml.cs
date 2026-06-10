using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Threading;
using Backsight.Environment;
using Backsight.Forms;
using Backsight.Geometry;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Logging;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia;
using Mapsui.Rendering.Skia.Extensions;
using Mapsui.Widgets;
using Mapsui.Widgets.InfoWidgets;
using SkiaSharp;

namespace Backsight.Editor.Map;

public partial class MapWindow : Avalonia.Controls.Window
{
    private readonly CadastralMapProvider _provider = new();
    private readonly Mapsui.Map _map;

    /// <summary>
    /// The size to draw point features (in pixels).
    /// </summary>
    private float _pointSize = 10f;
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

    public MapWindow()
    {
        InitializeComponent();
        DataContext = this;

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
        LoggingWidget.ShowLoggingInMap = ActiveMode.Yes;
        Logger.Settings.LogMapEvents = true;
        Logger.Settings.LogWidgetEvents = true;
        
        // The PerformanceWidget is created as part of the map.
        var performanceWidget = _map.Widgets.OfType<PerformanceWidget>().First();
        // The default is ActiveMode.OnlyInDebugMode, which is usually the best option.
        performanceWidget.Performance.IsActive = ActiveMode.Yes;
        performanceWidget.BackColor = Mapsui.Styles.Color.WhiteSmoke;
        performanceWidget.Opacity = 1;

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
        
        _map.Navigator.ViewportChanged += NavigatorOnViewportChanged;
        //_map.Tapped += MapOnTapped;
        
        MapControl.Map = _map;
        MapControl.PointerPressed += OnPointerPressed;

        var extent = EditingController.Current.ActiveMap?.Extent;
        if (extent is not null)
        {
            var msExtent = new MRect(extent.Min.X, extent.Min.Y, extent.Max.X, extent.Max.Y);
            _map.Navigator.ZoomToBox(msExtent);
        }
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

        _pointSize = (float)(EditingController.Current.PointHeight.Meters / e.Viewport.Resolution);

        var groundRect = e.Viewport.ToExtent();
        var screenRect = e.Viewport.ToSkiaRect();
        
        const double inchesToMeters = 0.0254;
        var width = (screenRect.Width/96.0) * inchesToMeters;
        var scale = groundRect.Width / width;
        Console.WriteLine($"Scale={scale}");

        _provider.MapScale = scale;
        
        // The viewport.Width adds 10% all round, whereas ToExtent and ToSkiaRect appear to be tight fitting
        // Either way, you end up with the same map scale
    }

    // Custom layer renderer
    void DrawMap(SKCanvas canvas, Viewport viewport, Mapsui.Layers.ILayer layer, RenderService renderService)
    {
         var sel = EditingController.Current.Selection;
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
        foreach (var pol in EditingController.Current.Selection.Items.Where(x => x is Polygon).Cast<Polygon>())
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

        Console.WriteLine("Draw " + n);
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
        // AI suggests that Skia really expects pixel size)
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
            Dispatcher.UIThread.Post(() =>
            {
                MapControl.ContextFlyout.ShowAt(MapControl);
            });

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

            var ec = EditingController.Current;
            ec.Select(_provider.MapScale, p, SpatialType.All);

            // Refresh goes back to fetch from the provider, RefreshGraphics just goes to the custom renderer
            //e.Map.Refresh();
            _map.RefreshGraphics();
        }
        e.Handled = true;

    }

    private void OnClick1(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Click1");
    }

    private void OnClick2(object? sender, RoutedEventArgs e)
    {
        Console.WriteLine("Click2");
    }
}