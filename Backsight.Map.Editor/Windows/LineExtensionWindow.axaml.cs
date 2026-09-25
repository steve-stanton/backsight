using Backsight.Map.Editor.Tools;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

public partial class LineExtensionWindow : DialogWindow<LineExtensionViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public LineExtensionWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal LineExtensionWindow(LineExtensionTool tool, LineFeature extendLine)
        : base(new LineExtensionViewModel(tool, extendLine))
    {
        InitializeComponent();
    }
}