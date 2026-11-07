using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class PathDistanceWindow : DialogWindow<PathDistanceViewModel>
{
    public PathDistanceWindow()
        : this(DistanceUnit.GetUnit(DistanceUnitType.Meters))
    {
    }

    public PathDistanceWindow(DistanceUnit entryUnit) : base(new PathDistanceViewModel(entryUnit))
    {
        InitializeComponent();
    }
}