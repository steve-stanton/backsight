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
using System.Collections.Generic;

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-OCT-2007" />
    /// <summary>
    /// Dumb implementation of <see cref="ITerminal"/> that has no incident polygon dividers
    /// (i.e. it always floats in space).
    /// </summary>
    /// <remarks>This class exists only because I need to detect intersections while a
    /// new line is in the process of getting added. To do that using the <c>IntersectionFinder</c>
    /// class, I need to pass in an instance of <c>LineGeometry</c>, and to create that, I need
    /// two instances of <c>ITerminal</c>.</remarks>
    class FloatingTerminal : PointGeometry, ITerminal
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FloatingTerminal</c> at the specified position (rounded off to
        /// the nearest micron)
        /// </summary>
        /// <param name="p">The position of the terminal</param>
        internal FloatingTerminal(IPosition p)
            : base(p)
        {
        }

        #endregion

        #region ITerminal Members

        /// <summary>
        /// Returns null, indicating that no polygon dividers start or end at this terminal.
        /// </summary>
        public IDivider[] IncidentDividers()
        {
            return null;
        }

        #endregion
    }
}
