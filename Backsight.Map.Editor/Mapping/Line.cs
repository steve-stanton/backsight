using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

class Line : FeatureBase
{
    private static readonly PaintStyle _defaultStyle = new()
    {
        Color = SKColors.Black,
        IsAntialias = true,
        Style = SKPaintStyle.Stroke,
        StrokeWidth = 1f
    };

    private LineFeature Feature { get; }
    
    internal Line(LineFeature line) : base(line)
    {
        Feature = line;
    }

    protected internal override void Render(MapCanvas canvas, PaintStyle? altStyle = null)
    {
        var style = altStyle ?? _defaultStyle;
        var geom = Feature.LineGeometry;
        if (geom is SectionGeometry section)
            geom = section.Make();

        if (geom is SegmentGeometry seg)
        {
            canvas.DrawLine(seg.Start, seg.End, style);
        }
        else if (geom is MultiSegmentGeometry multiSeg)
        {
            canvas.DrawPath(multiSeg, style);
        }
        else if (geom is ArcGeometry arc)
        {
            canvas.DrawArc(arc, style);
        }
    }
}