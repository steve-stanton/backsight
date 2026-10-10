using Backsight.Map.Editor.Tools;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class SimpleLineSubdivisionWindow : DialogWindow<SimpleLineSubdivisionViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public SimpleLineSubdivisionWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal SimpleLineSubdivisionWindow(SimpleLineSubdivisionTool tool, LineFeature line)
        : base(new SimpleLineSubdivisionViewModel(tool, line))
    {
        InitializeComponent();
    }
}