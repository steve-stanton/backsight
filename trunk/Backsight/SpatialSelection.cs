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

namespace Backsight
{
	/// <written by="Steve Stanton" on="08-JAN-2007" />
    /// <summary>
    /// A selection of spatial objects
    /// </summary>
    public class SpatialSelection : ISpatialSelection
    {
        #region Class data

        /// <summary>
        /// The currently selected spatial objects (never null)
        /// </summary>
        private readonly List<ISpatialObject> m_Items;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SpatialSelection</c> with nothing in it.
        /// </summary>
        public SpatialSelection()
        {
            m_Items = new List<ISpatialObject>();
        }

        /// <summary>
        /// Creates a new <c>SpatialSelection</c> that contains a single item (or nothing).
        /// </summary>
        /// <param name="so">The object to remember as part of this selection (if null, it
        /// will not be added to the selection)</param>
        public SpatialSelection(ISpatialObject so)
        {
            if (so==null)
                m_Items = new List<ISpatialObject>();
            else
            {
                m_Items = new List<ISpatialObject>(1);
                m_Items.Add(so);
            }
        }

        /// <summary>
        /// Creates a new <c>SpatialSelection</c> that consists of the items in the supplied list.
        /// </summary>
        /// <param name="items">The items defining the content of the new selection</param>
        public SpatialSelection(IEnumerable<ISpatialObject> items)
        {
            m_Items = new List<ISpatialObject>(items);
        }

        #endregion

        /// <summary>
        /// The one and only item in this selection (null if the selection is empty, or
        /// it contains more than one item).
        /// </summary>
        public ISpatialObject Item
        {
            get { return (m_Items.Count==1 ? m_Items[0] : null); }
        }

        /// <summary>
        /// The number of items in the selection
        /// </summary>
        public int Count
        {
            get { return (m_Items.Count); }
        }

        /// <summary>
        /// The items in the selection
        /// </summary>
        public IEnumerable<ISpatialObject> Items
        {
            get { return m_Items; }
        }

        /// <summary>
        /// Checks whether this selection refers to the same spatial objects as
        /// another selection.
        /// </summary>
        /// <param name="that">The selection to compare with</param>
        /// <returns>True if the two selections refer to the same spatial objects (not
        /// necessarily in the same order)</returns>
        public bool Equals(ISpatialSelection that)
        {
            return Equals(this, that);
        }

        /// <summary>
        /// Checks whether two selections refer to the same objects
        /// </summary>
        /// <param name="a">The 1st selection</param>
        /// <param name="b">The 2nd selection</param>
        /// <returns>True if both selections are not null, contain the same number
        /// of elements, and refer to the same spatial objects (the same instances)</returns>
        public static bool Equals(ISpatialSelection a, ISpatialSelection b)
        {
            if (a==null || b==null)
                return false;

            if (Object.ReferenceEquals(a, b))
                return true;

            if (a.Count != b.Count)
                return false;

            foreach (ISpatialObject sob in b.Items)
            {
                bool found = false;

                foreach (ISpatialObject soa in a.Items)
                {
                    if (Object.ReferenceEquals(soa, sob))
                    {
                        found = true;
                        break;
                    }
                }

                if (!found)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether this selection refers to one specific spatial object.
        /// </summary>
        /// <param name="so">The object to compare with</param>
        /// <returns>True if this selection refers to a single item that corresponds
        /// to the specified spatial object</returns>
        public bool Equals(ISpatialObject so)
        {
            if (so==null)
                return false;

            if (this.Count!=1)
                return false;

            return Object.ReferenceEquals(so, m_Items[0]);
        }

        /// <summary>
        /// Draws the content of this selection
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style to use</param>
        public virtual void Render(ISpatialDisplay display, IDrawStyle style)
        {
            foreach (ISpatialObject item in m_Items)
                item.Render(display, style);
        }
    }
}
