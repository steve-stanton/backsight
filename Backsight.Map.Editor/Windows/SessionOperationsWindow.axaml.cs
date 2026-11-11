using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class SessionOperationsWindow : DialogWindow<SessionOperationsViewModel>
{
    public SessionOperationsWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal SessionOperationsWindow(Session session)
        : base(new SessionOperationsViewModel(session))
    {
        InitializeComponent();
    }
}