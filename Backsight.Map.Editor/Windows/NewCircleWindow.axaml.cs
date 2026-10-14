using Backsight.Map.Editor.Tools;

namespace Backsight.Map.Editor.Windows;

internal partial class NewCircleWindow : DialogWindow<NewCircleViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    public NewCircleWindow() : base(null!)
    {
        InitializeComponent();
    }
    
    internal NewCircleWindow(NewCircleTool tool) : base(new NewCircleViewModel(tool))
    {
        InitializeComponent();
    }
}