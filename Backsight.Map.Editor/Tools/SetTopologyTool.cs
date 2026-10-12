using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;

namespace Backsight.Map.Editor.Tools;

internal class SetTopologyTool : CommandTool
{
    private readonly LineFeature _line;
    
    public SetTopologyTool(MapEditorViewModel viewModel, LineFeature line)
        : base(viewModel, EditingActionId.SetTopology)
    {
        _line = line;
    }

    internal override bool Run()
    {
        var op = new SetTopologyOperation(_line);
        op.Execute();
        Finish();
        return true;
    }
}