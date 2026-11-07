namespace Backsight.Map.Editor.Windows;

internal partial class PathAngleWindow : DialogWindow<PathAngleViewModel>
{
    public PathAngleWindow() : base(new PathAngleViewModel())
    {
        InitializeComponent();
    }
}