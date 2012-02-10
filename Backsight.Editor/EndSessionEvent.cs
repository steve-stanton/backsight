// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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
    /// <written by="Steve Stanton" on="10-FEB-2012"/>
    /// <summary>
    /// Event data for normal completion of an editing session
    /// </summary>
    class EndSessionEvent : Change
    {
        #region Class data

        // No data (all I really want is the timestamp stored in the base class).

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="EndSessionEvent"/> class.
        /// </summary>
        internal EndSessionEvent(uint id)
            : base(id)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EndSessionEvent"/> class.
        /// </summary>
        /// <param name="ed">The mechanism for reading back content.</param>
        internal EndSessionEvent(EditDeserializer ed)
            : base(ed)
        {
        }

        #endregion
    }
}
