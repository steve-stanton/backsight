// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <summary>
    /// Wrapper class that contains a point where the position is calculated by the
    /// edit that created it. For use during serialization.
    /// </summary>
    class CalculatedPoint : Content
    {
        #region Class data

        /// <summary>
        /// The calculated point (may be null)
        /// </summary>
        readonly PointFeature m_Point;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedPoint"/> class.
        /// </summary>
        /// <param name="point">The calculated point (may be null)</param>
        internal CalculatedPoint(PointFeature point)
        {
            m_Point = point;
        }

        #endregion

        /// <summary>
        /// The string that will be used as the xsi:type for this content.
        /// </summary>
        /// <remarks>Implements IXmlContent</remarks>
        public override string XmlTypeName
        {
            get { return "CalculatedFeatureType"; }
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            if (m_Point != null)
                m_Point.WriteFeatureAttributes(writer);
        }
    }
}
