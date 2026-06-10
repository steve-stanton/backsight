using Mapsui;
using Mapsui.Styles;

namespace Backsight.Editor.Map;

abstract class FeatureBase : Mapsui.IFeature
{
    private readonly MRect _extent;
    
    protected FeatureBase(ISpatialObject item)
    {
        var window = item.Extent;
        _extent = new MRect(window.Min.X, window.Min.Y, window.Max.X, window.Max.Y);
    }

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
    public MRect? Extent => _extent;
    public object? Data
    {
        get => null;
        set => throw new NotImplementedException();
    }
}