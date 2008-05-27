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
    /// The length of something on the ground
    /// </summary>
    public class Length : ILength
    {
        public static Length Zero = new Length(0.0);

        #region Class data

        private readonly double m_Value;

        #endregion

        #region Constructors

        public Length(double meters)
        {
            m_Value = meters;
        }

        #endregion

        public double Meters
        {
            get { return m_Value; }
        }

        public long Microns
        {
            get { return ToMicrons(m_Value); }
        }

        /// <summary>
        /// Converts a value in meters on the ground into microns
        /// </summary>
        /// <param name="v">The value in meters to convert</param>
        /// <returns>The corresponding value in microns</returns>
        public static long ToMicrons(double v)
        {
            return (long)Math.Round(v*1000000.0, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Converts a value from microns on the ground into meters
        /// </summary>
        /// <param name="v">The value in microns to convert</param>
        /// <returns>The corresponding value in meters</returns>
        public static double ToMeters(long v)
        {
            return ((double)v) / 1000000.0;
        }

        public override string ToString()
        {
            return String.Format("{0:0.000000}", Meters);
        }

        #region IXmlContent Members

        public void WriteContent(XmlContentWriter writer)
        {
            writer.WriteString("Value", m_Value.ToString());
        }

        #endregion
    }
}
