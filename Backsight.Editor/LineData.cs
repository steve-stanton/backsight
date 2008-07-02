/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <summary>
    /// Data class for a line feature, excluding the geometry (you use this class
    /// if the geometry will be calculated on-the-fly).
    /// </summary>
    class LineData : FeatureData
    {
        #region Class data

        /// <summary>
        /// The internal ID of a previously existing point at the start of the line.
        /// </summary>
        string m_FromId;

        /// <summary>
        /// Data for the point at the start of the line (null if <c>m_FromId</c> should
        /// be used to obtain the point). If not null, it means the point was created
        /// at the same time as the line.
        /// </summary>
        FeatureData m_FromPoint;

        /// <summary>
        /// The internal ID of a previously existing point at the end of the line.
        /// </summary>
        string m_ToId;

        /// <summary>
        /// Data for the point at the end of the line (null if <c>m_ToId</c> should
        /// be used to obtain the point). If not null, it means the point was created
        /// at the same time as the line.
        /// </summary>
        FeatureData m_ToPoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public LineData()
            : base()
        {
        }

        /// <summary>
        /// Creates a new <c>LineData</c> that's initialized with the information
        /// in the supplied feature.
        /// </summary>
        /// <param name="line">The line of interest (may be null)</param>
        internal LineData(LineFeature line)
            : base(line)
        {
            PointFeature from = line.StartPoint;
            if (line.Creator == from.Creator)
                m_FromPoint = new FeatureData(from);
            else
                m_FromId = from.DataId;

            PointFeature to = line.EndPoint;
            if (line.Creator == to.Creator)
                m_ToPoint = new FeatureData(to);
            else
                m_ToId = to.DataId;
        }

        #endregion

        #region IXmlContent Members

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);

            // Write IDs (as attributes) if the point(s) existed prior to the line

            if (!String.IsNullOrEmpty(m_FromId))
            {
                Debug.Assert(m_FromPoint==null);
                writer.WriteString("From", m_FromId);
            }

            if (!String.IsNullOrEmpty(m_ToId))
            {
                Debug.Assert(m_ToPoint==null);
                writer.WriteString("To", m_ToId);
            }

            // Write FeatureData (as elements) if the point(s) were created at the same time
            // as the line

            if (m_FromPoint!=null)
            {
                Debug.Assert(String.IsNullOrEmpty(m_FromId));
                writer.WriteElement("Start", m_FromPoint);
            }

            if (m_ToPoint!=null)
            {
                Debug.Assert(String.IsNullOrEmpty(m_ToId));
                writer.WriteElement("End", m_ToPoint);
            }
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);

            m_FromId = reader.ReadString("From");
            m_ToId = reader.ReadString("To");

            if (String.IsNullOrEmpty(m_FromId))
                m_FromPoint = reader.ReadElement<FeatureData>("Start");

            if (String.IsNullOrEmpty(m_ToId))
                m_ToPoint = reader.ReadElement<FeatureData>("End");
        }

        #endregion

        /// <summary>
        /// Reads back an end point of the line
        /// </summary>
        /// <param name="p">The calculated position of the point</param>
        /// <returns>The corresponding point (may actually be a previously existing point)</returns>
        /*
        internal PointFeature ReadCalculatedPoint(IPosition p)
        {
            PointGeometry pg = PointGeometry.Create(p);
            PointFeature result = new PointFeature(pg, fd.EntityType, FindParent<Operation>());
            result.CreatorSequence = fd.CreationSequence;

            FeatureId fid = fd.Id;
            if (fid != null)
                fid.Add(result);

            AddFeature(result);
            return result;
        }
        */

        /// <summary>
        /// Obtains the 
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        /*
        internal PointFeature GetFromPoint(XmlContentReader reader, IPosition p)
        {
            PointFeature result = reader.ReadCalculatedPoint(
        }
         */
    }
}
