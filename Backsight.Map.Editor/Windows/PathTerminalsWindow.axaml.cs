using Backsight.Map.Editor.Tools;

namespace Backsight.Map.Editor.Windows;

internal partial class PathTerminalsWindow  : DialogWindow<PathTerminalsViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public PathTerminalsWindow() : base(null!)
    {
        InitializeComponent();
    }
    
    internal PathTerminalsWindow(ConnectionPathTool tool)
        : base(new PathTerminalsViewModel(tool))
    {
        InitializeComponent();
    }
}