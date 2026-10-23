using System.Collections.Generic;
using System.Linq;
using Backsight.Environment;
using Backsight.Model;
using Backsight.Model.Operations;
using Mapsui.Nts;
using Nts = NetTopologySuite.Geometries;

namespace Backsight.Map.Editor;

internal class GeoJsonImportSource : FileImportSource
{
    private readonly ImportOperation _creator;
    private readonly GeoJsonReader _reader;
    private readonly IEntity _pointType;
    private readonly IEntity _lineType;

    /// <summary>
    /// Any points generated to act as line terminals.
    /// </summary>
    private record Location(long X, long Y)
    {
        internal Location(PointGeometry p) : this(p.Easting.Microns, p.Northing.Microns)
        {
        }
    }
    private readonly Dictionary<Location, Model.PointFeature> _terminals = new();

    internal GeoJsonImportSource(ImportOperation creator, GeoJsonReader reader, IEntity pointType, IEntity lineType)
        : base(reader.Path)
    {
        _creator = creator;
        _reader = reader;
        _pointType = pointType;
        _lineType = lineType;
    }

    protected override Feature[] LoadFeatures(ImportOperation creator)
    {
        var lines = new List<LineFeature>();

        foreach (GeometryFeature inboundFeature in _reader.Features)
        {
            foreach (var inboundLine in GetLineStrings(inboundFeature.Geometry))
            {
                var outboundLine = CreateLine(inboundFeature, inboundLine);
                
                if (outboundLine is not null)
                    lines.Add(outboundLine);
            }
        }
        
        var result = _terminals.Values.Cast<Feature>().Concat(lines.Cast<Feature>()).ToArray();
        return result;
    }

    private IEnumerable<Nts.LineString> GetLineStrings(Nts.Geometry? geom)
    {
        if (geom is Nts.LineString singleLine)
        {
            yield return singleLine;
        }
        else if (geom is Nts.MultiLineString multiLine)
        {
            foreach (var line in multiLine.Geometries.OfType<Nts.LineString>())
                yield return line;
        }
 
        // Perhaps need to handle geometry collections that contain lines
    }

    private LineFeature? CreateLine(
        Mapsui.Nts.GeometryFeature inboundFeature,
        Nts.LineString inboundLine)
    {
        PointGeometry[] positions = [.. inboundLine.Coordinates.Select(c => new PointGeometry(c.X, c.Y))];
        if (positions.Length < 2)
            return null;

        // Determine point features to be used for the start and end locations
        var start = CreatePoint(positions[0]);
        var end = CreatePoint(positions[^1]);
        
        LineGeometry geom = positions.Length == 2 ?
            new SegmentGeometry(start, end) :
            new MultiSegmentGeometry(start, end, positions);
        
        InternalIdValue id = _creator.Session.AllocateNextId();
        return new LineFeature(_creator, id, _lineType, start, end, geom);
    }

    private Model.PointFeature CreatePoint(PointGeometry position)
    {
        var location = new Location(position);
        if (_terminals.TryGetValue(location, out Model.PointFeature? point))
            return point;

        InternalIdValue id = _creator.Session.AllocateNextId();
        var newPoint = new Model.PointFeature(_creator, id, _pointType, position);
        _terminals.Add(location, newPoint);

        return newPoint;
    }
}