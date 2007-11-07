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

namespace Backsight.Index
{
    /// <written by="Steve Stanton" on="20-DEC-2006" />
    /// <summary>
    /// Some sort of positional range (where the range is expressed as an unsigned 64-bit integer).
    /// </summary>
    struct RangeValue
    {
        #region Class data

        /// <summary>
        /// The axis the range relates to.
        /// </summary>
        private readonly Dimension m_Dimension;

        /// <summary>
        /// The low end of the range (inclusive).
        /// </summary>
        private ulong m_Min;

        /// <summary>
        /// The high end of the range (inclusive).
        /// </summary>
        private ulong m_Max;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Range</c>
        /// </summary>
        /// <param name="d">The positional dimension the range refers to</param>
        /// <param name="a">One end of the range (either the min or the max)</param>
        /// <param name="b">The other end of the range (either the min or the max)</param>
        internal RangeValue(Dimension d, ulong a, ulong b)
        {
            m_Dimension = d;

            if (a<b)
            {
                m_Min = a;
                m_Max = b;
            }
            else
            {
                m_Min = b;
                m_Max = a;
            }
        }

        /// <summary>
        /// Creates a new <c>Range</c>, converting the supplied values into unsigned space.
        /// </summary>
        /// <param name="d">The positional dimension the range refers to</param>
        /// <param name="a">One end of the range (either the min or the max)</param>
        /// <param name="b">The other end of the range (either the min or the max)</param>
        internal RangeValue(Dimension d, long a, long b)
            : this(d, SpatialIndex.ToUnsigned(a), SpatialIndex.ToUnsigned(b))
        {
        }

        #endregion

        /// <summary>
        /// Checks whether a pair of ranges overlap
        /// </summary>
        /// <param name="that">The range to compare with this one</param>
        /// <returns>True if the ranges refer to the same positional dimension and they overlap.
        /// False if they refer to different dimensions, or they don't overlap (or only touch).
        /// </returns>
        internal bool IsOverlap(RangeValue that)
        {
            if (this.m_Dimension!=that.m_Dimension)
                return false;

            if (that.m_Min==that.m_Max)
                return (this.m_Min<that.m_Min && that.m_Max<this.m_Max);

            return (Math.Max(this.m_Min, that.m_Min) < Math.Min(this.m_Max, that.m_Max));
        }

        /// <summary>
        /// The axis the range relates to.
        /// </summary>
        internal Dimension Dimension
        {
            get { return m_Dimension; }
        }

        /// <summary>
        /// Override displays the range as fixed-width hexadecimal.
        /// </summary>
        public override string ToString()
        {
            return String.Format("[Min={0:X016} Max={1:X016}]", m_Min, m_Max);
        }

        /// <summary>
        /// The low end of the range (inclusive).
        /// </summary>
        internal ulong Min
        {
            get { return m_Min; }
        }

        /// <summary>
        /// The high end of the range (inclusive).
        /// </summary>
        internal ulong Max 
        {
            get { return m_Max;  }
        }

        /// <summary>
        /// The size of this range (inclusive of one end only).
        /// </summary>
        internal ulong Size
        {
            get { return (m_Max-m_Min); }
        }

        /// <summary>
        /// Increase both ends of this range by the specified amount.
        /// </summary>
        /// <param name="delta">The amount to add to both ends of the range</param>
        internal void Increase(ulong delta)
        {
            m_Min += delta;
            m_Max += delta;
        }
    }
}
