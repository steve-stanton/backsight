// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;

using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="20-AUG-2008" />
    /// <summary>
    /// The database content relating to an individual section in a <see cref="LineSubdivisionOperation"/>,
    /// or a span in a <see cref="PathOperation"/>.
    /// </summary>
    class SpanContent : IXmlContent
    {
        #region Class data

        /// <summary>
        /// The observed length for the span
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// The item number of the line that was created to represent the span (0 if no
        /// line was created)
        /// </summary>
        uint m_LineItem;

        /// <summary>
        /// The ID of the point at the end of the span (null if the point was created by the
        /// editing operation)
        /// </summary>
        string m_ExistingEndPoint;

        /// <summary>
        /// The point that was created at the end of the span (null if the point existed
        /// prior to the editing operation).
        /// </summary>
        FeatureData m_CreatedEndPoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, for use during deserialization
        /// </summary>
        public SpanContent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanContent"/> class.
        /// </summary>
        /// <param name="op">The editing operation this content is for</param>
        /// <param name="mf">Information about one of the spans created by the edit</param>
        /// <exception cref="ArgumentException">If <paramref name="mf"/> is not associated with
        /// a line feature</exception>
        internal SpanContent(Operation op, MeasuredLineFeature mf)
        {
            m_Length = mf.ObservedLength;

            LineFeature line = mf.Line;
            if (line==null)
                throw new ArgumentException();

            m_LineItem = line.InternalId.ItemSequence;

            PointFeature ep = line.EndPoint;
            if (Object.ReferenceEquals(ep.Creator, op))
            {
                m_CreatedEndPoint = new FeatureData(ep);
                m_ExistingEndPoint = null;
            }
            else
            {
                m_CreatedEndPoint = null;
                m_ExistingEndPoint = ep.DataId;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpanContent"/> class for
        /// an individual span in a connection path.
        /// </summary>
        /// <param name="op">The editing operation this content is for</param>
        /// <param name="span">Information about one of the spans defining the connection path</param>
        internal SpanContent(Operation op, SpanData span)
        {
            m_Length = span.ObservedDistance;

            // The feature created to represent the span is either a line or a point (or null)...
            Feature f = span.CreatedFeature;

            if (f==null)
            {
                m_LineItem = 0;
                m_ExistingEndPoint = null;
                m_CreatedEndPoint = null;
            }
            else
            {
                PointFeature ep;

                if (f is LineFeature)
                {
                    LineFeature line = (LineFeature)f;
                    m_LineItem = line.CreatorSequence;
                    ep = line.EndPoint;
                }
                else
                {
                    m_LineItem = 0;
                    Debug.Assert(f is PointFeature);
                    ep = (PointFeature)f;
                }

                if (Object.ReferenceEquals(ep.Creator, op))
                {
                    m_CreatedEndPoint = new FeatureData(ep);
                    m_ExistingEndPoint = null;
                }
                else
                {
                    m_CreatedEndPoint = null;
                    m_ExistingEndPoint = ep.DataId;
                }
            }
        }

        #endregion

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteAttributes(XmlContentWriter writer)
        {
            // No need to write line item number if there's no line
            if (m_LineItem != 0)
                writer.WriteUnsignedInt("Line", m_LineItem);

            // You always get either To or Point
            if (m_ExistingEndPoint != null)
                writer.WriteString("To", m_ExistingEndPoint);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteChildElements(XmlContentWriter writer)
        {
            // You always get either To or Point
            if (m_ExistingEndPoint == null)
                writer.WriteElement("Point", m_CreatedEndPoint); // could be null

            writer.WriteElement("Length", m_Length);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public void ReadAttributes(XmlContentReader reader)
        {
            m_LineItem = reader.ReadUnsignedInt("Line");
            m_ExistingEndPoint = reader.ReadString("To");
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public void ReadChildElements(XmlContentReader reader)
        {
            if (m_ExistingEndPoint == null)
                m_CreatedEndPoint = reader.ReadElement<FeatureData>("Point");
            else
                m_CreatedEndPoint = null;

            m_Length = reader.ReadElement<Distance>("Length");
        }

        /// <summary>
        /// The observed length for the span
        /// </summary>
        internal Distance Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// Obtains the point at the end of the span
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <param name="p">The expected position of the point</param>
        /// <returns>The point at the start or end of the span (either a newly created point, or a point
        /// that previously existed). Null if the span wasn't associated with either a line or a point.
        /// </returns>
        internal PointFeature GetEndPoint(XmlContentReader reader, IPosition p)
        {
            if (!String.IsNullOrEmpty(m_ExistingEndPoint))
            {
                PointFeature result = reader.GetFeatureByReference<PointFeature>(m_ExistingEndPoint);
                Debug.Assert(result!=null);
                Debug.Assert(PointGeometry.Create(p).IsCoincident(result));
                return result;
            }

            if (m_CreatedEndPoint!=null)
                return reader.CreateCalculatedPoint(m_CreatedEndPoint, p);

            return null;
        }

        /// <summary>
        /// The item number of the line that was created to represent the span
        /// </summary>
        internal uint LineItemNumber
        {
            get { return m_LineItem; }
        }

        /// <summary>
        /// Creates the end point for this span (with undefined geometry)
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /// <returns>The point at the start or end of the span (either a newly created point, or a point
        /// that previously existed). Null if the span wasn't associated with either a line or a point.
        /// </returns>
        internal PointFeature CreateEndPoint(XmlContentReader reader)
        {
            if (!String.IsNullOrEmpty(m_ExistingEndPoint))
            {
                PointFeature result = reader.GetFeatureByReference<PointFeature>(m_ExistingEndPoint);
                Debug.Assert(result!=null);
                return result;
            }

            if (m_CreatedEndPoint!=null)
                return reader.CreateCalculatedPoint(m_CreatedEndPoint, null);

            return null;
        }
    }
}
