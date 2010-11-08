// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="27-MAR-2002" />
    /// <summary>
    /// The sections that define one face of a subdivided line.
    /// </summary>
    class LineSubdivisionFace
    {
        #region Class data

        /// <summary>
        /// The data entry string that defines the subdivision sections.
        /// </summary>
        readonly string m_EntryString;

        /// <summary>
        /// The default distance units to use when decoding the data entry string.
        /// </summary>
        readonly DistanceUnit m_DefaultEntryUnit;

        /// <summary>
        /// Are the distances observed from the end of the line?
        /// </summary>
        readonly bool m_IsEntryFromEnd;

        /// <summary>
        /// The sections of the subdivided line.
        /// </summary>
        List<MeasuredLineFeature> m_Sections;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineSubdivisionFace"/> class.
        /// </summary>
        /// <param name="entryString">The data entry string that defines the subdivision sections.</param>
        /// <param name="defaultEntryUnit">The default distance units to use when decoding
        /// the data entry string.</param>
        /// <param name="isEntryFromEnd">Are the distances observed from the end of the line?</param>
        internal LineSubdivisionFace(string entryString, DistanceUnit defaultEntryUnit, bool isEntryFromEnd)
        {
            m_EntryString = entryString;
            m_DefaultEntryUnit = defaultEntryUnit;
            m_IsEntryFromEnd = isEntryFromEnd;
            m_Sections = null;
        }

        #endregion

    }
}
