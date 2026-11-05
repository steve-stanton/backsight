using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Windows;

internal partial class LineSubdivisionWindow : DialogWindow<LineSubdivisionViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public LineSubdivisionWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal LineSubdivisionWindow(LineSubdivisionTool tool, LineFeature line, LineSubdivisionOperation? recall)
        : base(new LineSubdivisionViewModel(tool, line, recall))
    {
        InitializeComponent();
    }
}