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

using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;

using GisSharpBlog.NetTopologySuite.IO;
using NTS=GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Geometries;

using Backsight.Index;
using Backsight.Forms;

namespace Backsight.ShapeViewer
{
    class ShapeFile : ISpatialData
    {
        private readonly string m_MapName;
        private readonly IWindow m_Extent;
        private readonly ISpatialIndex m_Index;

        public ShapeFile(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
                throw new ArgumentNullException();
                            
            m_MapName = fileName;
            IEditSpatialIndex index = new SpatialIndex();

            ShapefileDataReader sfdr = Shapefile.CreateDataReader(fileName, new GeometryFactory());
            ShapefileHeader hdr = sfdr.ShapeHeader;
            Envelope ex = hdr.Bounds;
            m_Extent = new Window(ex.MinX, ex.MinY, ex.MaxX, ex.MaxY);

            foreach (object o in sfdr)
            {
                // You get back an instance of GisSharpBlog.NetTopologySuite.IO.RowStructure, but
                // that's internal, so cast to the interface it implements (we'll attach this to
                // the geometry we wrap).
                //ICustomTypeDescriptor row = (ICustomTypeDescriptor)o;
                AdhocPropertyList row = CreatePropertyList(sfdr);
                NTS.Geometry geom = sfdr.Geometry;
                geom.UserData = row;

                List<NTS.Geometry> geoms = GetBasicGeometries(geom);
                foreach (NTS.Geometry g in geoms)
                {
                    g.UserData = row;
                    index.Add(new GeometryWrapper(g));
                }
            }

            // Don't permit any further additions
            m_Index = index;
        }

        private List<NTS.Geometry> GetBasicGeometries(NTS.Geometry geom)
        {
            List<NTS.Geometry> result = new List<NTS.Geometry>();
            AppendBasicGeometries(result, geom);
            return result;
        }

        private void AppendBasicGeometries(List<NTS.Geometry> result, NTS.Geometry geom)
        {
            if (geom is GeometryCollection)
            {
                // Note that I initially had 'foreach (NTS.Geometry g in gc)', which compiled,
                // but which led to an infinite loop when dealing with multi-polygons.
                GeometryCollection gc = (GeometryCollection)geom;
                NTS.Geometry[] ga = gc.Geometries;
                foreach (NTS.Geometry g in ga)
                    AppendBasicGeometries(result, g); // recurse
            }
            else
                result.Add(geom);
        }

        // If you select the ICustomTypeDescriptor implemented by RowStructure into a
        // property grid, the values are somehow missing, so copy them over to a class
        // that works.
        private AdhocPropertyList CreatePropertyList(ShapefileDataReader sfdr)
        {
            object[] vals = new object[sfdr.FieldCount-1]; // ignore the geometry column
            sfdr.GetValues(vals);
            AdhocPropertyList result = new AdhocPropertyList(vals.Length);

            for(int i=0; i<vals.Length; i++)
            {
                string name = sfdr.GetName(i+1); // yes it's +1
                result.Add(new AdhocProperty(name, vals[i], true, true)); // but the value isn't!
            }

            return result;
        }

        public IWindow Extent
        {
            get { return m_Extent; }
        }

        public string Name
        {
            get { return Path.GetFileName(m_MapName); }
        }

        public bool IsEmpty
        {
            get { return m_Index.IsEmpty; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            new DrawQuery(m_Index, display, style);
        }

        public ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return m_Index.QueryClosest(p, radius, types);
        }
    }
}
