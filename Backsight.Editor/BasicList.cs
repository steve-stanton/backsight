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
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// A <c>BasicList</c> is a simplified list, implemented via association with a <c>List</c>
    /// </summary>
    /// <typeparam name="T">The type of object in the list</typeparam>
    class BasicList<T> : IPossibleList<T> where T : IPossibleList<T>
    {
        #region Class data

        /// <summary>
        /// The items in the list (not null)
        /// </summary>
        private readonly List<T> m_List;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>BasicList</c> with nothing in it.
        /// </summary>
        internal BasicList()
        {
            m_List = new List<T>();
        }
        
        /// <summary>
        /// Creates a <c>BasicList</c> that contains two items.
        /// </summary>
        /// <param name="item1">The first item to add to the list.</param>
        /// <param name="item2">The second item to add to the list (not the same as the first item).</param>
        /// <exception cref="ArgumentException">If the two items refer to the same object</exception>
        internal BasicList(T item1, T item2)
        {
            if (Object.ReferenceEquals(item1, item2))
                throw new ArgumentException("Attempt to initialize list with two identical items");

            m_List = new List<T>(2);
            m_List.Add(item1);
            m_List.Add(item2);
        }

        /// <summary>
        /// Creates a <c>BasicList</c> that contains the items in the supplied collection.
        /// </summary>
        /// <param name="items">The items to add to the list</param>
        internal BasicList(IEnumerable<T> items)
        {
            m_List = new List<T>(items);
        }

        #endregion

        /// <summary>
        /// The number of items in the list.
        /// </summary>
        public int Count
        {
            get { return m_List.Count; }
        }

        /// <summary>
        /// A specific element in the list.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public T this[int index]
        {
            get { return m_List[index]; }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return m_List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // the other one
        }

        /// <summary>
        /// Appends an object to this list
        /// </summary>
        /// <param name="thing">The thing to append</param>
        /// <returns>A reference to this list (always)</returns>
        public IPossibleList<T> Add(T thing)
        {
            m_List.Add(thing);
            return this;
        }

        /// <summary>
        /// Removes an object from this list
        /// </summary>
        /// <param name="thing">The thing to remove</param>
        /// <returns>The object that holds what's left of the list. Corresponds to <c>this</c>
        /// list if the list contains two or more elements after removal. Refers to the
        /// first element of the list if the list ends up with just one element.
        /// </returns>
        public IPossibleList<T> Remove(T thing)
        {
            m_List.Remove(thing);

            if (m_List.Count==1)
                return m_List[0];
            else
                return this;
        }

        /// <summary>
        /// Checks whether an object appears in this list.
        /// </summary>
        /// <param name="item">The object to look for</param>
        /// <returns>True if the object is in the list. False if it isn't.</returns>
        public bool Contains(T item)
        {
            return m_List.Contains(item);
        }
    }
}
