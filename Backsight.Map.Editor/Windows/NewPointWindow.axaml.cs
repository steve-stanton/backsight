using Backsight.Map.Editor.Tools;

namespace Backsight.Map.Editor.Windows;

internal partial class NewPointWindow : DialogWindow<NewPointViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public NewPointWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal NewPointWindow(MapEditorViewModel vm)
        : base(new NewPointViewModel(vm))
    {
        InitializeComponent();
    }
}