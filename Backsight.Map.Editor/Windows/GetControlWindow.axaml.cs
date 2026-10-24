using Avalonia.Interactivity;

namespace Backsight.Map.Editor.Windows;

/// <summary>
/// Dialog to add control points to a map.
/// </summary>
internal partial class GetControlWindow : DialogWindow<GetControlViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public GetControlWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal GetControlWindow(MapEditorViewModel vm) : base(new GetControlViewModel(vm))
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        AreaCombo.IsDropDownOpen = true;
        base.OnLoaded(e);
    }
}