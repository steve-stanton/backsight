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
	/// <written by="Steve Stanton" on="05-APR-2007" />
    /// <summary>
    /// A numeric ID that is potentially decorated according to some display format (as
    /// defined through an associated <see cref="IdGroup"/>
    /// </summary>
    class DisplayId
    {
        #region Class data

        /// <summary>
        /// The raw ID (undecorated with stuff like check digits)
        /// </summary>
        uint m_Id;

        /// <summary>
        /// The group the ID is part of
        /// </summary>
        IdGroup m_Group;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>DisplayId</c>
        /// </summary>
        /// <param name="group">The ID group containing the raw ID</param>
        /// <param name="rawId">The raw ID (undecorated with stuff like check digits)</param>
        internal DisplayId(IdGroup group, uint rawId)
        {
            if (group == null)
                throw new ArgumentNullException();

            m_Group = group;
            m_Id = rawId;
        }

        #endregion

        /// <summary>
        /// The formatted version of the raw ID.
        /// </summary>
        /// <returns>The result of a call to <c>IdGroup.FormatId(RawId)</c></returns>
        public override string ToString()
        {
            return m_Group.FormatId(m_Id);
        }

        /// <summary>
        /// The raw ID (undecorated with stuff like check digits)
        /// </summary>
        internal uint RawId
        {
            get { return m_Id; }
        }

        /// <summary>
        /// Creates a new feature ID that doesn't reference anything (and does not add it to the map model).
        /// </summary>
        /// <returns>The created feature ID.</returns>
        internal FeatureId CreateId()
        {
            IdPacket p = m_Group.FindPacket(m_Id);
            p.ReserveId(m_Id);
            return p.CreateId(m_Id);
        }
    }
}
