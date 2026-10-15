using System;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Tools;

internal class TrimDanglesTool : CommandTool
{
    public TrimDanglesTool(MapEditorViewModel viewModel)
        : base(viewModel, EditingActionId.Trim)
    {
    }

    internal override bool Run()
    {
        // Grab the current selection & filter out stuff that can't be trimmed
        LineFeature[] lines = TrimLineOperation.PreCheck(ViewModel.Selection.Items);
        if (lines.Length == 0)
            return false;

        var store = ViewModel.Store ?? throw new InvalidOperationException("Map store not available");
        var op = new TrimLineOperation(store);
        op.Execute(lines);
        ViewModel.ClearSelection();

        return true;
    }
}