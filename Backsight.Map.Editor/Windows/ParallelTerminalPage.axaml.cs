using Avalonia.Controls;

namespace Backsight.Map.Editor.Windows;

public partial class ParallelTerminalPage : UserControl
{
    public ParallelTerminalPage()
    {
        InitializeComponent();
    }

    public ParallelTerminalPage(ParallelTerminalViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}