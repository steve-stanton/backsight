using Backsight.Map.Editor.Tools;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

public partial class RadialWindow : DialogWindow<RadialViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public RadialWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal RadialWindow(RadialTool tool, PointFeature fromPoint)
        : base(new RadialViewModel(tool, fromPoint))
    {
        InitializeComponent();
    }
}