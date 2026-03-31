// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System.Text;

using NetTopologySuite.Features;
using NTS=NetTopologySuite.Geometries;
using NetTopologySuite.IO.Esri;

using Backsight.Index;
using Backsight.Forms;

namespace Backsight.ShapeViewer;

class ShapeFile : ISpatialData
{
    private readonly string _mapName;
    private readonly IWindow _extent;
    private readonly ISpatialIndex _index;

    public ShapeFile(string fileName)
    {
        if (String.IsNullOrEmpty(fileName))
            throw new ArgumentNullException();
                            
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        _mapName = Path.GetFileName(fileName);
        IEditSpatialIndex index = new SpatialIndex();

        var minX = Double.MaxValue;
        var minY = Double.MaxValue;
        var maxX = Double.MinValue;
        var maxY = Double.MinValue;

        foreach (var feature in Shapefile.ReadAllFeatures(fileName))
        {
            var geom = feature.Geometry;
            if (geom.IsEmpty)
                continue;

            var attributes = CreatePropertyList(feature);
            
            // Update overall extent
            var envelope = geom.EnvelopeInternal;

            minX = Math.Min(minX, envelope.MinX);
            minY = Math.Min(minY, envelope.MinY);
            maxX = Math.Max(maxX, envelope.MaxX);
            maxY = Math.Max(maxY, envelope.MaxY);
            
            List<NTS.Geometry> geoms = GetBasicGeometries(geom);
            foreach (NTS.Geometry g in geoms)
            {
                g.UserData = attributes;
                if (g is NTS.Point)
                    index.Add(new PointWrapper((NTS.Point)g));
                else
                    index.Add(new GeometryWrapper(g));
            }
        }

        _extent = new Window(minX, minY, maxX, maxY);

        // Don't permit any further additions
        _index = index;
    }

    private List<NTS.Geometry> GetBasicGeometries(NTS.Geometry geom)
    {
        List<NTS.Geometry> result = new List<NTS.Geometry>();
        AppendBasicGeometries(result, geom);
        return result;
    }

    private void AppendBasicGeometries(List<NTS.Geometry> result, NTS.Geometry geom)
    {
        if (geom is NTS.GeometryCollection || geom is NTS.MultiPolygon) 
        {
            // Note that I initially had 'foreach (NTS.Geometry g in gc)', which compiled,
            // but which led to an infinite loop when dealing with multi-polygons.
            NTS.GeometryCollection gc = (NTS.GeometryCollection)geom;
            NTS.Geometry[] ga = gc.Geometries;
            foreach (NTS.Geometry g in ga)
                AppendBasicGeometries(result, g); // recurse
        }
        else
            result.Add(geom);
    }

    private AdhocPropertyList CreatePropertyList(Feature feature)
    {
        var result = new AdhocPropertyList(feature.Attributes.Count);
        
        foreach (var attribute in feature.Attributes.GetNames())
        {
            result.Add(new AdhocProperty(attribute, feature.Attributes[attribute], true, true));
        }

        return result;
    }

    public IWindow Extent => _extent;

    public string Name => _mapName;

    public bool IsEmpty => _index.IsEmpty;

    public void Render(ISpatialDisplay display, IDrawStyle style)
    {
        new DrawQuery(_index, display, style);
    }

    public ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
    {
        return _index.QueryClosest(p, radius, types);
    }
}