using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class CuldeSacWindow : DialogWindow<CuldeSacViewModel>
{
    public CuldeSacWindow()
        : this(DistanceUnit.GetUnit(DistanceUnitType.Meters))
    {
    }
    
    public CuldeSacWindow(DistanceUnit entryUnit)
        : base(new CuldeSacViewModel(entryUnit))
    {
        InitializeComponent();
    }
}