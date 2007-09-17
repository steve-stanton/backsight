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
	/// <written by="Steve Stanton" on="19-SEP-2006" />
    /// <summary>
    /// Something that might be a list. Intended to reduce overhead in situations where
    /// a class needs to refer to something that might be a list (but which is frequently
    /// a list with only one element).
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPossibleList<T> : IEnumerable<T> where T : IPossibleList<T>
    {
        /// <summary>
        /// The number of elements in the list.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The element at a specific location within the list.
        /// </summary>
        /// <param name="index">List index (>=0, less than this.Count)</param>
        /// <returns>The element at the requested list index</returns>
        T this[int index] { get; }

        /// <summary>
        /// Appends an additional element to this list
        /// </summary>
        /// <param name="thing">The item to append</param>
        /// <returns>The object representing the result (may not be the same as the
        /// original list)</returns>
        IPossibleList<T> Add(T thing);

        /// <summary>
        /// Removes an element from this list.
        /// </summary>
        /// <param name="thing">The item to remove</param>
        /// <returns>The object representing the result (may not be the same as the
        /// original list)</returns>
        IPossibleList<T> Remove(T thing);
    }
}
