using System;
using System.Collections.Generic;
using System.Linq;
using Backsight.Model;
using Mapsui;
using Mapsui.Extensions;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// The canvas to be used when rendering a map.
/// </summary>
/// <remarks>
/// An instance of this class will be created each time the <see cref="Renderer"/> class is asked to render a map.
/// This aims to encapsulate some of the implementation details.
/// <para/>
/// Make sure that the instance gets disposed when done (presumably via a <c>using</c> statement).
/// Instances of <see cref="SKPaint"/> are needed when doing any sort of drawing to the Skia canvas,
/// but they are "unsafe" and need to be disposed of. This class tries to avoid too much churn by
/// reusing instances during a single render cycle. 
/// </remarks>
class MapCanvas : IDisposable
{
    private readonly SKCanvas _canvas;
    private readonly Viewport _viewport;
    
    /// <summary>
    /// The height and width of rendered points (in screen units). If null, no points will be rendered.
    /// </summary>
    private readonly float? _pointSize;

    /// <summary>
    /// The distinct paint styles that have been used so far.
    /// </summary>
    private readonly Dictionary<PaintStyle, SKPaint> _paintCache = new();
    
    /// <summary>
    /// Creates a new instance of the <see cref="MapCanvas"/> class.
    /// </summary>
    /// <param name="canvas">The Skia canvas to render to.</param>
    /// <param name="viewport">The viewport for the map display.</param>
    /// <param name="pointHeight">The size for point features (in meters on the ground), or null if
    /// points should not be rendered,</param>
    internal MapCanvas(SKCanvas canvas, Viewport viewport, double? pointHeight)
    {
        _canvas = canvas;
        _viewport = viewport;
        _pointSize = pointHeight is null ? null : (float)(pointHeight.Value / viewport.Resolution);
    }

    internal MRect Extent => _viewport.ToExtent();
    
    /// <inheritdoc />
    /// <remarks>
    /// </remarks>
    public void Dispose()
    {
        Console.WriteLine($"Dispose {_paintCache.Count} styles");
        foreach (var paint in _paintCache.Values)
            paint.Dispose();
    }

    private SKPaint GetPaint(PaintStyle style)
    {
        if (_paintCache.TryGetValue(style, out var result))
            return result;

        result = style.ToPaint();
        Console.WriteLine($"style: {style}");
        _paintCache.Add(style, result);
        return result;
    }

    /// <summary>
    /// Draws a point as a square.
    /// </summary>
    /// <param name="p">The position of the point (the center of the rendered square).</param>
    /// <param name="style">The style to use for drawing the point.</param>
    internal void DrawPoint(IPosition p, PaintStyle style)
    {
        if (_pointSize is null)
            return;

        var pointOffset = 0.5f * _pointSize.Value;
        var (sx, sy) = _viewport.WorldToScreenXY(p.X, p.Y);

        _canvas.DrawRect(
            (float)sx - pointOffset,
            (float)sy - pointOffset,
            _pointSize.Value,
            _pointSize.Value,
            GetPaint(style));
    }
    
    /// <summary>
    /// Draws a line segment.
    /// </summary>
    /// <param name="start">The start of the line</param>
    /// <param name="end">The end of the line</param>
    /// <param name="style">The style to use for drawing the line.</param>
    internal void DrawLine(IPosition start, IPosition end, PaintStyle style)
    {
        var ps = _viewport.ToScreenPoint(start);
        var pe = _viewport.ToScreenPoint(end);

        _canvas.DrawLine(ps, pe, GetPaint(style));
    }
    
    /// <summary>
    /// Draws a line consisting of multiple segments.
    /// </summary>
    /// <param name="multiSeg">The positions defining the line (expected to be at least two positions)</param>
    /// <param name="style">The style to use for drawing the line.</param>
    internal void DrawPath(MultiSegmentGeometry multiSeg, PaintStyle style)
    {
        using var path = new SKPath();
        path.MoveTo(_viewport.ToScreenPoint(multiSeg.Start));

        foreach (var p in multiSeg.Data.Skip(1))
            path.LineTo(_viewport.ToScreenPoint(p));
            
        _canvas.DrawPath(path, GetPaint(style));
    }

    internal void DrawArc(ArcGeometry arc, PaintStyle style)
    {
        var rect = _viewport.ToScreenRect(arc.Circle);
        var startAngle = arc.StartBearingInRadians * MathConstants.RADTODEG - 90.0;
        var sweepAngle = arc.SweepAngleInRadians * MathConstants.RADTODEG;
        _canvas.DrawArc(rect, (float)startAngle, (float)sweepAngle, false, GetPaint(style));
    }

