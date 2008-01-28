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

namespace Backsight
{
	/// <written by="Steve Stanton" on="08-JAN-2007" />
    /// <summary>
    /// A selection of spatial objects
    /// </summary>
    public class SpatialSelection : IEditSpatialSelection
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
                this.Add(so);
            }
        }

        #endregion

        /// <summary>
        /// Adds a spatial object to this selection, given that it is not already
        /// part of the selection.
        /// </summary>
        /// <param name="so">The object to remember as part of this selection (not null)</param>
        /// <exception cref="ArgumentNullException">If the specified object is null</exception>
        public void Add(ISpatialObject so)
        {
            if (so==null)
                throw new ArgumentNullException();

            if (!m_Items.Contains(so))
                m_Items.Add(so);
        }

        /// <summary>
        /// Removes a spatial object from this selection.
        /// </summary>
        /// <param name="so">The object to remove from this selection</param>
        /// <returns>True if object removed. False if the object isn't part of this selection.</returns>
        public bool Remove(ISpatialObject so)
        {
            return m_Items.Remove(so);
        }

        /// <summary>
        /// Replaces the current selection with a specific item.
        /// </summary>
        /// <param name="so">The object that will replace the current selection.</param>
        /// <exception cref="ArgumentNullException">If the specified object is null</exception>
        public void Replace(ISpatialObject so)
        {
            m_Items.Clear();
            this.Add(so);
        }

        /// <summary>
        /// Removes all items from this selection.
        /// </summary>
        public virtual void Clear()
        {
            m_Items.Clear();
        }

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
        public virtual bool Equals(ISpatialSelection that)
        {
            if (that==null)
                return false;

            if (Object.ReferenceEquals(this, that))
                return true;

            if (this.Count != that.Count)
                return false;

            foreach (ISpatialObject so in that.Items)
            {
                if (!m_Items.Contains(so))
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
