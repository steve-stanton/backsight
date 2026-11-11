using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Backsight.Model;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Backsight.Map.Editor.Windows;

internal partial class HistoryViewModel : DialogViewModel
{
    /// <summary>
    /// Column data for an editing session.
    /// </summary>
    /// <param name="StartTime"></param>
    /// <param name="FinishTime"></param>
    /// <param name="User"></param>
    /// <param name="EditCount"></param>
    /// <param name="Session"></param>
    internal record SessionItem(DateTime StartTime, DateTime? FinishTime, string User, int EditCount, Session Session);

    /// <summary>
    /// The items to be displayed.
    /// </summary>
    internal ObservableCollection<SessionItem> Sessions { get; } = new();
    
    [ObservableProperty]
    private SessionItem? _selectedSession;
    
    internal HistoryViewModel(IEnumerable<Session> sessions)
    {
        var items = sessions.Select(s =>
            new SessionItem(s.StartTime, s.EndTime, s.User, s.OperationCount, s)).ToArray();
        
        foreach (var session in items.Reverse())
            Sessions.Add(session);
        
        SelectedSession = Sessions.FirstOrDefault();
    }

    [RelayCommand]
    private async Task Details()
    {
        Debug.Assert(DialogService is not null);
        
        var item = SelectedSession;
        if (item is null)
            return;

        var dialog = new SessionOperationsWindow(item.Session);
        await DialogService.ShowDialog(dialog);
    }
}