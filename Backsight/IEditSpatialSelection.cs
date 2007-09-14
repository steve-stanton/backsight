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

namespace Backsight
{
	/// <written by="Steve Stanton" on="08-JAN-2007" />
    /// <summary>
    /// A mutable spatial selection
    /// </summary>
    public interface IEditSpatialSelection : ISpatialSelection
    {
        /// <summary>
        /// Adds a spatial object to this selection.
        /// </summary>
        /// <param name="so">The object to remember as part of this selection (not null)</param>
        /// <exception cref="ArgumentNullException">If the specified object is null</exception>
        void Add(ISpatialObject so);

        /// <summary>
        /// Removes a spatial object from this selection.
        /// </summary>
        /// <param name="so">The object to remove from this selection</param>
        /// <returns>True if object removed. False if the object isn't part of this selection.</returns>
        bool Remove(ISpatialObject so);

        /// <summary>
        /// Replaces the current selection with a specific item.
        /// </summary>
        /// <param name="so">The object that will replace the current selection.</param>
        /// <exception cref="ArgumentNullException">If the specified object is null</exception>
        void Replace(ISpatialObject so);

        /// <summary>
        /// Removes all items from this selection.
        /// </summary>
        void Clear();
    }
}
