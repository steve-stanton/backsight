namespace Backsight.Map.Editor.Windows;

public partial class OpenMapWindow : DialogWindow<OpenMapViewModel>
{
    public OpenMapWindow() : this(null!)
    {
    }

    public OpenMapWindow(OpenMapViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
