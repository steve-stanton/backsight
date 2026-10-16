using Backsight.Map.Editor.Tools;
using Backsight.Model;

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

    internal LineSubdivisionWindow(LineSubdivisionTool tool, LineFeature line)
        : base(new LineSubdivisionViewModel(tool, line))
    {
        InitializeComponent();
    }
}