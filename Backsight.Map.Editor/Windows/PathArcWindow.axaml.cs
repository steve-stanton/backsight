using Avalonia.Input;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class PathArcWindow : DialogWindow<PathArcViewModel>
{
    public PathArcWindow()
        : this(DistanceUnit.GetUnit(DistanceUnitType.Meters))
    {
    }
    
    public PathArcWindow(DistanceUnit entryUnit) : base(new PathArcViewModel(entryUnit))
    {
        InitializeComponent();
    }
}