using System.Collections.Generic;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class HistoryWindow : DialogWindow<HistoryViewModel>
{
    public HistoryWindow() : base(null!)
    {
        InitializeComponent();
    }

    internal HistoryWindow(IEnumerable<Session> sessions) : base(new HistoryViewModel(sessions))
    {
        InitializeComponent();
    }
}