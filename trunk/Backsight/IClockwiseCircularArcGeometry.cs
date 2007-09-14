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

namespace Backsight
{
	/// <written by="Steve Stanton" on="28-NOV-2006" />
    /// <summary>
    /// A circular arc that is directed clockwise. Provided to make it
    /// convenient to draw arcs using GDI+
    /// </summary>
    public interface IClockwiseCircularArcGeometry
    {
        /// <summary>
        /// The circle the arc falls on.
        /// </summary>
        ICircleGeometry Circle { get; }

        /// <summary>
        /// The bearing from the centre of circle to the start of the arc
        /// </summary>
        IAngle StartBearing { get; }

        /// <summary>
        /// The angular length of the arc
        /// </summary>
        IAngle SweepAngle { get; }

        /// <summary>
        /// The point at the start of the clockwise arc.
        /// </summary>
        IPointGeometry First { get; }

        /// <summary>
        /// The point at the end of the clockwise arc.
        /// </summary>
        IPointGeometry Second { get; }
    }
}
