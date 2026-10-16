using System;
using System.Collections.Generic;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;
using SKPaintStyle = SkiaSharp.SKPaintStyle;

namespace Backsight.Map.Editor.Tools;

internal class LineSubdivisionTool : CommandTool
{
    private readonly LineSubdivisionWindow _dialog;
    
    internal LineSubdivisionTool(MapEditorViewModel viewModel, LineFeature line)
        : base(viewModel, EditingActionId.LineSubdivision)
    {
        _dialog = new LineSubdivisionWindow(this, line);
    }

    internal override bool Run()
    {
        ViewModel.Show(_dialog);
        return true;
    }

    protected override bool Finish()
    {
        var model = _dialog.ViewModel;
        
        /*
        UpdateUI? up = this.Update;
        bool doAbort = false;

        if (up is not null)
        {
            // Get the original operation.
            LineSubdivisionOperation pop = (up.GetOp() as LineSubdivisionOperation);
            if (pop == null)
            {
                MessageBox.Show("LineSubdivisionUI.DialFinish - Unexpected edit type.");
                return false;
            }

            // Ensure we've got the edit for the primary face
            if (!pop.IsPrimaryFace)
            {
                pop = pop.OtherSide;
                Debug.Assert(pop != null);
                Debug.Assert(pop.IsPrimaryFace);
            }

            // Package up any changes to the primary face
            UpdateItemCollection changes = pop.GetUpdateItems(m_UpDial.WorkingFace1);
            if (changes.Count > 0)
                up.AddUpdate(pop, changes);

            // If a second face has just been defined, append a new edit.
            LineSubdivisionFace face2 = m_UpDial.WorkingFace2;
            if (face2 != null)
            {
                if (pop.OtherSide == null)
                    AddOtherSide(pop, face2);
                else
                {
                    changes = pop.OtherSide.GetUpdateItems(face2);
                    if (changes.Count > 0)
                        up.AddUpdate(pop.OtherSide, changes);
                }
            }

            // If all we've done is add a new face, UpdateUI.AddUpdate wouldn't have been
            // called, which will later lead to an exception from UpdateUI.FinishCommand (via
            // the FinishCommand done below). So remember to abort in that case.
            doAbort = (up.HasRevisions == false);
        }
        else
        {
        */

        // Get the observed distances (ordered from the start of the line) 
        var distances = model.GetDistances();
        if (distances.Length < 2)
            throw new InvalidOperationException("At least two distances must be specified.");

        var op = new LineSubdivisionOperation(model.Line, distances, false);
        op.Execute();

        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        var style = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Fill
        };

        // Draw the points (except for the last one, which should
        // correspond with either the start or the end of the line
        // we are subdividing).

        var positions = GetPositions();
        for (int i = 0; i < positions.Count - 1; i++)
            canvas.DrawPoint(positions[i], style);
    }

    /// <summary>
    /// Works out the subdivision positions.
    /// </summary>
    private List<IPosition> GetPositions()
    {
        var result = new List<IPosition>();
        
        // Return if there is nothing to add (we need at least 2 distances).
        var model = _dialog.ViewModel;
        var distances = model.GetDistances();
        if (distances.Length < 2)
            return result;

        // Get adjusted lengths for each section (on the map projection, not the ground)
        double[] lens = LineSubdivisionFace.GetAdjustedLengths(model.Line, distances);
        double tot = 0.0;
        
        foreach (double d in lens)
        {
            // The total SHOULD add up exactly to the line length, but there could be some
            // rounding error near the end. In that case, GetPosition should return the
            // position at the end of the line.
            
            tot += d;
            model.Line.LineGeometry.GetPosition(new Length(tot), out IPosition p);
            result.Add(p);
        }

        return result;
    }
}