using Backsight.Editor.Forms;

namespace Backsight.Editor.Map;

class NewCenterMapTool : MapWindowTool
{
    internal NewCenterMapTool(MapWindow mapWindow) : base(mapWindow)
    {
    }

    public override int Id => (int)DisplayToolId.NewCentre;

    public override bool Start()
    {
        MapWindow.SetCursor(EditingCursors.NewCenterCursor);
        return true;
    }

    public override void MouseDown(IPosition p, MouseButton b)
    {
        MapWindow.SetCenter(p);
        Finish();
    }
}