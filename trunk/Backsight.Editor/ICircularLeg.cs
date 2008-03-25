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
    /// Information about a circular leg in a connection path
    /// </summary>
    interface ICircularLeg : ILeg
    {
        /// <summary>
        /// Is the leg directed clockwise?
        /// </summary>
        bool IsClockwise { get; }

        /// <summary>
        /// The observed radius, in meters
        /// </summary>
        double Radius { get; }

        /// <summary>
        /// Is the leg flagged as a cul-de-sac?
        /// </summary>
        bool IsCulDeSac { get; }

        /// <summary>
        /// Given the position of the start of this leg, along with an initial bearing, get
        /// other positions (and bearings) relating to the circle.
        /// </summary>
        /// <param name="bc">The position for the BC.</param>
        /// <param name="sbearing">The position for the BC.</param>
        /// <param name="sfac">Scale factor to apply to distances.</param>
        /// <param name="center">Position of the circle centre.</param>
        /// <param name="bear2bc">Bearing from the centre to the BC.</param>
        /// <param name="ec">Position of the EC.</param>
        /// <param name="ebearing">Exit bearing.</param>
        void GetPositions(IPosition bc, double sbearing, double sfac,
                            out IPosition center, out double bear2bc, out IPosition ec, out double ebearing);
    }
}
