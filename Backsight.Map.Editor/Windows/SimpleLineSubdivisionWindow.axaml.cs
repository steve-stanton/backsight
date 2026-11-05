using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Operations;

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

    internal SimpleLineSubdivisionWindow(
        SimpleLineSubdivisionTool tool, LineFeature line, SimpleLineSubdivisionOperation? recall)
        : base(new SimpleLineSubdivisionViewModel(tool, line, recall))
    {
        InitializeComponent();
    }
}