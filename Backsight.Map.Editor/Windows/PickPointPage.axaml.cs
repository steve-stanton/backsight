using Avalonia.Controls;
using Avalonia.Media;

namespace Backsight.Map.Editor.Windows;

internal partial class PickPointPage : UserControl
{
    public PickPointPage()
        : this(new PickPointViewModel(null!, Colors.Black, "Pick point"))
    {
    }

    internal PickPointPage(PickPointViewModel parentModel)
    {
        InitializeComponent();
        DataContext = parentModel;
    }
}