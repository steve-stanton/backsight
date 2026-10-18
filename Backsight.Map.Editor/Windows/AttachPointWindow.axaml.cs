namespace Backsight.Map.Editor.Windows;

public partial class AttachPointWindow : DialogWindow<AttachPointViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public AttachPointWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal AttachPointWindow(MapEditorViewModel vm)
        : base(new AttachPointViewModel(vm))
    {
        InitializeComponent();
    }
}