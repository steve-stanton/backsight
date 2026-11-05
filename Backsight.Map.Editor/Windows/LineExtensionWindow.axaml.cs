using Backsight.Map.Editor.Tools;
using Backsight.Model;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Windows;

internal partial class LineExtensionWindow : DialogWindow<LineExtensionViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public LineExtensionWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal LineExtensionWindow(LineExtensionTool tool, LineFeature extendLine, LineExtensionOperation? recall)
        : base(new LineExtensionViewModel(tool, extendLine, recall))
    {
        InitializeComponent();
    }
}