using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Backsight.Editor.Forms;
using Backsight.Editor.UI;
using RepoDb;

namespace Backsight.Editor.Map;

class NewLineUIa : NewLineUI
{
    private readonly MapWindow _mapWindow;
    private readonly Avalonia.Controls.Shapes.Line _line;
    
    internal NewLineUIa(MapWindow mapWindow, IUserAction action, PointFeature? start)
        : base(null, action, start)
    {
        _mapWindow = mapWindow;

        _line = new Avalonia.Controls.Shapes.Line
        {
            Stroke = Brushes.Magenta,
            StrokeThickness = 2,
        };
    }

    internal override void SetCommandCursor()
    {
        _mapWindow.SetCursor(EditingCursors.PenCursor);
        _mapWindow.MapOverlay.Children.Add(_line);
    }

    internal override bool PerformsPainting => false;

    internal override void Paint(PointFeature point)
    {
        throw new NotImplementedException(nameof(Paint));
    }

    internal virtual void ErasePainting()
    {
        //_mapWindow.MapOverlay.Children.Clear();
    }

    internal override void MouseMove(IPosition p)
    {
        base.MouseMove(p);

        var geom = GetIntersectGeometry();

        if (geom is not null)
        {
            var (p1x, p1y) = _mapWindow.WorldToScreen(geom.Start);
            var (p2x, p2y) = _mapWindow.WorldToScreen(geom.End);
            
            _line.StartPoint = new Avalonia.Point(p1x, p1y);
            _line.EndPoint = new Avalonia.Point(p2x, p2y);
            _line.InvalidateVisual();

            EditingController ec = EditingController.Current;
            if (ec.Project.Settings.AreIntersectionsDrawn && ArePointsDrawn() && AddingTopology())
            {
                var xf = new IntersectionFinder(geom, false);
                var points = new List<Rectangle>();

                foreach (var x in xf.Intersections.SelectMany(x => x.Intersections))
                {
                    points.Add(CreateRectangle(x.P1));
                }

                var oldPoints = _mapWindow.MapOverlay.Children.OfType<Rectangle>().ToList();
                _mapWindow.MapOverlay.Children.RemoveAll(oldPoints);
                _mapWindow.MapOverlay.Children.AddRange(points);
                _mapWindow.MapOverlay.InvalidateVisual();
            }
        }
    }

    Rectangle CreateRectangle(IPosition p)
    {
        var offset = EditingController.Current.PointHeight.Meters * 0.5;
        var topLeft = new Position(p.X - offset, p.Y + offset);
        var bottomRight = new Position(p.X + offset, p.Y - offset);
        
        var (left, top) = _mapWindow.WorldToScreen(topLeft);
        var (right, bottom) = _mapWindow.WorldToScreen(bottomRight);

        return new Rectangle
        {
            Width = right - left,
            Height = bottom - top,
            Fill = Brushes.Transparent,
            Stroke = Brushes.Magenta,
            StrokeThickness = 2.0,
            RenderTransform = new TranslateTransform(left, top),
        };
    }

    public override void Dispose()
    {
        _mapWindow.MapOverlay.Children.Clear();
        base.Dispose();
    }
}