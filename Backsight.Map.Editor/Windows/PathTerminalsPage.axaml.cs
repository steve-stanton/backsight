using Avalonia.Controls;

namespace Backsight.Map.Editor.Windows;

internal partial class PathTerminalsPage  : UserControl
{
    public PathTerminalsPage()
    {
        InitializeComponent();
    }

    internal PathTerminalsPage(PathTerminalsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}