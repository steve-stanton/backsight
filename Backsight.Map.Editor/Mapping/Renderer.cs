using System;
using System.Diagnostics;
using System.Linq;
using Backsight.Geometry;
using Backsight.Map.Editor.ViewModels;
using Backsight.Model;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Rendering;
using Mapsui.Rendering.Skia.Functions;
using SkiaSharp;

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
    internal const string RendererName = "backsight-map-renderer";
    
    private readonly IMapEditorViewModel _viewModel;
    
    public Renderer(IMapEditorViewModel viewModel)
    {
        _viewModel = viewModel;
        Mapsui.Rendering.Skia.MapRenderer.RegisterLayerRenderer(RendererName, RenderMap);
    }
    
    public void RenderMap(SKCanvas canvas, Viewport viewport, ILayer layer, RenderService renderService)
    {
        Console.WriteLine("render " + _viewModel.CurrentMapName);
        
        // The implementation of GetFeatures by the Layer class (which is the only type of layer that we care about)
        // just returns what it has cached - the supplied extent and resolution are ignored.

        int n = 0;
        
        var extent = new MRect(0.0); // viewport.ToExtent()
        var resolution = 0.0; // viewport.Resolution

        double? pointHeight = null;
        var settings = _viewModel.Settings;
        if (settings is not null)
        {
            var drawTypes = _viewModel.GetTypesAtCurrentScale();
            if (drawTypes.HasFlag(SpatialType.Point))
                pointHeight = _viewModel.Settings?.PointHeight;
        }
            
        using var draw = new MapCanvas(canvas, viewport, pointHeight);
        
        //foreach (var feature in layer.GetFeatures(viewport.ToExtent(), viewport.Resolution))
        foreach (var feature in layer.GetFeatures(extent, resolution).OfType<FeatureBase>())
        {
            n++;
            feature.Render(draw);
        }

        // Draw any intersections when required
        if (pointHeight is not null && settings?.IntersectionsDrawn == true)
            RenderIntersections(draw);
        
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
            IsAntialias = true,
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
}