using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class PickEditWindow : DialogWindow<PickEditViewModel>
{
    public PickEditWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal PickEditWindow(Session session) : base(new PickEditViewModel(session))
    {
        InitializeComponent();
    }
}