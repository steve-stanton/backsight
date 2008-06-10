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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology for a section of a line where it overlaps another line (or possibly several
    /// lines). In these situations, an overlap object is created to ensure that some
    /// sort of topology can be associated with the section. However, it does not know
    /// about adjacent polygons (you get back nulls, and an attempt to set them will
    /// lead to an exception).
    /// <para/>
    /// This class does not currently hold any information about the lines that are
    /// overlapped (since we may be dealing with complicated overlap scenarios, it is
    /// not immediately evident what sort of object model would be needed). If anything
    /// needs the information, it is assumed that the necessary information can be readily
    /// obtained by searching the map model for the overlaps.
    /// </summary>
    /// <seealso cref="LineOverlap"/>
    class SectionOverlap : SectionTopology
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SectionOverlap</c> that relates to a section of the specified line.
        /// </summary>
        /// <param name="line">The line the overlap topology is for (this is <b>not</b>
        /// the line that is overlapped by this section)</param>
        /// <param name="from">The start position for the section.</param>
        /// <param name="to">The end position for the section.</param>
        internal SectionOverlap(LineFeature line, ITerminal from, ITerminal to)
            : base(line, from, to)
        {
        }

        #endregion

        /// <summary>
        /// The polygon ring to the left of the section. This implementation returns null (always).
        /// An attempt to set the polygon ring will lead to an <c>InvalidOperationException</c>.
        /// </summary>
        public override Ring Left // IDivider
        {
            get { return null; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// The polygon ring to the right of the section. This implementation returns null (always).
        /// An attempt to set the polygon ring will lead to an <c>InvalidOperationException</c>.
        /// </summary>
        public override Ring Right // IDivider
        {
            get { return null; }
            set { throw new InvalidOperationException(); }
        }

        /// <summary>
        /// Returns <c>true</c> (always), indicating that this divider represents
        /// some sort of overlap.
        /// </summary>
        public override bool IsOverlap // IDivider
        {
            get { return true; }
        }
    }
}
