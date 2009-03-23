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

using Backsight.Editor.Xml;


namespace Backsight.Editor.Observations
{
	/// <written by="Steve Stanton" on="23-OCT-1997" />
    /// <summary>
    /// A distance observation.
    /// </summary>
    class Distance : Observation, ILength, IEquatable<Distance>
    {
        #region Static

        /// <summary>
        /// Attempts to parse the supplied string.
        /// </summary>
        /// <param name="s">The string to parse</param>
        /// <param name="d">The result of the parse attempt (null if the string cannot
        /// be parsed as a distance)</param>
        /// <returns>True if <paramref name="d"/> was successfully defined</returns>
        internal static bool TryParse(string s, out Distance d)
        {
            Distance t = new Distance(s);
            d = (t.IsDefined ? t : null);
            return t.IsDefined;
        }

        #endregion

        #region Class data

        /// <summary>
        /// The way the distance was originally specified by the user.
        /// </summary>
        DistanceUnit m_EnteredUnit;

        /// <summary>
        /// Observed distance, in meters on the ground.
        /// </summary>
        double m_ObservedMetric;

        /// <summary>
        /// Is the distance fixed? (if so, the distance cannot be adjusted in any way).
        /// </summary>
        bool m_IsFixed;

        #endregion

        #region Constructors

