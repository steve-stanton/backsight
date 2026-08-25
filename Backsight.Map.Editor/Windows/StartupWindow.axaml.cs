using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.Windows;

public partial class StartupWindow : DialogWindow<StartupViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    private StartupWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal StartupWindow(IMapEditorModel model, IDialogService dialogService)
        : base(new StartupViewModel(model, dialogService))
    {
        InitializeComponent();
    }
}