namespace Backsight.Map.Editor.Windows;

internal partial class PathDistanceWindow : DialogWindow<PathDistanceViewModel>
{
    public PathDistanceWindow() : base(new PathDistanceViewModel())
    {
        InitializeComponent();
    }
}