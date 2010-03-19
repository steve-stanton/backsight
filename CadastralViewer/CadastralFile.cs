// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Diagnostics;

using Backsight;
using Backsight.Index;

using CadastralViewer.Xml;


namespace CadastralViewer
{
    /// <written by="Steve Stanton" on="09-MAR-2010" />
    /// <summary>
    /// An ArcGIS cadastral (XML) file.
    /// </summary>
    class CadastralFile : ISpatialData
    {
        #region Static

        /// <summary>
        /// Creates an instance of <see cref="CadastralFile"/> by loading
        /// it from a file. Assumes the file is well-formed, and conforms
        /// to the recognized schema.
        /// </summary>
        /// <param name="fileName">The path of the cadastral XML file.</param>
        /// <returns>The object that corresponds to the content of the file</returns>
        /// <exception cref="Exception">If the file is not well-formed, or
        /// does not conform to the XML schema.</exception>
        internal static CadastralFile ReadFile(string fileName)
        {
            string data = File.ReadAllText(fileName);
            GeoSurveyPacketData packet = ReadXmlString(data);
            return new CadastralFile(fileName, packet);
        }

        /// <summary>
        /// Creates an instance of <see cref="CadastralFile"/> by loading
        /// it from the supplied string. Assumes the string is well-formed,
        /// and conforms to the recognized schema.
        /// </summary>
        /// <param name="data">The data read from a cadastral XML file</param>
        /// <returns>The object that corresponds to the supplied string</returns>
        /// <exception cref="Exception">If the string is not well-formed, or
        /// does not conform to the XML schema.</exception>
        internal static GeoSurveyPacketData ReadXmlString(string data)
        {
            using (StringReader sr = new StringReader(data))
            {
                using (XmlReader xr = XmlReader.Create(sr))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(GeoSurveyPacketData));
                    GeoSurveyPacketData packet = (GeoSurveyPacketData)xs.Deserialize(xr);
                    return packet;
                }
            }
        }

        #endregion

        #region Class data

        /// <summary>
        /// The name of the file
        /// </summary>
        readonly string m_Name;

        /// <summary>
        /// The data that was deserialized from XML
        /// </summary>
        readonly GeoSurveyPacketData m_Data;

        /// <summary>
        /// The coverage of the points in the file.
        /// </summary>
        readonly IWindow m_Extent;

        /// <summary>
        /// Spatial index for the data
        /// </summary>
        readonly ISpatialIndex m_Index;

        /// <summary>
        /// The points in the file, indexed by point number
        /// </summary>
        readonly Dictionary<int, IPoint> m_Points;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CadastralFile"/> class.
        /// </summary>
        /// <param name="name">The name of the xml file</param>
        /// <param name="data">The data that was deserialized</param>
        internal CadastralFile(string name, GeoSurveyPacketData data)
        {
            m_Name = Path.GetFileName(name);
            m_Data = data;
            m_Extent = new Window(m_Data.points);
            m_Points = new Dictionary<int, IPoint>(m_Data.points.Length);

            IEditSpatialIndex index = new SpatialIndex();

            foreach (Point p in m_Data.points)
            {
                index.Add(p);
                m_Points.Add(p.pointNo, p);
            }

            foreach (Plan plan in m_Data.plans)
            {
                foreach (Parcel parcel in plan.parcels)
                {
                    // Relate the parcel to it's plan
                    parcel.Plan = plan;

                    foreach (Line line in parcel.lines)
                    {
                        Debug.Assert(line.From == null && line.To == null);

                        // Relate the line to the parcel that it is part of
                        line.Parcel = parcel;

                        line.From = m_Points[line.fromPoint];
                        line.To = m_Points[line.toPoint];

                        if (line.centerPointSpecified)
                            line.Center = m_Points[line.centerPoint];

                        index.Add(line);
                    }

                    foreach (LinePoint lp in parcel.linePoints)
                    {
                        // Relate to the parcel it's referenced by

                        // Relate the associated point 
                    }
                }
            }

            m_Index = index;
        }

        #endregion

        #region ISpatialData Members

        /// <summary>
        /// A name for the data source
        /// </summary>
        public string Name
        {
            get { return m_Name; }
        }

        /// <summary>
        /// Draws the content of the map on a display.
        /// </summary>
        /// <param name="display">The display on which to draw</param>
        /// <param name="style">The display style to use</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            new DrawQuery(m_Index, display, style);
        }

        /// <summary>
        /// Is the map currently empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return (m_Data.points.Length == 0); }
        }

        /// <summary>
        /// The ground extent of the map.
        /// </summary>
        public IWindow Extent
        {
            get { return m_Extent; }
        }

        /// <summary>
        /// Locates the object closest to a specific position.
        /// </summary>
        /// <param name="p">The search position</param>
        /// <param name="radius">The search radius</param>
        /// <param name="types">The type(s) of object to look for</param>
        /// <returns>
        /// The closest object of the requested type (null if nothing found)
        /// </returns>
        public ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return m_Index.QueryClosest(p, radius, types);
        }

        #endregion
    }
}
