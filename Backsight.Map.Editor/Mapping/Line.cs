using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

class Line : FeatureBase
{
    private static readonly PaintStyle _defaultStyle = new()
    {
        Color = SKColors.Black,
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
        canvas.DrawLine(Feature.LineGeometry, style);
    }
}