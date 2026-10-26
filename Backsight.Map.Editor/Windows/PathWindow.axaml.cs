using Backsight.Map.Editor.Tools;

namespace Backsight.Map.Editor.Windows;

internal partial class PathWindow  : DialogWindow<PathViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public PathWindow() : base(null!)
    {
        InitializeComponent();
    }
    
    internal PathWindow(ConnectionPathTool tool)
        : base(new PathViewModel(tool))
    {
        InitializeComponent();
    }
}