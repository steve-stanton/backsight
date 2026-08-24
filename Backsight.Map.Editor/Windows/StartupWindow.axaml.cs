namespace Backsight.Map.Editor.Windows;

public partial class StartupWindow : DialogWindow<StartupViewModel> 
{
    public StartupWindow(StartupViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}