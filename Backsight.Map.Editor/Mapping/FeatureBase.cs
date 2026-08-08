using System;
using System.Collections.Generic;
using Backsight.Model;
using Mapsui;
using Mapsui.Styles;
using SkiaSharp;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Minimal implementation of a <see cref="Mapsui.IFeature"/>.
/// </summary>
abstract class FeatureBase : Mapsui.IFeature
{
    private readonly MRect _extent;
    
    protected FeatureBase(IMapObject item)
    {
        _extent = item.Extent.ToMRect() ?? throw new ArgumentException("Map object has no spatial extent");
    }

    /// <summary>
    /// Renders the feature to the given canvas.
    /// </summary>
    /// <param name="canvas">The canvas to draw on.</param>
    /// <param name="style">The style properties to use for the rendering (specify null to use defaults).</param>
    protected internal abstract void Render(MapCanvas canvas, PaintStyle? style = null);
    
    public MRect? Extent => _extent;

    public object Clone()
    {
        throw new NotImplementedException();
    }

    public void CoordinateVisitor(Action<double, double, CoordinateSetter> visit)
    {
        throw new NotImplementedException();
    }
    
    public ICollection<IStyle> Styles => [];

    public object? this[string key]
    {
        get => null;
        set => throw new NotImplementedException();
    }

    public IEnumerable<string> Fields => [];
    public object? Data
    {
        get => null;
        set => throw new NotImplementedException();
    }
}