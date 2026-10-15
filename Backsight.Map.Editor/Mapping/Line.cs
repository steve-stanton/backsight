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
            if (geom is ArcGeometry { IsCircle: true } arc)
            {
                canvas.DrawCircle(arc.Circle, style with { Dashed = true, StrokeWidth = 4f });
            }
            else
            {
                // If we're dealing with a line that's been divided into at least two sections,
                // and the line is marked as trimmed, ensure we only render the non-dangling sections.
                RenderLine(canvas, style);
            }
        }
        else
        {
            canvas.DrawLine(Feature.LineGeometry, altStyle.Value);
        }
    }

    private void RenderLine(MapCanvas canvas, PaintStyle style)
    {
        if (Feature is { IsTrimmed: true, Topology: SectionTopologyList sectionList })
        {
            foreach (var divider in sectionList)
            {
                if (sectionList.IsVisible(divider))
                    canvas.DrawLine(divider.LineGeometry, style);
            }
        }
        else
        {
            canvas.DrawLine(Feature.LineGeometry, style);
        }
    }
}