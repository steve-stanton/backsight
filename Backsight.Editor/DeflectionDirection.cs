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
	/// <written by="Steve Stanton" on="09-JUN-1999" />
    /// <summary>A deflection angle.</summary>
    [Serializable]
    class DeflectionDirection : AngleDirection
    {
        #region Class data

        // no data

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="backsight">The backsight point.</param>
        /// <param name="occupied">The occupied station.</param>
        /// <param name="observation">The angle to an observed point, measured with respect
        /// to the projection of an orientation line defined by the backsight and the occupied
        /// station. Positive values indicate a clockwise rotation & negated values for
        /// counter-clockwise.
        /// </param>
        internal DeflectionDirection(PointFeature backsight, PointFeature occupied, IAngle observation)
            : base(backsight, occupied, observation)
        {
        }

        #endregion

        /// <summary>
        /// The angle as a bearing
        /// </summary>
        internal override IAngle Bearing
        {
            get
            {
                // Get the bearing to the backsight
                double bb = Geom.BearingInRadians(this.Backsight, this.From);

                // Add on the observed angle, and restrict to [0,2*PI]
                double a = bb + this.Observation.Radians;
                return new RadianValue(Direction.Normalize(a));
            }
        }

        internal override DirectionType DirectionType
        {
            get { return DirectionType.Deflection; }
        }
    }
}
