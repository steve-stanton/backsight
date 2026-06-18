using Mapsui;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;

namespace Backsight.Editor.Map;

public class CadastralMapProvider : IProvider
{
    internal double MapScale { get; set; } = 1000.0;
    internal long FetchCount { get; private set; }

    internal event EventHandler<FetchWindowChangedEventArgs>? FetchWindowChanged; 
    
    private Window _lastWindow = new();

    /// <summary>
    /// The tolerance to use when approximating circular arcs (in meters on the ground).
    /// </summary>
    //private double _arcTolerance = 1.0;
    
    public MRect? GetExtent()
    {
        var extent = CadastralMapModel.Current.Extent;
        if (extent.IsEmpty)
            return null;
        
        return new MRect(extent.Min.X, extent.Min.Y, extent.Max.X, extent.Max.Y);
    }

    // Not sure if this is viable... it seems that this gets called each time I click on the map
    // Could perhaps cache the result in case the FetchInfo hasn't changed?... but also need
    // to know if the original model has changed since the last fetch.
    // Perhaps consider implementing IDynamicProvider??
    // Also see GeoJsonProvider, which has a static spatial index on a collection of IFeature
    public Task<IEnumerable<Mapsui.IFeature>> GetFeaturesAsync(FetchInfo fetchInfo)
    {
        var ec = EditingController.Current;
        
        var extent = new Window(
            new Position(fetchInfo.Extent.MinX, fetchInfo.Extent.MinY),
            new Position(fetchInfo.Extent.MaxX, fetchInfo.Extent.MaxY));

        FetchCount++;
        if (extent.Equals(_lastWindow))
        {
            //Console.WriteLine("Fetch same as last");
        }
        else
        {
            FetchWindowChanged?.Invoke(this, new FetchWindowChangedEventArgs(extent, MapScale));
            Console.WriteLine("Fetch " + FetchCount);
        }
        
        _lastWindow = extent;
        
        var result = new List<Mapsui.IFeature>();
        var lineStyle = new VectorStyle()
        {
            Line = new Pen(Color.Black),
            Fill = null,
            Outline = null
        };
        var lineStyles = new List<IStyle>();
        lineStyles.Add(lineStyle);

        // There seems to be no reason to fetch polygons. If I click inside a polygon to select it, the
        // app should use the usual selection logic (the Mapsui control doesn't do selections by itself).
        // We only really need to render selected polygons, which will be done as part of the
        // custom renderer.

        CadastralMapModel.Current.Index.QueryWindow(extent, SpatialType.Line, item =>
        {
            var line = item as LineFeature ?? throw new ApplicationException("Unexpected feature type");
            result.Add(new Map.Line(line));
            
            /*
            var geom = line.LineGeometry;
            if (geom is SectionGeometry section)
                geom = section.Make();

            NetTopologySuite.Geometries.Geometry? ntsGeom = null;

            if (geom is SegmentGeometry seg)
            {
                var ps = new Coordinate(seg.Start.X, seg.Start.Y);
                var pe = new Coordinate(seg.End.X, seg.End.Y);
                ntsGeom = new LineString([ps, pe]);
            }
            else if (geom is MultiSegmentGeometry multiSeg)
            {
                var coords = multiSeg.Data.Select(p => new Coordinate(p.X, p.Y));
                ntsGeom = new LineString(coords.ToArray());
            }
            else if (geom is ArcGeometry arc)
            {
                // Draw an approximation of the curve (such that the chord-to-circumference
                // distance does not exceed 0.2mm at draw scale).
                var tol = new Length(MapScale * 0.0002);
                IPointGeometry[] pts = CircularArcGeometry.GetApproximation(arc, tol);
                var coords = pts.Select(p => new Coordinate(p.X, p.Y));
                ntsGeom = new LineString(coords.ToArray());
                //Console.WriteLine($"Approximated arc: {pts.Length} points");
            }
            
            if (ntsGeom is not null)
                result.Add(new Mapsui.Nts.GeometryFeature(ntsGeom)
                {
                    Data = line,
                    Styles = lineStyles
                });

            result.Add(new Mapsui.Nts.GeometryFeature()
            {
                Data = line
            });
            */
            
            return true;
        });
        
        if (ec.ArePointsDrawn) CadastralMapModel.Current.Index.QueryWindow(extent, SpatialType.Point, item =>
        {
            var point = item as PointFeature ?? throw new ApplicationException("Unexpected feature type");
            result.Add(new Map.Point(point));
            return true;
        });

        if (ec.AreLabelsDrawn) CadastralMapModel.Current.Index.QueryWindow(extent, SpatialType.Text, item =>
        {
            var text = item as TextFeature ?? throw new ApplicationException("Unexpected feature type");
            result.Add(new Map.Text(text));
            return true;
        });

        Console.WriteLine($"Found {result.Count} features");
        return Task.FromResult((IEnumerable<Mapsui.IFeature>)result);
    }

    public string? CRS
    {
        get => "EPSG:26914";
        set => throw new NotImplementedException();
    }
}