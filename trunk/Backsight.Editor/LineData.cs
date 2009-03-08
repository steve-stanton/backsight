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
        public override void WriteContent(XmlContentWriter writer)
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
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);

            // Write IDs (as attributes) if the point(s) existed prior to the line

            if (!String.IsNullOrEmpty(m_FromId))
            {
                Debug.Assert(m_FromPoint == null);
                writer.WriteString("From", m_FromId);
            }

            if (!String.IsNullOrEmpty(m_ToId))
            {
                Debug.Assert(m_ToPoint == null);
                writer.WriteString("To", m_ToId);
            }
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            // Write FeatureData (as elements) if the point(s) were created at the same time
            // as the line

            if (m_FromPoint != null)
            {
                Debug.Assert(String.IsNullOrEmpty(m_FromId));
                writer.WriteElement("Start", m_FromPoint);
            }

            if (m_ToPoint != null)
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
        public override void ReadContent(XmlContentReader reader)
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
        /// Obtains the point at the start of the line
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <param name="p">The expected position of the point</param>
        /// <returns>The point at the start of the line (either a newly created point, or a point
        /// that previously existed)</returns>
        internal PointFeature GetFromPoint(XmlContentReader reader, IPosition p)
        {
            return GetPoint(reader, p, m_FromPoint, m_FromId);
        }

        /// <summary>
        /// Obtains the point at the end of the line
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <param name="p">The expected position of the point</param>
        /// <returns>The point at the end of the line (either a newly created point, or a point
        /// that previously existed)</returns>
        internal PointFeature GetToPoint(XmlContentReader reader, IPosition p)
        {
            return GetPoint(reader, p, m_ToPoint, m_ToId);
        }

        /// <summary>
        /// Obtains the point at the start or end of the line
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <param name="p">The expected position of the point</param>
        /// <param name="fd">Data for the point (null if <paramref name="id"/> should be
        /// used to obtain the point)</param>
        /// <param name="id">The internal ID of a previously created point that should
        /// be returned (null if the point should be created here)</param>
        /// <returns>The point at the start or end of the line (either a newly created point, or a point
        /// that previously existed)</returns>
        PointFeature GetPoint(XmlContentReader reader, IPosition p, FeatureData fd, string id)
        {
            // If there is no information about a created point, it must be a reference
            // to a previously created point.

            if (fd==null)
            {
                Debug.Assert(!String.IsNullOrEmpty(id));
                PointFeature result = reader.GetFeatureByReference<PointFeature>(id);
                Debug.Assert(result!=null);
                Debug.Assert(PointGeometry.Create(p).IsCoincident(result));
                return result;
            }

            // Create a new point
            Debug.Assert(String.IsNullOrEmpty(id));
            return reader.CreateCalculatedPoint(fd, p);
        }
    }
}
