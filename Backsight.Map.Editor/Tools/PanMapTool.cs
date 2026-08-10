using Avalonia.Input;
using Backsight.Map.Editor.ViewModels;

namespace Backsight.Map.Editor.Tools;

internal class PanMapTool : MapDisplayTool
{
    internal PanMapTool(MapEditorViewModel viewModel) : base(viewModel)
    {
    }

    internal override bool Start()
    {
        ViewModel.MapCursor = EditingCursors.StartPanCursor;
        (ViewModel as IMapEditorViewModel).MapData.Navigator.PanLock = false;
        return true;
    }

    private protected override void MouseDown(IPosition p, MouseButton b)
    {
        ViewModel.MapCursor = EditingCursors.HandCursor;
    }

    private protected override void MouseUp(IPosition p, MouseButton b)
    {
        ViewModel.MapCursor = EditingCursors.StartPanCursor;
    }

    private protected override bool Finish()
    {
        (ViewModel as IMapEditorViewModel).MapData.Navigator.PanLock = true;
        ViewModel.MapCursor = Cursor.Default;
        return base.Finish();   
    }
}