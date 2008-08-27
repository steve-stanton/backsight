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

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// The database content that corresponds to an instance of <see cref="Leg"/>
    /// </summary>
    class LegContent : IXmlContent
    {
        #region Class data

        /// <summary>
        /// The leg this content is based on
        /// </summary>
        Leg m_Leg;

        /// <summary>
        /// The definition for each span on this leg (should always contain at least
        /// one element).
        /// </summary>
        SpanContent[] m_Spans;

        /// <summary>
        /// The face number of this leg (if this leg is staggered). In the range [0,2]. A value
        /// of zero means the leg is not staggered.
        /// </summary>
        byte m_FaceNumber;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, for serialization.
        /// </summary>
        public LegContent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LegContent"/> class.
        /// </summary>
        /// <param name="op">The editing operation the leg is part of</param>
        /// <param name="leg">The leg data that needs to be formatted for the database</param>
        internal LegContent(PathOperation op, Leg leg)
        {
            m_Leg = leg;
            m_Spans = new SpanContent[leg.NumSpan];
            for (int i=0; i<m_Spans.Length; i++)
                m_Spans[i] = new SpanContent(op, leg.GetSpanData(i));

            m_FaceNumber = (byte)leg.FaceNumber;
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
            if (m_FaceNumber!=0)
                writer.WriteUnsignedInt("Face", (uint)m_FaceNumber);

            // Write description of the geometry that defines the leg

            writer.WriteArray("SpanArray", "Span", m_Spans);
        }

        /// <summary>
        /// Loads the content of this class.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public void ReadContent(XmlContentReader reader)
        {
            m_FaceNumber = (byte)reader.ReadUnsignedInt("Face");
            //m_Leg = reader.ReadElement<Leg>
            m_Spans = reader.ReadArray<SpanContent>("SpanArray", "Span");
        }

        #endregion
    }
}
