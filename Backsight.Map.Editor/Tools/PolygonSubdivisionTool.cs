using System;
using Backsight.Map.Editor.Mapping;
using Backsight.Map.Editor.Windows;
using Backsight.Model;
using Backsight.Model.Operations;
using SkiaSharp;

namespace Backsight.Map.Editor.Tools;

internal class PolygonSubdivisionTool : CommandTool
{
    private readonly IMapStore _store;
    
    /// <summary>
    /// The polygon (if any) that is currently selected.
    /// </summary>
    private Polygon? _polygon;
    
    /// <summary>
    /// The links for the currently selected polygon (null if no links could be determined).
    /// </summary>
    private PolygonSub? _links;
    
    public PolygonSubdivisionTool(MapEditorViewModel viewModel)
        : base(viewModel, EditingActionId.PolygonSubdivision)
    {
        _store = viewModel.Store ?? throw new ArgumentNullException(nameof(viewModel.Store));
    }

    internal override bool Run()
    {
        ViewModel.MapCursor = EditingCursors.PolygonSubdivideCursorNo;
        return true;
    }

    internal override void MouseMove(IPosition p, MouseButton b)
    {
        IPointGeometry pg = PointGeometry.Create(p);
        var pol = new FindPointContainerQuery(_store.Model.Index, pg).Result;

        if (!ReferenceEquals(_polygon, pol))
        {
            _polygon = pol;
            _links = null;

            if (_polygon is not null)
            {
                var links = new PolygonSub(_polygon);
                if (links.NumLink > 0)
                    _links = links; 
            }
            
            ViewModel.MapCursor = _links is null ?
                EditingCursors.PolygonSubdivideCursorNo : EditingCursors.PolygonSubdivideCursorYes;

            ViewModel.RefreshMapDisplay();
        }
    }
    
    internal override bool MouseDown(IPosition p, MouseButton b)
    {
        if (_links is null)
            Escape();
        else
            Finish();

        return true;
    }
    

    protected override bool Finish()
    {
        if (_links is null)
            return false;

        var op = new PolygonSubdivisionOperation(_store);
        op.Execute(_links);

        return base.Finish();
    }

    internal override void Render(MapCanvas canvas)
    {
        if (_links is null)
            return;

        var lineStyle = new PaintStyle
        {
            Color = SKColors.Magenta,
            Style = SKPaintStyle.Stroke,
            StrokeWidth = 2f
        };
        
        PointFeature ps, pe;
        for (int i = 0; _links.GetLink(i, out ps, out pe); i++)
            canvas.DrawLine(ps, pe, lineStyle);
    }
}