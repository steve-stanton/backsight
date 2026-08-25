using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

public partial class OpenMapWindow : DialogWindow<OpenMapViewModel>
{
    /// <summary>
    /// Design-time constructor.
    /// </summary>
    private OpenMapWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal OpenMapWindow(IMapRepository mapRepository)
        : base(new OpenMapViewModel(mapRepository))
    {
        InitializeComponent();
    }
}
