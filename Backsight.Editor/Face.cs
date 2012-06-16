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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="16-MAY-1999" />
    /// <summary>
    ///	A polygon ring divider that is associated with a facing direction. This is a transient
    ///	class that is utilized when a new polygon is being created.
    /// </summary>
    class Face : IEquatable<Face>
    {
        #region Class data

        /// <summary>
        /// What divider?
        /// </summary>
        readonly IDivider m_Divider;

        /// <summary>
        /// Is it facing left?
        /// </summary>
        readonly bool m_IsLeft;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Face</c> for the specified divider.
        /// </summary>
        internal Face(IDivider d, bool isLeft)
        {
            m_Divider = d;
            m_IsLeft = isLeft;
        }

        #endregion

        /// <summary>
        /// The divider involved.
        /// </summary>
        internal IDivider Divider
        {
            get { return m_Divider; }
        }

        /// <summary>
        /// Is the polygon involved to the left of the divider?
        /// </summary>
        internal bool IsLeft
        {
            get { return m_IsLeft; }
        }

        #region IEquatable<Face> Members

        public bool Equals(Face that)
        {
            return (Object.ReferenceEquals(this.m_Divider, that.m_Divider) && this.m_IsLeft==that.m_IsLeft);
        }

        #endregion
    }
}
