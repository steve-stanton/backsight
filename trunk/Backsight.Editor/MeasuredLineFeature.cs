/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="03-AUG-2007" />
    /// <summary>
    /// Association between a measurement (observed length) and a line feature.
    /// </summary>
    class MeasuredLineFeature : IXmlContent
    {
        #region Class data

        /// <summary>
        /// The line the measurement refers to.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The observed length of the line (on the ground).
        /// </summary>
        readonly Distance m_ObservedLength;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>MeasuredLineFeature</c>
        /// </summary>
        /// <param name="line">The line the measurement applies to (may be null)</param>
        /// <param name="d">The observed length for the line (on the ground). Not null.</param>
        internal MeasuredLineFeature(LineFeature line, Distance d)
        {
            if (d==null)
                throw new ArgumentNullException();

            m_Line = line;
            m_ObservedLength = d;
        }

        #endregion

        /// <summary>
        /// The line the measurement refers to.
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        /// <summary>
        /// The observed length of the line (on the ground).
        /// </summary>
        internal Distance ObservedLength
        {
            get { return m_ObservedLength; }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public void WriteContent(XmlContentWriter writer)
        {
            writer.WriteElement("Distance", m_ObservedLength);
            writer.WriteString("Line", m_Line.DataId);
        }
    }
}
