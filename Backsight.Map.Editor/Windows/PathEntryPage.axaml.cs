using Avalonia.Controls;

namespace Backsight.Map.Editor.Windows;

public partial class PathEntryPage : UserControl
{
    public PathEntryPage()
    {
        InitializeComponent();
    }

    internal PathEntryPage(PathEntryViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}