    internal void DrawText(TextGeometry geom, PaintStyle style)
    {
        /*
         * from old TextFeature...

        style.Render(display, m_Geom);

        if (s_DrawReferencePoints || style is HighlightStyle)
        {
            IPointGeometry p = GetPolPosition();
            if (p!=null)
            {
                Color c = style.LineColor;
                style.LineColor = Color.Gray;
                style.RenderPlus(display, p);
                style.LineColor = c;
            }
        }
         */
        
        // TODO: The text position and size seems to be different from what I got in the old editor... why?... and which one is correct?
        
        // Create the font
        var t = geom.Text;
        using var font = CreateFont(geom);
        ScaleFontToRequiredDimensions(font, t, geom);

        // The text position is the top-left corner, but DrawText wants a Y position
        // on the baseline of the text.
        // TODO: The geom.Height may not be exactly equivalent to font.Metrics.Ascent

        double topToBottomBearing = geom.Rotation.Radians + MathConstants.PI;
        var bottomLeft = BasicGeom.Polar(geom.Position, topToBottomBearing, geom.Height);
        var screenPosition = _viewport.WorldToScreen(bottomLeft.X, bottomLeft.Y);
        var skPosition = new SKPoint((float)screenPosition.X, (float)screenPosition.Y);

        try
        {
            _canvas.Save();
            _canvas.Translate(skPosition);
            _canvas.RotateDegrees((float)geom.Rotation.Degrees);

            _canvas.DrawText(t, SKPoint.Empty, SKTextAlign.Left, font, GetPaint(style));
        }
        finally
        {
            _canvas.Restore();
        }
    }

    private static SKFont CreateFont(TextGeometry textGeom)
    {
        var font = textGeom.Font;
        if (font is null)
            return new SKFont();

        var typeface = SKFontManager.Default.MatchFamily(font.TypeFace);
        return new SKFont(typeface);
    }

    private void ScaleFontToRequiredDimensions(SKFont font, string text, TextGeometry textGeom)
    {
        // Work with an arbitrary size of 100 (while documentation says this is in "points" (1/72nd of an inch),
        // AI suggests that Skia really expects pixel size). It doesn't actually matter since the required
        // dimensions are in ground units, so we can use the viewport resolution to scale things.
        font.Size = 100f;

        // How big would that make the text (in screen units)
        font.MeasureText(text, out SKRect skBounds);
        if (skBounds.Height <= 0)
            throw new NotImplementedException("SKFont.MeasureText() returned unexpected height");

        // What's that in ground units?
        var ght = skBounds.Height * _viewport.Resolution;
        var gwd = skBounds.Width * _viewport.Resolution;

        // Figure out the font size we need that will yield the required ground height
        var scaleY = textGeom.Height / ght;
        font.Size = (float)(100.0 * scaleY);

        // Assuming that the calculated size will alter the width proportionally, what width (on the ground) would we get?
        gwd *= scaleY;

        // So how much do we need to scale in X to give us the required width?
        font.ScaleX = (float)(textGeom.Width / gwd);
    }

    internal void DrawLine(LineGeometry geom, PaintStyle style)
    {
        if (geom is SectionGeometry section)
            geom = section.Make();

        if (geom is SegmentGeometry seg)
        {
            DrawLine(seg.Start, seg.End, style);
        }
        else if (geom is MultiSegmentGeometry multiSeg)
        {
            DrawPath(multiSeg, style);
        }
        else if (geom is ArcGeometry arc)
        {
            DrawArc(arc, style);
        }
    }
    
    internal void DrawPolygon(Polygon pol, double mapScale, PaintStyle style)
    {
        // While SKPath does have an ArcTo method that lets you include circular arcs in the
        // path, that tends to complicate things here - just approximate arcs on each ring
        // (since that's the way it worked in the past).

        var drawWindow = _viewport.ToExtent().ToWindow();
        var outlines = pol.GetRingOutlines(mapScale, drawWindow);

        using var path = new SKPath();

        foreach (var outline in outlines)
        {
            path.MoveTo(_viewport.ToScreenPoint(outline[0]));

            foreach (var p in outline.Skip(1))
                path.LineTo(_viewport.ToScreenPoint(p));
        }

        // The shader option isn't part of PaintStyle, so just work with an un-cached version
        using var paint = style.ToPaint();
        paint.Shader = CreateShader(paint.Color, paint.StrokeWidth);
        
        // TODO: Can the path be clipped (e.g. street polygons that go everywhere)? Is it worth it?
        _canvas.DrawPath(path, paint);
    }

    private static SKShader CreateShader(SKColor color, float strokeWidth = 1f, int tileSize = 8)
    {
        // This implementation was AI generated, but may also want to see this:
        // https://bclehmann.github.io/2022/11/05/HatchingWithSKShader/

        using var bitmap = new SKBitmap(tileSize, tileSize);
        using var tileCanvas = new SKCanvas(bitmap);

        tileCanvas.Clear(SKColors.Transparent);

        using var hatchPaint = new SKPaint
        {
            Color = color,
            StrokeWidth = strokeWidth,
            Style = SKPaintStyle.Stroke,
            IsAntialias = true
        };

        // One diagonal direction
        tileCanvas.DrawLine(0, tileSize, tileSize, 0, hatchPaint);

        // Other diagonal direction for cross-hatching
        //tileCanvas.DrawLine(0, 0, tileSize, tileSize, hatchPaint);

        return SKShader.CreateBitmap(
            bitmap,
            SKShaderTileMode.Repeat,
            SKShaderTileMode.Repeat);
    }
    
}