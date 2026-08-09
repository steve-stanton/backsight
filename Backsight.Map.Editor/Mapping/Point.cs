using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

internal class Point : FeatureBase
{
    private static readonly PaintStyle _defaultStyle = new()
    {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill
    };
    
    private PointFeature Feature { get; }
    
    internal Point(PointFeature point) : base(point)
    {
        Feature = point;
    }

    protected internal override void Render(MapCanvas canvas, PaintStyle? altStyle = null)
    {
        canvas.DrawPoint(Feature.Geometry, altStyle ?? _defaultStyle);
    }
}