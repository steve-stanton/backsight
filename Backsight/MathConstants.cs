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
    public class MathConstants
    {
        public const double PI = System.Math.PI; // 3.14159265358979323846
        public const double PIDIV2 = PI/2.0;
        public const double PIDIV4 = PI/4.0;
        public const double PIMUL1P5 = PI*1.5;
        public const double PIMUL2 = PI*2.0;

        /// <summary>
        /// As good as zero
        /// </summary>
        public const double TINY = 10e-38;

        /// <summary>
        /// Multiplier to convert from radians to decimal degrees.
        /// </summary>
        public const double RADTODEG = 360.0/PIMUL2;

        /// <summary>
        /// Multiplier to convert from decimal degrees to radians.
        /// </summary>
        public const double DEGTORAD = PIMUL2/360.0;
    }
}
