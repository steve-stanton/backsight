using System.Collections.ObjectModel;
using System.Linq;
using Backsight.Model;

namespace Backsight.Map.Editor.Windows;

internal partial class SessionOperationsViewModel : DialogViewModel
{
    /// <summary>
    /// Column data for an editing operation.
    /// </summary>
    /// <param name="Sequence"></param>
    /// <param name="Type"></param>
    /// <param name="FeatureCount"></param>
    /// <param name="Operation"></param>
    internal record EditItem(uint Sequence, string Type, uint FeatureCount, Operation Operation);

    /// <summary>
    /// The items to be displayed.
    /// </summary>
    internal ObservableCollection<EditItem> Edits { get; } = new();

    internal SessionOperationsViewModel(Session session)
    {
        var items = session.Operations.Select(o => new EditItem(o.EditSequence, o.Name, o.FeatureCount, o));

        foreach (var item in items.Reverse())
            Edits.Add(item);
    }
}