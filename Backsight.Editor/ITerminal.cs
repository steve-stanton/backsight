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
	/// <written by="Steve Stanton" on="05-JUL-2007" />
    /// <summary>
    /// A position at one end of a polygon divider. Implemented by
    /// the <see cref="PointFeature"/> and <see cref="Intersection"/> classes.
    /// </summary>
    interface ITerminal : IPointGeometry
    {
        /// <summary>
        /// The dividers that start or end at the terminal. If a divider
        /// starts and also ends at the terminal, it should appear in the
        /// returned array just once.
        /// </summary>
        IDivider[] IncidentDividers();

        /// <summary>
        /// Associates this terminal with an additional divider
        /// </summary>
        /// <param name="d">The divider the terminal should be referred to</param>
        //void AddDivider(SectionDivider d);

        /// <summary>
        /// Go through each divider that is incident on this terminal, marking adjacent
        /// polygons for deletion.
        /// </summary>
        void MarkPolygons();
    }
}
