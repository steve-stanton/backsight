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
    /// <written by="Steve Stanton" on="05-NOV-2007" />
    /// <summary>
    /// Data relating to splits made for an <see cref="Intersectable"/>
    /// </summary>
    class SplitData
    {
        #region Class data

        /// <summary>
        /// The locations where intersections have been detected
        /// </summary>
        readonly IntersectionResult m_Intersections;

        /// <summary>
        /// Has a split occurred on the invisible portion of a trimmed line?
        /// </summary>
        bool m_RequiresRetrim;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>SplitData</c> that contains the specified intersection
        /// results, cutting up the intersectable where indicated.
        /// </summary>
        /// <param name="xres">The intersection results</param>
        internal SplitData(IntersectionResult xres)
        {
            m_Intersections = xres;
            m_RequiresRetrim = false;
        }

        #endregion

        /// <summary>
        /// The locations where intersections have been detected
        /// </summary>
        internal IntersectionResult Intersections
        {
            get { return m_Intersections; }
        }

        /// <summary>
        /// Has a split occurred on the invisible portion of a trimmed line?
        /// </summary>
        internal bool RequiresRetrim
        {
            get { return m_RequiresRetrim; }
            set { m_RequiresRetrim = value; }
        }
    }
}
