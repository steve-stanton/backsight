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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="10-FEB-1998" was="CeControl" />
    /// <summary>
    /// A control point.
    /// </summary>
    class ControlPoint : Position3D, IEquatable<ControlPoint>
    {
        #region Static

        /// <summary>
        /// Attempts to parse a string that may represent a control point.
        /// </summary>
        /// <param name="s">String read from external control file.</param>
        /// <param name="control">The point created (if successfully parsed)</param>
        /// <returns>True if control point was parsed ok</returns>
        internal static bool TryParse(string s, out ControlPoint control)
        {
            try
            {
                control = CreateInstance(s);
                return true;
            }

            catch (ArgumentException) { }
            control = null;
            return false;
        }

        /// <summary>
        /// Creates a <c>ControlPoint</c> based on the supplied string.
        /// </summary>
        /// <param name="s">String read from external control file.</param>
        /// <returns>The created point</returns>
        /// <exception cref="ArgumentException">If the supplied string cannot be parsed as
        /// a control point</exception>
        /// <remarks>Assumes Manitoba control file</remarks>
        static ControlPoint CreateInstance(string s)
        {
            // The string must contain at least 77 characters.
            if (s.TrimEnd().Length < 77)
                throw new ArgumentException("Control point string must contain at least 77 characters");

            // Characters 7:13 (zero-based) must contain a numeric value.
            string t = s.Substring(7, 7).Trim();
            int xid;
            if (!Int32.TryParse(t, out xid) || xid<0)
                throw new ArgumentException("Control point string doesn't contain valid ID field");

            // Characters 35:38 contain the UTM zone number.
            t = s.Substring(35, 4).Trim();
            int zone;
            if (!Int32.TryParse(t, out zone) || zone<=0 || zone>60)
                throw new ArgumentException("Control point string doesn't contain valid UTM zone");

            // Characters 51:60 contain the easting.
            t = s.Substring(51, 10).Trim();
            double easting;
            if (!Double.TryParse(t, out easting) || easting<Constants.TINY)
                throw new ArgumentException("Control point string doesn't contain valid easting");

            // Characters 66:76 contain the northing.
            t = s.Substring(66, 11).Trim();
            double northing;
            if (!Double.TryParse(t, out northing) || northing < Constants.TINY)
                throw new ArgumentException("Control point string doesn't contain valid northing");

            // Everything looks ok, so define the object.
            return new ControlPoint((uint)xid, easting, northing, 0.0, (byte)zone);
        }

        #endregion

        #region Class data

        /// <summary>
        /// The ID of the control point.
        /// </summary>
        uint m_ControlId;

        /// <summary>
        /// The zone number (in the range [1,60])
        /// </summary>
        byte m_Zone;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>ControlPoint</c>
        /// </summary>
        /// <param name="id">The ID of the control point</param>
        /// <param name="x">The easting of the position</param>
        /// <param name="y">The northing of the position</param>
        /// <param name="z">The elevation of the position</param>
        /// <param name="zone">The zone number (in the range [1,60])</param>
        private ControlPoint(uint id, double x, double y, double z, byte zone)
            : base(x,y,z)
        {
            m_ControlId = id;
            m_Zone = zone;
        }

        #endregion

        /// <summary>
        /// Is the position of this control point defined?
        /// </summary>
        internal bool IsDefined
        {
            get { return (X > Constants.TINY); }
        }

        /// <summary>
        /// Does this control point equal the supplied control point?
        /// </summary>
        /// <param name="that">The control point to compare with</param>
        /// <returns>True if this control point has the same ID as <paramref name="that"/></returns>
        public bool Equals(ControlPoint that) // IEquatable<ControlPoint>
        {
            return (this.m_ControlId == that.m_ControlId);
        }

        /// <summary>
        /// The ID of the control point (should be unique)
        /// </summary>
        internal uint ControlId
        {
            get { return m_ControlId; }
        }

        /// <summary>
        /// Checks if this control point falls inside a control range.
        /// </summary>
        /// <param name="range">The control range to compare with.</param>
        /// <returns>True if this control point is in the range.</returns>
        internal bool IsInRange(ControlRange range)
        {
            return range.IsEnclosing(m_ControlId);
        }

        /// <summary>
        /// Draws this control point on the specified display.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The style for the drawing</param>
        internal void Render(ISpatialDisplay display, IDrawStyle style)
        {
            if (IsDefined)
                style.RenderTriangle(display, this);
        }
    }
}
