using Backsight.Editor.Forms;

namespace Backsight.Editor.Map;

abstract class MapWindowTool : ISpatialDisplayTool
{
    private readonly MapWindow _mapWindow;

    protected MapWindow MapWindow => _mapWindow;

    protected MapWindowTool(MapWindow mapWindow)
    {
        _mapWindow = mapWindow;
    }

    public abstract int Id { get; }
    public abstract bool Start();

    public virtual bool Finish()
    {
        _mapWindow.Finish(this);
        return true;
    }

    public virtual void MouseDown(IPosition p, MouseButton b)
    {
    }

    public virtual void MouseUp(IPosition p, MouseButton b)
    {
    }

    public virtual void MouseMove(IPosition p, MouseButton b)
    {
    }

    public virtual void Escape()
    {
        _mapWindow.Escape(this);
    }
}