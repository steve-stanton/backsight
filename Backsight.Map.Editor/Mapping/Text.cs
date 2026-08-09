using Backsight.Model;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

class Text : FeatureBase
{
    private static readonly PaintStyle _defaultStyle = new()
    {
        Color = SKColors.Black,
        Style = SKPaintStyle.Fill
    };

    private TextFeature Feature { get; }
    
    internal Text(TextFeature text) : base(text)
    {
        Feature = text;
    }

    protected internal override void Render(MapCanvas canvas, PaintStyle? altStyle = null)
    {
        canvas.DrawText(Feature.TextGeometry, altStyle ?? _defaultStyle);
    }
}