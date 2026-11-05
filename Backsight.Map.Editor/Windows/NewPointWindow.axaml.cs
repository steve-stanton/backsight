using Backsight.Model.Operations;

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

    internal NewPointWindow(MapEditorViewModel vm, NewPointOperation? recall)
        : base(new NewPointViewModel(vm, recall))
    {
        InitializeComponent();
    }
}