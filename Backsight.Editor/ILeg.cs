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
    /// <written by="Steve Stanton" on="25-MAR-2008"/>
    /// <summary>
    /// Basic information about a leg in a connection path
    /// </summary>
    interface ILeg
    {
        /// <summary>
        /// The total length of this leg
        /// </summary>
        ILength Length { get; }

        /// <summary>
        /// Gets the total observed length of this leg
        /// </summary>
        /// <returns>The sum of the observed lengths for this leg, in meters on the ground</returns>
        double GetTotal();

        /// <summary>
        /// The number of distance observations for this leg.
        /// May be zero in the case of cul-de-sacs (if defined only
        /// in terms of a center point and central angle).
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Is a specific span in this leg associated with a line feature?
        /// </summary>
        /// <param name="index">The index of the span in question.</param>
        /// <returns>True if line feature will be produced.</returns>
        bool HasLine(int index);

        /// <summary>
        /// Is a specific span in this leg associated with a terminating point feature?
        /// </summary>
        /// <param name="index">The index of the span in question.</param>
        /// <returns>True if a point feature will be produced.</returns>
        bool HasEndPoint(int index);

        /// <summary>
        /// Gets the observed distance to the start and end of a specific
        /// span, in meters on the ground.
        /// </summary>
        /// <param name="index">Index of the required span.</param>
        /// <param name="sdist">Distance to the start of the span.</param>
        /// <param name="edist">Distance to the end of the span.</param>
        void GetDistances(int index, out double sdist, out double edist);

        /// <summary>
        /// The circle (if any) that this leg sits on. Defined only for instances of
        /// <see cref="CircularLeg"/>.
        /// </summary>
        Circle Circle { get; }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing,
        /// project the end of the leg, along with an exit bearing.
        /// </summary>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg.
        /// Updated for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances (default=1.0).</param>
        void Project(ref IPosition pos, ref double bearing, double sfac);

        /// <summary>
        /// Draws this leg
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="pos">The position for the start of the leg. Updated to be
        /// the position for the end of the leg.</param>
        /// <param name="bearing">The bearing at the end of the previous leg. Updated
        /// for this leg.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        void Render(ISpatialDisplay display, ref IPosition terminal, ref double bearing, double sfac);
    }
}
