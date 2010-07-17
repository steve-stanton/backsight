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
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology for a line that seperates a pair of polygons.
    /// </summary>
    /// <seealso cref="SectionDivider"/>
    class LineDivider : LineTopology
    {
        #region Class data

        /// <summary>
        /// The polygon ring to the left of this divider.
        /// </summary>
        Ring m_Left;

        /// <summary>
        /// The polygon ring to the right of this divider.
        /// </summary>
        Ring m_Right;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Divider</c> that relates to the specified line.
        /// </summary>
        /// <param name="line">The line the topology relates to.</param>
        internal LineDivider(LineFeature line)
            : base(line)
        {
            m_Left = m_Right = null;
        }

        #endregion

        public override Ring Left // IDivider
        {
            get { return m_Left; }
            set { m_Left = value; }
        }

        public override Ring Right // IDivider
        {
            get { return m_Right; }
            set { m_Right = value; }
        }
    }
}
