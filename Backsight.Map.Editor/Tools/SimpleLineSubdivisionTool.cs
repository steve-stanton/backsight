using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class SimpleLineSubdivisionTool : CommandTool
{
    private readonly SimpleLineSubdivisionWindow _dialog;

    internal SimpleLineSubdivisionTool(MapEditorViewModel viewModel, LineFeature line)
        : base(viewModel, EditingActionId.SimpleLineSubdivision)
    {
        _dialog = new SimpleLineSubdivisionWindow(this, line);
    }

    internal override bool Run()
    {
        ViewModel.Show(_dialog);
        return true;
    }

    protected override bool Finish()
    {
        var model = _dialog.ViewModel;
        var distance = model.GetDistance();
        if (distance is null)
            return false;

        /*
        // If we are doing an update, alter the original operation.
        UpdateUI? up = this.Update;

        if (up is not null)
        {
            // Get the original operation.
            SimpleLineSubdivisionOperation pop = (up.GetOp() as SimpleLineSubdivisionOperation);
            if (pop==null)
            {
                MessageBox.Show("SimpleLineSubdivisionUI.DialFinish - Unexpected edit type.");
                return false;
            }

            // Remember the changes as part of the UI object (the original edit remains
            // unchanged for now)
            UpdateItemCollection changes = pop.GetUpdateItems(m_Length, m_Dialog.IsFromEnd);
            if (!up.AddUpdate(pop, changes))
                return false;
        }
        else
        {
        */

        var op = new SimpleLineSubdivisionOperation(model.Line, distance, model.FromEnd);
        op.Execute();

        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        var model = _dialog.ViewModel;
        
        // Ensure the line that's being subdivided is still highlighted
        canvas.Highlight(model.Line);

        // Draw the split point (if any)
        var splitPos = model.SplitPosition;
        if (splitPos is not null)
        {
            var pointStyle = new PaintStyle
            {
                Color = SKColors.Magenta,
                Style = SKPaintStyle.Fill,
            };
            
            canvas.DrawPoint(splitPos, pointStyle);
        }
    }
}