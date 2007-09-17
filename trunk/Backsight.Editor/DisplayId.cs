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
	/// <written by="Steve Stanton" on="05-APR-2007" />
    /// <summary>
    /// A numeric ID that is potentially decorated according to some display format (as
    /// defined through an associated <c>IdRange</c>).
    /// </summary>
    class DisplayId
    {
        #region Class data

        /// <summary>
        /// The raw ID (undecorated with stuff like check digits)
        /// </summary>
        uint m_Id;

        /// <summary>
        /// The range the ID is part of
        /// </summary>
        IdRange m_Range;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DisplayId</c>
        /// </summary>
        /// <param name="range">The range the ID is part of (holds formatting rules for the ID)</param>
        /// <param name="rawId">The raw ID (undecorated with stuff like check digits)</param>
        internal DisplayId(IdRange range, uint rawId)
        {
            if (range==null)
                throw new ArgumentNullException();

            m_Range = range;
            m_Id = rawId;
        }

        #endregion

        /// <summary>
        /// The formatted version of the raw ID.
        /// </summary>
        /// <returns>The result of a call to <c>IdRange.FormatId(RawId)</c></returns>
        public override string ToString()
        {
            return m_Range.FormatId(m_Id);
        }

        /// <summary>
        /// The range the ID is part of
        /// </summary>
        internal IdRange IdRange
        {
            get { return m_Range; }
        }

        /// <summary>
        /// The raw ID (undecorated with stuff like check digits)
        /// </summary>
        internal uint RawId
        {
            get { return m_Id; }
        }
    }
}
