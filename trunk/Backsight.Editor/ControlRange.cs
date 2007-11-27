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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CeControlRange" />
    /// <summary>
    /// A range of control points.
    /// </summary>
    class ControlRange
    {
        #region Class data

        /// <summary>
        /// The low end of the range.
        /// </summary>
        uint m_MinId;

        /// <summary>
        /// The high end of the range.
        /// </summary>
        uint m_MaxId;

        /// <summary>
        /// The number of control points in the range.
        /// </summary>
        uint m_NumControl;

        /// <summary>
        /// Control data (one for each control point in the range).
        /// </summary>
        ControlPoint[] m_Control;
        //UINT4 m_NumAlloc;		// Allocated size of the array.

        /// <summary>
        /// Number of control points that have been defined.
        /// </summary>
        uint m_NumDefined;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor.
        /// Creates a new <c>ControlRange</c>
        /// </summary>
        internal ControlRange()
        {
        }

        #endregion
    }
}
