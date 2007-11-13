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
    class CadastralSelection : SpatialSelection
    {
        #region Class data

        /// <summary>
        /// A position associated with the selection (e.g. could be the point where
        /// the user clicked on a map display). May be null if a single position does
        /// not apply to the selection.
        /// </summary>
        readonly IPosition m_SearchPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CadastralSelection</c> that contains a single item (or nothing).
        /// </summary>
        /// <param name="so">The object to remember as part of this selection (if null, it
        /// will not be added to the selection)</param>
        /// <param name="searchPosition">A position to associate with the selection (null
        /// if a specific position isn't relevant)</param>
        public CadastralSelection(ISpatialObject so, IPosition searchPosition)
            : base(so)
        {
            m_SearchPosition = searchPosition;
        }

        #endregion

        /// <summary>
        /// Draws the content of this selection. This calls the version implemented by
        /// the base class, and may then draw a thin yellow line on top of topological
        /// lines that have been divided into sections.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style to use</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // Draw stuff the normal way
            base.Render(display, style);

            // Just return if a specific search position isn't available
            if (m_SearchPosition==null)
                return;

            // If we have any sectioned topological lines, highlight the section
            // with a thin yellow overlay.
            IDrawStyle thinYellow = new DrawStyle(Color.Yellow);

            foreach (ISpatialObject so in Items)
            {
                LineFeature line = (so as LineFeature);
                if (line!=null && line.Topology is SectionTopologyList)
                {
                    SectionTopologyList sections = (line.Topology as SectionTopologyList);
                    IDivider d = sections.FindClosestSection(m_SearchPosition);
                    if (d!=null)
                        d.LineGeometry.Render(display, thinYellow);
                }
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
            // If this is a single-item selection that refers to a line, and which has
            // a specific reference position, all selections are considered to be different.
            ISpatialObject so = Item;
            if (m_SearchPosition!=null && so!=null && so.SpatialType==SpatialType.Line)
                return false;

            return base.Equals(that);
        }
    }
}
