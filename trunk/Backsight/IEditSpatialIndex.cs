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
	/// <written by="Steve Stanton" on="31-OCT-2006" />
    /// <summary>
    /// A spatial index that can be edited.
    /// </summary>
    public interface IEditSpatialIndex : ISpatialIndex
    {
        /// <summary>
        /// Adds a spatial object into the index
        /// </summary>
        /// <param name="o">The object to add to the index</param>
        void Add(ISpatialObject o);

        /// <summary>
        /// Removes a spatial object from the index
        /// </summary>
        /// <param name="o">The object to remove from the index</param>
        /// <returns>True if object removed. False if it couldn't be found.</returns>
        bool Remove(ISpatialObject o);
    }
}
