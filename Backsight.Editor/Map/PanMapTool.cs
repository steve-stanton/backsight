using Backsight.Editor.Forms;

namespace Backsight.Editor.Map;

class PanMapTool : MapWindowTool
{
    internal PanMapTool(MapWindow mapWindow) : base(mapWindow)
    {
    }

    public override int Id => (int)DisplayToolId.Pan;

    public override bool Start()
    {
        MapWindow.SetCursor(EditingCursors.StartPanCursor);
        MapWindow.PanLock = false;
        return true;
    }

    public override void MouseDown(IPosition p, MouseButton b)
    {
        MapWindow.SetCursor(EditingCursors.PanCursor);
    }

    public override void MouseUp(IPosition p, MouseButton b)
    {
        MapWindow.SetCursor(EditingCursors.StartPanCursor);
    }

    public override bool Finish()
    {
        MapWindow.PanLock = true;
        return base.Finish();   
    }

    public override void Escape()
    {
        MapWindow.PanLock = true;
        base.Escape();
    }
}