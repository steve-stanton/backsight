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

namespace Backsight
{
	/// <written by="Steve Stanton" on="19-FEB-1998" />
    /// <summary>
    /// A turn is the angle from some reference bearing to something else.
    /// </summary>
    public class Turn
    {
        #region Class data

        /// <summary>
        /// The origin of the turn.
        /// </summary>
        private readonly IPosition m_Origin;

        /// <summary>
        /// The reference position.
        /// </summary>
        private readonly IPosition m_Reference;

        /// <summary>
        /// The bearing from the origin to the reference position, in radians
        /// </summary>
        private readonly double m_Bearing;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Turn</c> with the supplied origin and reference positions
        /// </summary>
        /// <param name="origin">The origin for the turn.</param>
        /// <param name="reference">The position of the reference point.</param>
        public Turn(IPosition origin, IPosition reference)
        {
	        m_Origin = origin;
	        m_Reference = reference;
            m_Bearing = BasicGeom.BearingInRadians(m_Origin, m_Reference);
        }

        #endregion

        public double GetAngle(Turn that)
        {
            return GetAngleInRadians(that.BearingInRadians, 0.0);
        }

        public double BearingInRadians
        {
            get { return m_Bearing; }
        }

        /// <summary>
        /// Returns the clockwise angle between the reference direction and a position.
        /// </summary>
        /// <param name="pos">The position we want the clockwise angle to.</param>
        /// <returns>The clockwise angle, in radians.</returns>
        public double GetAngleInRadians(IPosition pos)
        {
            return GetAngleInRadians(pos, 0.0);
        }

        /// <summary>
        /// Returns the clockwise angle between the reference direction and a position.
        /// </summary>
        /// <param name="pos">The position we want the clockwise angle to.</param>
        /// <param name="angtol">Angular tolerance. If you specify a non-zero value, and the position is
        ///	within tolerance of the reference bearing, you will get back a value of zero.</param>
        /// <returns>The clockwise angle, in radians.</returns>
        public double GetAngleInRadians(IPosition pos, double angtol)
        {
            // Get the bearing of the specified position.
            double bearing = BasicGeom.BearingInRadians(m_Origin, pos);

            // Return the angle.
            return GetAngleInRadians(bearing, angtol);
        }

        /// <summary>
        /// Calls <c>GetAngle(double, double)</c> with an angular tolerance of zero.
        /// </summary>
        /// <param name="bearing">The bearing we want the angle to (in radians)</param>
        /// <returns></returns>
        public double GetAngleInRadians(double bearing)
        {
            return GetAngleInRadians(bearing, 0.0);
        }

        /// <summary>
        /// Returns the clockwise angle between the reference direction and a value
        /// representing a bearing that was measured from the same origin.
        /// </summary>
        /// <param name="bearing">The bearing we want the angle to (in radians)</param>
        /// <param name="angtol">Angular tolerance for match with reference direction.</param>
        /// <returns></returns>
        double GetAngleInRadians(double bearing, double angtol)
        {
            // Get the clockwise angle with respect to the reference bearing.
	        double angle;
            double thisVal = m_Bearing;
            double thatVal = bearing;
	        if (thatVal < thisVal)
		        angle = MathConstants.PIMUL2 + thatVal - thisVal;
	        else
		        angle = thatVal - thisVal;

	        if (angle < MathConstants.TINY)
                angle = 0.0;

            // If an angular tolerance has been specified, check whether
            // the angle is sufficiently close to the reference bearing.
            // Otherwise return an angle in the range [0,PIMUL2).

	        if ( angtol > MathConstants.TINY &&
		        (angle<angtol || Math.Abs(angle-MathConstants.PIMUL2)<angtol) )
		        return 0.0;
	        else
		        return angle;
        }
    }
}
