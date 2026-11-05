using System.Linq;
using System.Collections.ObjectModel;
using Backsight.Model;
using Backsight.Model.Operations;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Backsight.Map.Editor.Windows;

internal partial class PickEditViewModel : DialogViewModel
{
    /// <summary>
    /// Column data for an editing operation.
    /// </summary>
    /// <param name="Sequence"></param>
    /// <param name="Type"></param>
    /// <param name="FeatureCount"></param>
    /// <param name="Recallable"></param>
    /// <param name="Operation"></param>
    internal record EditItem(uint Sequence, string Type, uint FeatureCount, string Recallable, Operation Operation);

    /// <summary>
    /// The items to be displayed.
    /// </summary>
    internal ObservableCollection<EditItem> Edits { get; } = new();
    
    /// <summary>
    /// The displayed item that is currently selected.
    /// </summary>
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(OkCommand))]
    private EditItem? _selectedEdit;
    
    /// <summary>
    /// The editing operation to be recalled.
    /// </summary>
    internal Operation? SelectedOperation => SelectedEdit?.Operation;

    internal PickEditViewModel(Session session)
    {
        foreach (var op in session.Edits.Reverse())
        {
            Edits.Add(new EditItem(
                op.EditSequence, op.Name, op.FeatureCount, op is IRecallable ? "Yes" : "No", op));
        }
        
        SelectedEdit = Edits.FirstOrDefault();
    }
    
    protected override bool CanExecuteOk()
    {
        return SelectedEdit?.Operation is IRecallable;
    }
}