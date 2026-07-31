using System;
using System.Collections.Generic;
using Backsight.Model;
using Mapsui;
using Mapsui.Styles;

namespace Backsight.Map.Editor.Mapping;

/// <summary>
/// Minimal implementation of a <see cref="Mapsui.IFeature"/>.
/// </summary>
abstract class FeatureBase : Mapsui.IFeature
{
    private readonly MRect _extent;
    
    protected FeatureBase(IMapObject item)
    {
        _extent = item.Extent.ToMRect();
    }

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