        internal Distance()
        {
            m_EnteredUnit = null;
            m_ObservedMetric = 0.0;
            m_IsFixed = false;
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <param name="t">The serialized version of this observation</param>
        internal Distance(Operation op, DistanceType t)
            : base(op, t)
        {
            DistanceUnitType dut = (DistanceUnitType)t.Unit;
            m_EnteredUnit = EditingController.Current.GetUnits(dut);
            m_ObservedMetric = m_EnteredUnit.ToMetric(t.Value);
            m_IsFixed = t.Fixed;
        }

        /// <summary>
        /// Creates a distance (regarded as non-fixed)
        /// </summary>
        /// <param name="distance">The entered distance value</param>
        /// <param name="unit">The units for the entered distance.</param>
        internal Distance(double distance, DistanceUnit unit) : this(distance, unit, false)
        {
        }

        /// <summary>
        /// Creates a distance
        /// </summary>
        /// <param name="distance">The entered distance value</param>
        /// <param name="unit">The units for the entered distance.</param>
        /// <param name="isFixed">Should the distance be treated as fixed?</param>
        Distance(double distance, DistanceUnit unit, bool isFixed)
        {
	        m_ObservedMetric = unit.ToMetric(distance);
	        m_EnteredUnit = unit;
            m_IsFixed = isFixed;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="copy">The distance to copy.</param>
        internal Distance(Distance copy) : base(copy)
        {
            m_ObservedMetric = copy.m_ObservedMetric;
            m_EnteredUnit = copy.m_EnteredUnit;
            m_IsFixed = copy.m_IsFixed;
        }

        /// <summary>
        /// Constructor that accepts a string. Use the <c>IsDefined</c> property to check
        /// whether the string was parsed ok. Also see <see cref="TryParse"/>.
        /// </summary>
        /// <param name="s">The string to parse. It should look like a floating
        ///	point number, but may have a units abbreviation stuck on the end (like that
        ///	produced by <c>Distance.Format</c>).</param>
        internal Distance(string str)
        {
            // Initialize with undefined values.
	        m_ObservedMetric = 0.0;
	        m_EnteredUnit = null;
	        m_IsFixed = false;

            // Ignore any trailing white space. Return if it's ALL white space.
            string s = str.Trim();
            if (s.Length==0)
                return;

            // Split the string into a numeric & abbreviation part
            string num, abbr;
            ParseDistanceString(str, out num, out abbr);

            // Try to convert the entered string to a float.
            double dval;
            if (!Double.TryParse(num, out dval))
                return;

            // If the abbreviation corresponds to some form a data entry, save the
            // entered distance in those. Otherwise save in the current data entry units.
            EditingController ec = EditingController.Current;
            if (abbr.Length > 0)
            {
                m_EnteredUnit = ec.GetUnit(abbr);
                if (m_EnteredUnit!=null)
                    m_ObservedMetric = m_EnteredUnit.ToMetric(dval);
            }
            else
            {
                DistanceUnit dunit = ec.EntryUnit;
                m_ObservedMetric = dunit.ToMetric(dval);
                m_EnteredUnit = dunit;
            }
        }

        #endregion

        void ParseDistanceString(string s, out string num, out string abbr)
        {
            // Working back from the end of the string, look for the first
            // character that isn't a letter (the numeric bit runs to there).

            for (int index=s.Length-1; index>=0; index--)
            {
                if (!Char.IsLetter(s[index]))
                {
                    num = s.Substring(0, index+1);
                    abbr = (index==s.Length-1 ? String.Empty : s.Substring(index+1));
                    return;
                }
            }

            num = s;
            abbr = String.Empty;
        }

        internal bool IsFixed
        {
            get { return m_IsFixed; }
        }

        internal void SetFixed()
        {
            m_IsFixed = true;
        }

        /// <summary>
        /// Has this distance been defined properly? (meaning the value
        /// for <c>EntryUnit</c> is not null).
        /// </summary>
        internal bool IsDefined
        {
            get { return (m_EnteredUnit!=null); }
        }

        /// <summary>
        /// The way the distance was originally specified by the user.
        /// </summary>
        internal DistanceUnit EntryUnit
        {
            get { return m_EnteredUnit; }
        }

        public double Meters
        {
            get { return m_ObservedMetric; }
        }

        public long Microns
        {
            get { return Backsight.Length.ToMicrons(m_ObservedMetric); }
        }

        /// <summary>
        /// Formats this distance in a specific unit of measurement.
        /// </summary>
        /// <param name="unit">The desired unit of measurement.</param>
        /// <param name="appendAbbrev">True if units abbreviation should be appended (default was TRUE)</param>
        /// <returns></returns>
        internal string Format(DistanceUnit unit, bool appendAbbrev)
        {
            return unit.Format(m_ObservedMetric, appendAbbrev);
        }

        /// <summary>
        /// Formats this distance in units that correspond to the original data entry unit
        /// (with units abbreviation appended).
        /// </summary>
        /// <returns>The formatted distance</returns>
        internal string Format()
        {
            return Format(true);
        }

        /// <summary>
        /// Formats this distance in units that correspond to the original data entry unit.
        /// </summary>
        /// <param name="appendAbbrev">True if units abbreviation should be appended.</param>
        /// <returns></returns>
        string Format(bool appendAbbrev)
        {
            return (m_EnteredUnit==null ?
                String.Empty : m_EnteredUnit.Format(m_ObservedMetric, appendAbbrev));
        }

        /// <summary>
        /// Returns this distance in a specific type of unit.
        /// </summary>
        /// <param name="unit">The desired unit of measurement.</param>
        /// <returns></returns>
        double GetDistance(DistanceUnit unit)
        {
            return unit.FromMetric(m_ObservedMetric);
        }

        /// <summary>
        /// Returns the distance in the way it was originally entered. 
        /// </summary>
        /// <returns></returns>
        double GetDistance()
        {
            return (m_EnteredUnit==null ? 0.0 : m_EnteredUnit.FromMetric(m_ObservedMetric));
        }

        /// <summary>
        /// Ensures that the stored distance is greater than 0. 
        /// </summary>
        /// <returns>True if the sign of the distance was changed.</returns>
        internal bool SetPositive()
        {
            if (m_ObservedMetric < 0.0)
            {
                m_ObservedMetric = -m_ObservedMetric;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Ensures that the stored distance is less than 0. 
        /// </summary>
        /// <returns>True if the sign of the distance was changed.</returns>
        internal bool SetNegative()
        {
            if (m_ObservedMetric > 0.0)
            {
                m_ObservedMetric = -m_ObservedMetric;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns the equivalent distance on the mapping plane. 
        /// </summary>
        /// <param name="from">The position the distance is measured from.</param>
        /// <param name="bearing">The bearing for the distance, in radians.</param>
        /// <param name="sys">The mapping system</param>
        /// <returns>The distance on the mapping plane.</returns>
        internal double GetPlanarMetric(IPosition from, double bearing, ICoordinateSystem sys)
        {
        	// Return zero if this distance is undefined.
	        if (!this.IsDefined)
                return 0.0;

            // Calculate approximation for the terminal position (treating
	        // this distance as a planar distance).
            IPosition to = Geom.Polar(from, bearing, m_ObservedMetric);

            // Use the approximate location to determine line scale factor
            double sfac = sys.GetLineScaleFactor(from,to);
        	return (m_ObservedMetric * sfac);
        }

        /// <summary>
        /// Retruns the equivalent distance on the mapping plane. 
        /// </summary>
        /// <param name="from">The position the distance is measured from.</param>
        /// <param name="to">The approximate end position.</param>
        /// <param name="sys">The mapping system</param>
        /// <returns>The distance on the mapping plane.</returns>
        internal double GetPlanarMetric(IPosition from, IPosition to, ICoordinateSystem sys)
        {
	        if (!this.IsDefined)
                return 0.0;

            double sfac = sys.GetLineScaleFactor(from,to);
        	return (m_ObservedMetric * sfac);
        }

        internal override bool HasReference(Feature feat)
        {
            return false;
        }

        internal override void OnRollback(Operation op)
        {
            // Nothing to do
        }

        internal override void AddReferences(Operation op)
        {
            // Nothing to do
        }

        /// <summary>
        /// Relational equality test
        /// </summary>
        /// <param name="that">The distance to compare with.</param>
        /// <returns>True if the distance values are the same, and the distances are
        /// either both fixed or both floating</returns>
        public bool Equals(Distance that) // IEquatable<Distance>
        {
            return (this.IsFixed == that.IsFixed &&
                    Math.Abs(this.Meters - that.Meters) < Constants.TINY);
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteDouble("Value", GetDistance());
            writer.WriteInt("Unit", (int)m_EnteredUnit.UnitType);
            writer.WriteBool("Fixed", m_IsFixed);
        }

        /// <summary>
        /// The observed distance value
        /// </summary>
        internal double ObservedValue
        {
            get { return GetDistance(m_EnteredUnit); }
        }

        /// <summary>
        /// The string that will be used as the xsi:type for this edit
        /// </summary>
        public override string XmlTypeName
        {
            get { return "DistanceType"; }
        }
    }
}
