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
using System.Drawing;

namespace Backsight.Editor
{
    /// <summary>
    /// Definition of a unit of measurement.  Three units will be defined when a
    /// cadastral map model is created (for meters, feet, and chains). The resultant
    /// objects will be held as part of the model.
    /// </summary>
    class DistanceUnit : IEquatable<DistanceUnit>
    {
        #region Class data

        /// <summary>
        /// Numeric identitifer for the distance unit.
        /// </summary>
        private readonly DistanceUnitType m_UnitCode;

        /// <summary>
        /// A name for the unit of measurement.
        /// </summary>
        private string m_UnitName;

        /// <summary>
        /// Accepted abbreviation (e.g. "ft"). May be an empty string (not null).
        /// </summary>
        private string m_Abbreviation;

        /// <summary>
        /// Scaling factor to convert an entered unit of this type to meters (i.e. 0.3048
        /// for feet to meters).
        /// </summary>
        private double m_Multiplier;

        /// <summary>
        /// The display colour.
        /// </summary>
        private Color m_Colour;

        #endregion

        #region Constructors

        /// <summary>
        /// Create a distance unit with the specified type.
        /// </summary>
        /// <param name="unitType">The type of unit to create.</param>
        internal DistanceUnit(DistanceUnitType unitType)
        {
            switch (unitType)
            {
                case DistanceUnitType.AsEntered:
                {
                    m_UnitCode = unitType;
                    m_UnitName = String.Empty;
                    m_Abbreviation = String.Empty;
                    m_Multiplier = 1.0;
                    m_Colour = Color.White;
                    break;
                }

                case DistanceUnitType.Feet:
                {
                    m_UnitCode = unitType;
                    m_UnitName = "Feet";
                    m_Abbreviation = "ft";
                    m_Multiplier = 0.3048;
                    m_Colour = Color.Red;
                    break;
                }

                case DistanceUnitType.Chains:
                {
                    m_UnitCode = unitType;
                    m_UnitName = "Chains";
                    m_Abbreviation = "ch";
                    m_Multiplier = 20.1168;
                    m_Colour = Color.Green;
                    break;
                }

                case DistanceUnitType.Meters:
                default:
                {
                    m_UnitCode = unitType;
                    m_UnitName = "Meters";
                    m_Abbreviation = "m";
                    m_Multiplier = 1.0;
                    m_Colour = Color.Black;
                    break;
                }
            }
        }

        #endregion

        /// <summary>
        /// The numeric code identifying this distance unit.
        /// </summary>
        public DistanceUnitType UnitType { get { return m_UnitCode; } }

        /// <summary>
        /// The name for the unit of measurement.
        /// </summary>
        public string Name { get { return m_UnitName; } }

        /// <summary>
        /// The accepted abbreviation (e.g. "ft"). May be an empty string (not null).
        /// </summary>
        public string Abbreviation { get { return m_Abbreviation; } }

        /// <summary>
        /// Converts a value in this distance unit to a metric value.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public double ToMetric(double value)
        {
            if (m_UnitCode==DistanceUnitType.Meters)
                return value;
            else
                return value*m_Multiplier;
        }

        /// <summary>
        /// Converts a value in meters into this distance unit.
        /// </summary>
        /// <param name="meters"></param>
        /// <returns></returns>
        public double FromMetric(double meters)
        {
            if (m_UnitCode==DistanceUnitType.Meters)
                return meters;
            else
                return meters/m_Multiplier;
        }

        // Provided for C++ transition (specifies default precision)
        public string Format(double meters, bool units)
        {
            return Format(meters, units, -1);
        }

        // Provided for C++ transition (specifies default appendAbbreviation & precision)
        public string Format(double meters)
        {
            return Format(meters, true, -1);
        }

        /// <summary>
        /// Formats a metric distance in this distance unit
        /// </summary>
        /// <param name="meters">The distance in meters</param>
        /// <param name="units">Should units abbreviation be appended?</param>
        /// <param name="prec">Fixed number of digits to appear after the decimal place. A value
        /// of -1 means that up to 6 digits should be used (fewer if the trailing digits are zeroes).</param>
        /// <returns>The formatted string</returns>
        public string Format(double meters, bool units, int prec)
        {
            string s, fmt;

            if (prec<0)
            {
                // Get result with a suitable number of digits after the decimal place
                    
                if (m_UnitCode==DistanceUnitType.Feet)
                    fmt = "{0:0.00}";
                else if (m_UnitCode==DistanceUnitType.Chains)
                    fmt = "{0:0.0000}";
                else
                    fmt = "{0:0.000}";

                s = String.Format(fmt, FromMetric(meters));

                // Where's the decimal place (I guess it SHOULD be there, maybe not if strange culture).
                // If we've got one, strip off any trailing "0" chars.
                int dotPos = s.IndexOf('.');
                if (dotPos>=0)
                {
                    while (s.EndsWith("0"))
                        s = s.Substring(0, s.Length-1);
                }

                // If we got to the decimal place, strip that off too.
                if(s.EndsWith("."))
                    s = s.Substring(0, s.Length-1);

                // Append units if necessary.
		        if ( units )
                    s += m_Abbreviation;
            }
            else
            {
                fmt = "{0:F"+prec+"}";
                s = String.Format(fmt, FromMetric(meters));
                if(units)
                    s += m_Abbreviation;
            }

            return s;
        }

        public override string ToString()
        {
            return m_UnitName;
        }

        /// <summary>
        /// Checks whether this distance unit is the same as another distance unit.
        /// </summary>
        /// <param name="that">The distance unit to compare with</param>
        /// <returns>True if the <see cref="UnitType"/> property values are the same</returns>
        public bool Equals(DistanceUnit that)
        {
            return (this.m_UnitCode == that.m_UnitCode);
        }
    }
}
