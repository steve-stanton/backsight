using Backsight.Map.Editor.ViewModels;

namespace Backsight.Map.Editor.Tools;

internal abstract class MapDisplayTool(MapEditorViewModel viewModel)
{
    protected MapEditorViewModel ViewModel { get; } = viewModel;

    internal abstract bool Start();

    private protected virtual bool Finish()
    {
        ViewModel.FinishTool();
        return true;
    }

    internal virtual void Escape()
    {
        Finish();
    }

    internal virtual void MouseDown(IPosition p, MouseButton b)
    {
    }

    internal virtual void MouseUp(IPosition p, MouseButton b)
    {
    }

    internal virtual void MouseMove(IPosition p, MouseButton b)
    {
    }
}