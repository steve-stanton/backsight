using Backsight.Map.Editor.Models;

namespace Backsight.Map.Editor.Windows;

public partial class PreferencesWindow : DialogWindow<PreferencesViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public PreferencesWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal PreferencesWindow(IMapEditorModel model) : base(new PreferencesViewModel(model))
    {
        InitializeComponent();
    }
}