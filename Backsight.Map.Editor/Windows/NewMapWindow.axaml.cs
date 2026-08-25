using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.Windows;

public partial class NewMapWindow : DialogWindow<NewMapViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    private NewMapWindow() : base(null!)
    {
        InitializeComponent();
    }

    public NewMapWindow(IMapEditorModel model) : base(new NewMapViewModel(model))
    {
        InitializeComponent();
    }
}