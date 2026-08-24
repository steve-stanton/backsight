namespace Backsight.Map.Editor.Windows;

public partial class NewMapWindow : DialogWindow<NewMapViewModel>
{
    public NewMapWindow() : base(null!)
    {
        InitializeComponent();
    }

    public NewMapWindow(NewMapViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}