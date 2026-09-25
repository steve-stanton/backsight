using System.Diagnostics;
using System.Linq;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Tools;

internal class DeletionTool : CommandTool
{
    internal DeletionTool(MapEditorViewModel viewModel)
        : base(viewModel, EditingActionId.Deletion)
    {
    }

    internal override bool Run()
    {
        var features = ViewModel.Selection.Items.OfType<Feature>().ToArray();
        if (features.Length == 0)
            return false;
        
        var dop = new DeletionOperation(Store, features);
        dop.Execute();
        ViewModel.ClearSelection();
        ViewModel.FinishCommand(this);
        
        return true;
    }
}