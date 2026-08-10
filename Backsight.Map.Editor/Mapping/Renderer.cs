using System;
using System.Diagnostics;
using System.Linq;
using Backsight.Map.Editor.ViewModels;
using Backsight.Model;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Rendering;
using SkiaSharp;
using PointFeature = Mapsui.Layers.PointFeature;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Custom renderer for an instance of <see cref="Mapsui.UI.Avalonia.MapControl"/>
/// </summary>
public interface IMapControlRenderer
{
    /// <summary>
    /// Performs map rendering.
    /// </summary>
    /// <param name="canvas"></param>
    /// <param name="viewport"></param>
    /// <param name="layer"></param>
    /// <param name="renderService"></param>
    void RenderMap(
        SKCanvas canvas,
        Viewport viewport,
        Mapsui.Layers.ILayer layer,
        RenderService renderService);
}

/// <summary>
/// Implementation of <see cref="IMapControlRenderer"/>.
/// </summary>
class Renderer : IMapControlRenderer
{
    private readonly IMapEditorViewModel _viewModel;
    
    public Renderer(IMapEditorViewModel viewModel)
    {
        _viewModel = viewModel;
        Mapsui.Rendering.Skia.MapRenderer.RegisterLayerRenderer(viewModel.RendererName, RenderMap);
    }
    
    public void RenderMap(SKCanvas canvas, Viewport viewport, ILayer layer, RenderService renderService)
    {
        Console.WriteLine("render " + _viewModel.CurrentMapName);
        int n = 0;

        double? pointHeight = null;
        var settings = _viewModel.Settings;
        if (settings is not null)
        {
            var drawTypes = _viewModel.GetTypesAtCurrentScale();
            if (drawTypes.HasFlag(SpatialType.Point))
                pointHeight = _viewModel.Settings?.PointHeight;
        }
            
        using var draw = new MapCanvas(canvas, viewport, pointHeight);
        
        // The implementation of GetFeatures by the Layer class (which is the only type of layer that we care about)
        // just returns what it has cached - the supplied extent and resolution are ignored.

        var extent = new MRect(0.0); // viewport.ToExtent()
        var resolution = 0.0; // viewport.Resolution
        
        //foreach (var feature in layer.GetFeatures(viewport.ToExtent(), viewport.Resolution))
        foreach (var feature in layer.GetFeatures(extent, resolution).OfType<FeatureBase>())
        {
            n++;
            feature.Render(draw);
        }

        // Draw any intersections when required
        if (pointHeight is not null && settings?.IntersectionsDrawn == true)
            RenderIntersections(draw);
        
        // Draw any selected items
        RenderSelection(draw);
        
        Console.WriteLine("rendered " + n);
    }

    private void RenderIntersections(MapCanvas canvas)
    {
        var window = canvas.Extent.ToWindow();
        var index = _viewModel.Store?.Model.Index;
        Debug.Assert(index is not null);
                
        // Render like a normal point, but with no fill
        var style = new PaintStyle
        {
            Color = SKColors.Black,
            Style = SKPaintStyle.Stroke
        };

        index.ProcessIntersections(window, (IMapObject o) =>
        {
            var x = o as Intersection;
            Debug.Assert(x is not null);
            canvas.DrawPoint(x, style);
            return true;
        });
    }

    private void RenderSelection(MapCanvas canvas)
    {
        var drawTypes = _viewModel.GetTypesAtCurrentScale();
        var arePointsDrawn = drawTypes.HasFlag(SpatialType.Point);
        
        foreach (var item in _viewModel.Selection.Items)
        {
            if (item is Model.PointFeature pt)
            {
                new Point(pt).Render(canvas, new PaintStyle
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Fill
                });
            }
            else if (item is LineFeature line)
            {
                new Line(line).Render(canvas, new PaintStyle
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 5f
                });

                if (arePointsDrawn)
                {
                    new Point(line.StartPoint).Render(canvas, new PaintStyle
                    {
                        Color = SKColors.DarkBlue,
                        Style = SKPaintStyle.Fill
                    });
                
                    new Point(line.EndPoint).Render(canvas, new PaintStyle
                    {
                        Color = SKColors.Aqua,
                        Style = SKPaintStyle.Fill
                    });
                }
            }
            else if (item is TextFeature text)
            {
                new Text(text).Render(canvas, new PaintStyle
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Fill
                });
            }
            else if (item is Polygon pol)
            {
                canvas.DrawPolygon(pol, _viewModel.MapScale, new PaintStyle
                {
                    Color = SKColors.Yellow,
                    Style = SKPaintStyle.Fill,
                    StrokeWidth = 2f
                });
            }
        }

        var section = _viewModel.Selection.LineSection;
        if (section is not null)
        {
            canvas.DrawLine(section, new PaintStyle
            {
                Color = SKColors.Yellow,
                Style = SKPaintStyle.Stroke,
                StrokeWidth = 2f
            });
        }
    }
}