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
using System.Drawing;

using Backsight.Forms;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="13-NOV-2007" />
    /// <summary>
    /// A spatial selection
    /// </summary>
    class Selection : ISpatialSelection
    {
        #region Class data

        /// <summary>
        /// The currently selected spatial objects (never null)
        /// </summary>
        private readonly List<ISpatialObject> m_Items;

        /// <summary>
        /// The topological section that coincides with this selection. This will be
        /// defined only if the selection refers to a single topological line that has
        /// been divided into a series of sections.
        /// </summary>
        IDivider m_Section;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Selection</c> that contains a single item (or nothing).
        /// </summary>
        /// <param name="so">The object to remember as part of this selection (if null, it
        /// will not be added to the selection)</param>
        /// <param name="searchPosition">A position associated with the selection (null
        /// if a specific position isn't relevant). This is used to determine whether a
        /// topological section is relevant when a line is selected.</param>
        public Selection(ISpatialObject so, IPosition searchPosition)
        {
            m_Items = new List<ISpatialObject>(1);
            if (so!=null)
                m_Items.Add(so);

            // If we're dealing with a single line that's been topologically sectioned,
            // determine which divider we're closest to.

            m_Section = null;

            if (searchPosition != null)
            {
                LineFeature line = (so as LineFeature);
                if (line != null && line.Topology is SectionTopologyList)
                {
                    SectionTopologyList sections = (line.Topology as SectionTopologyList);
                    m_Section = sections.FindClosestSection(searchPosition);
                }
            }
        }

        /// <summary>
        /// Creates a new <c>Selection</c> that refers to nothing.
        /// </summary>
        internal Selection()
        {
            m_Section = null;
            m_Items = new List<ISpatialObject>();
        }

        /// <summary>
        /// Creates a new <c>Selection</c> that consists of the items in the supplied list.
        /// </summary>
        /// <param name="items">The items defining the content of the new selection</param>
        internal Selection(IEnumerable<ISpatialObject> items)
        {
            m_Section = null;
            m_Items = new List<ISpatialObject>(items);
        }

        #endregion

        /// <summary>
        /// Removes a spatial object from this selection.
        /// </summary>
        /// <param name="so">The object to remove from this selection</param>
        /// <returns>True if object removed. False if the object isn't part of this selection.</returns>
        internal bool Remove(ISpatialObject so)
        {
            return m_Items.Remove(so);
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
        /// Draws the content of this selection. This calls the version implemented by
        /// the base class, and may then draw a thin yellow line on top (if the selection
        /// refers to a single topological line that has been divided into sections).
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style to use</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Draw items the normal way
            foreach (ISpatialObject item in m_Items)
                item.Render(display, style);

            // Highlight any topological section with a thin yellow overlay.
            if (m_Section!=null)
            {
                IDrawStyle thinYellow = new DrawStyle(Color.Yellow);
                m_Section.LineGeometry.Render(display, thinYellow);
            }
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
        /// Checks whether this selection refers to the same spatial objects as
        /// another selection, and has the same reference position.
        /// </summary>
        /// <param name="that">The selection to compare with</param>
        /// <returns>True if the two selections refer to the same spatial objects (not
        /// necessarily in the same order)</returns>
        public bool Equals(ISpatialSelection that)
        {
            // The same spatial objects have to be involved
            if (!SpatialSelection.Equals(this, that))
                return false;

            // If both selections refer to the same divider (or null), they're the same
            Selection other = (that as Selection);
            return (other!=null && this.m_Section == other.m_Section);
        }

        /// <summary>
        /// The topological section that coincides with this selection. This should be
        /// defined only if the selection refers to a single topological line that has
        /// been divided into a series of sections.
        /// </summary>
        protected IDivider Section
        {
            get { return m_Section; }
            set { m_Section = value; }
        }

        /// <summary>
        /// Adds a spatial object to this selection, given that it is not already
        /// part of the selection.
        /// </summary>
        /// <param name="so">The object to remember as part of this selection (not null)</param>
        /// <exception cref="ArgumentNullException">If the specified object is null</exception>
        internal void Add(ISpatialObject so)
        {
            if (so==null)
                throw new ArgumentNullException();

            if (!m_Items.Contains(so))
                m_Items.Add(so);
        }
    }
}
