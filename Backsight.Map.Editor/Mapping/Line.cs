using System;
using Backsight.Model;
using Backsight.Model.Operations;
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
        if (altStyle is null)
        {
            var style = Feature.IsTopological ? _defaultStyle : new PaintStyle
            {
                Color = SKColors.Turquoise,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f
            };

            var geom = Feature.LineGeometry;
            if (geom is ArcGeometry arc && arc.IsCircle)
                canvas.DrawCircle(arc.Circle, style with { Dashed = true, StrokeWidth = 4f });
            else
                canvas.DrawLine(geom, style);            
        }
        else
        {
            canvas.DrawLine(Feature.LineGeometry, altStyle.Value);
        }
    }
}