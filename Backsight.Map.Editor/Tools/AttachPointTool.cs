using System;
using Backsight.Environment;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class AttachPointTool : CommandTool
{
    private IEntity PointType { get; }
    private bool AutoRepeat { get; }
    private IMapStore Store { get; }
    
    /// <summary>
    /// The line feature that is closest to the mouse cursor.
    /// </summary>
    private LineFeature? _line;
    
    /// <summary>
    /// The position where a point will be attached (null if no line is close to the cursor).
    /// </summary>
    private IPosition? _position;
    
    internal AttachPointTool(MapEditorViewModel viewModel, AttachPointViewModel options)
        : base(viewModel, EditingActionId.AttachPoint)
    {
        Store = viewModel.Store ?? throw new InvalidOperationException("No map store");
        PointType = options.SelectedPointType ?? throw new InvalidOperationException("No point type selected");
        AutoRepeat = options.IsAutoRepeat;
    }

    internal override bool Run()
    {
        ViewModel.MapCursor = EditingCursors.AttachPointCursor;
        return true;
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        var hadPosition = _position is not null;
        
        // The ground tolerance is 1mm at the draw scale.
        var scale = (ViewModel as IMapEditorViewModel).MapScale;
        ILength tol = new Length(0.001 * scale);

        // Ask the map to find the closest line (if any)
        _line = Store.Model.QueryClosest(p, tol, SpatialType.Line) as LineFeature;
        _position = _line?.LineGeometry.GetClosest(PointGeometry.Create(p), tol);

        // Refresh if the position is defined (or was, but is no longer). I was going to do this
        // only if the line changes, but that means the preview position tends to remain unchanged
        // even when you move the mouse a little)
        if (_position is not null || hadPosition)
            ViewModel.RefreshMapDisplay();
    }

    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        // Just finish if we don't have a selected line
        if (_line is null)
            return Finish();

        // Get the position on the selected line that is closest to the supplied position.
        // This should be pretty close to the _position that was evaluated on the last mouse move.
        var scale = (ViewModel as IMapEditorViewModel).MapScale;
        ILength tol = new Length(0.001 * scale);
        IPosition? pos = _line.LineGeometry.GetClosest(PointGeometry.Create(p), tol);
        
        // It's conceivable that there's no result if the user was moving the mouse real fast.
        // Just ignore the mouse click (not sure if it's worth giving the user some sort of feedback,
        // since they should see the lack of a new point soon enough)
        if (pos is null)
            return true;

        // Attach a new point to the line
        uint pr = AttachPointOperation.GetPositionRatio(_line, pos);
        var op = new AttachPointOperation(_line, pr);
        var ff = new FeatureFactory(op)
        {
            PointType = PointType
        };
        op.Execute(ff);

        // Exit the command if we're not supposed to repeat
        if (AutoRepeat)
        {
            // Ensure the draw includes the extra point (perhaps a bit of overkill to
            // draw just one point).
            ViewModel.RefreshMapDisplay();
            return true;
        }

        return Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        if (_line is not null)
            canvas.Highlight(_line);

        if (_position is not null)
        {
            canvas.DrawPoint(_position, new PaintStyle
            {
                Color = SKColors.Magenta,
                Style = SKPaintStyle.Fill
            });
        }
    }
}