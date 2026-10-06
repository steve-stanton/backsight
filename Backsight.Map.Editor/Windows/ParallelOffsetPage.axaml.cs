using Avalonia.Controls;

namespace Backsight.Map.Editor.Windows;

public partial class ParallelOffsetPage : UserControl
{
    public ParallelOffsetPage()
    {
        InitializeComponent();
    }
    
    public ParallelOffsetPage(ParallelOffsetViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}