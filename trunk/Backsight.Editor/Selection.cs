/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Foresight
///
/// Foresight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Foresight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Collections.Generic;
using Backsight.Forms;
using System.Drawing;

//using Foresight.

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="13-NOV-2007" />
    /// <summary>
    /// A spatial selection with special highlighting for topological lines.
    /// </summary>
    class Selection : SpatialSelection
    {
        #region Class data

        /// <summary>
        /// The topological section that coincides with this selection. This will be
        /// defined only if the selection refers to a single topological line that has
        /// been divided into a series of sections.
        /// </summary>
        readonly IDivider m_Section;

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
            : base(so)
        {
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

        #endregion

        /// <summary>
        /// Draws the content of this selection. This calls the version implemented by
        /// the base class, and may then draw a thin yellow line on top (if the selection
        /// refers to a single topological line that has been divided into sections).
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style to use</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Draw items the normal way
            base.Render(display, style);

            // Highlight any topological section with a thin yellow overlay.
            if (m_Section!=null)
            {
                IDrawStyle thinYellow = new DrawStyle(Color.Yellow);
                m_Section.LineGeometry.Render(display, thinYellow);
            }
        }

        /// <summary>
        /// Checks whether this selection refers to the same spatial objects as
        /// another selection, and has the same reference position.
        /// </summary>
        /// <param name="that">The selection to compare with</param>
        /// <returns>True if the two selections refer to the same spatial objects (not
        /// necessarily in the same order)</returns>
        public override bool Equals(ISpatialSelection that)
        {
            // The same spatial objects have to be involved
            if (!base.Equals(that))
                return false;

            // If both selections refer to the same divider (or null), they're the same
            Selection other = (that as Selection);
            return (other!=null && this.m_Section == other.m_Section);
        }
    }
}
