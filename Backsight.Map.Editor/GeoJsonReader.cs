using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Nts.Providers;
using Nts = NetTopologySuite.Geometries;

namespace Backsight.Map.Editor;

internal class GeoJsonReader
{
    /// <summary>
    /// The full path for the GeoJSON file.
    /// </summary>
    private readonly string _path;

    /// <summary>
    /// The features read from the GeoJSON file (empty until <see cref="ReadFeatures"/> is called).
    /// </summary>
    private readonly List<Mapsui.Nts.GeometryFeature> _features = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="GeoJsonReader"/> class.
    /// Before doing anything else with it, you need to call <see cref="ReadFeatures"/>.
    /// </summary>
    /// <param name="filePath">The full path for the GeoJSON file.</param>
    internal GeoJsonReader(string filePath)
    {
        _path = filePath;
    }

    internal string Path => _path;
    internal IReadOnlyCollection<Mapsui.Nts.GeometryFeature> Features => _features;

    /// <summary>
    /// Reads lines features from the GeoJSON file. Points and polygons are not supported.
    /// </summary>
    /// <returns>The number of line features loaded.</returns>
    internal async Task<int> ReadFeatures()
    {
        // Assume the data file has the correct coordinate system
        
        var provider = new GeoJsonProvider(Path);
        var extent = provider.GetExtent();
        if (extent is null)
            return 0;

        var ms = new MSection(extent, 1.0);
        var fetchInfo = new FetchInfo(ms);
        var features = await provider.GetFeaturesAsync(fetchInfo);

        // Hold only those features for lines (ignore all points and polygons)
        foreach (var f in features.OfType<Mapsui.Nts.GeometryFeature>())
        {
            var geom = f.Geometry;
            
            if (geom is Nts.LineString || geom is Nts.MultiLineString)
                _features.Add(f);
        }

        return _features.Count;
    }
}
