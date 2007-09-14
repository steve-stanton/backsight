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

using Backsight.Geometry;

namespace Backsight
{
	/// <written by="Steve Stanton" on="14-SEP-2006" />
    /// <summary>
    /// The 2D rectangular extent of something.
    /// </summary>
    [Serializable]
    public class Window : IEditWindow, IEquatable<Window>
    {
        #region Class data

        /// <summary>
        /// The position of the south-west corner.
        /// </summary>
        Position m_Min;

        /// <summary>
        /// The position of the north-east corner.
        /// </summary>
        Position m_Max;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates an empty (undefined) window.
        /// </summary>
        public Window()
        {
            SetEmpty();
        }

        /// <summary>
        /// Creates a new <c>Window</c> that encloses a pair of positions.
        /// </summary>
        /// <param name="x1">Easting of first position.</param>
        /// <param name="y1">Northing of first position.</param>
        /// <param name="x2">Easting of second position.</param>
        /// <param name="y2">Northing of second position.</param>
        public Window(double x1, double y1, double x2, double y2)
            : this()
        {
            Union(x1, y1);
            Union(x2, y2);
        }

        /// <summary>
        /// Creates a <c>Window</c> that coincides with the supplied positions
        /// </summary>
        /// <param name="p">The position the extent will cover</param>
        public Window(IPosition p)
            : this(p, p)
        {
        }

        public Window(IPosition a, IPosition b)
            : this(a.X, a.Y, b.X, b.Y)
        {
        }

        public Window(IWindow copy)
        {
            if (copy==null || copy.IsEmpty)
                SetEmpty();
            else
            {
                m_Min = new Position(copy.Min);
                m_Max = new Position(copy.Max);
            }
        }

        /// <summary>
        /// Constructor accepting two windows. The result is a window
        /// for the area of common overlap (if any). Use <c>IsEmpty</c> to
        /// check if there is no overlap.
        /// 
        /// If all you want is to test for overlap, use <c>IsOverlap</c> instead.
        /// </summary>
        /// <param name="e1">First window</param>
        /// <param name="e2">Second window</param>
        public Window(IWindow e1, IWindow e2)
        {
            m_Min = new Position(Math.Max(e1.Min.X, e2.Min.X), Math.Max(e1.Min.Y, e2.Min.Y));
            m_Max = new Position(Math.Min(e1.Max.X, e2.Max.X), Math.Min(e1.Max.Y, e2.Max.Y));

            if (m_Min.X > m_Max.X || m_Min.Y > m_Max.Y)
                SetEmpty();
        }

        /// <summary>
        /// Constructor for creating a square window centered at a specific position
        /// </summary>
        /// <param name="center">The center of the extent</param>
        /// <param name="size">The width and height of the extent</param>
        public Window(IPosition center, double size)
            : this(center.X-size*0.5, center.Y-size*0.5, center.X+size*0.5, center.Y+size*0.5)
        {
        }

        /// <summary>
        /// Creates a new <c>Window</c> that encloses an array of positions.
        /// </summary>
        /// <param name="points">The positions to use to define the extent</param>
        public Window(IPosition[] points)
        {
            SetEmpty();

            foreach (IPosition p in points)
                this.Union(p);
        }

        #endregion

        /// <summary>
        /// The position of the south-west corner.
        /// </summary>
        public IPosition Min
        {
            get { return m_Min; }
        }

        /// <summary>
        /// The position of the north-east corner.
        /// </summary>
        public IPosition Max
        {
            get { return m_Max; }
        }

        /// <summary>
        /// The height of this window, in meters (<c>Double.NaN</c> if the <see cref="IsEmpty"/> property is true).
        /// </summary>
        public double Height
        {
            get { return (this.IsEmpty ? double.NaN : (m_Max.Y-m_Min.Y)); }
        }

        /// <summary>
        /// The width of this window, in meters (<c>Double.NaN</c> if the <see cref="IsEmpty"/> property is true).
        /// </summary>
        public double Width
        {
            get { return (this.IsEmpty ? double.NaN : (m_Max.X-m_Min.X)); }
        }

        /// <summary>
        /// Is this window empty. An empty window is an undefined window.
        /// </summary>
        /// <seealso cref="IsPoint"/>
        public bool IsEmpty
        {
            get { return (m_Min==null); }
        }

        /// <summary>
        /// Does this window only cover a point in space?
        /// </summary>
        /// <seealso cref="IsEmpty"/>
        public bool IsPoint
        {
            get { return (this.IsEmpty ? false : (this.Width < Double.Epsilon && this.Height < Double.Epsilon)); }
        }

        /// <summary>
        /// The position at the center of this window (null if the <see cref="IsEmpty"/> property is true).
        /// </summary>
        public IPosition Center
        {
            get { return (this.IsEmpty ? null : new Position((m_Min.X+m_Max.X)*0.5, (m_Min.Y+m_Max.Y)*0.5)); }
        }

        public override string ToString()
        {
            return String.Format("Min [{0:0.000}E, {1:0.000}N]  Max [{2:0.000}E, {3:0.000}N]", m_Min.X, m_Min.Y, m_Max.X, m_Max.Y);
        }

        public bool Equals(Window other)
        {
            if (this.IsEmpty)
                return other.IsEmpty;

            if (other.IsEmpty)
                return false;

            return (Math.Abs(m_Min.X - other.m_Min.X) < Double.Epsilon
            &&      Math.Abs(m_Min.Y - other.m_Min.Y) < Double.Epsilon
            &&      Math.Abs(m_Max.X - other.m_Max.X) < Double.Epsilon
            &&      Math.Abs(m_Max.Y - other.m_Max.Y) < Double.Epsilon);
        }

        public void SetEmpty()
        {
            m_Min = m_Max = null;
        }

        public void Union(IWindow other)
        {
            if (!other.IsEmpty)
            {
                Union(other.Min.X, other.Min.Y);
                Union(other.Max.X, other.Max.Y);
            }
        }

        public void Union(IPosition position)
        {
            Union(position.X, position.Y);
        }

        public void Union(double x, double y)
        {
            if (this.IsEmpty)
            {
                m_Min = new Position(x, y);
                m_Max = new Position(x, y);
            }
            else
            {
                m_Min.X = Math.Min(m_Min.X, x);
                m_Min.Y = Math.Min(m_Min.Y, y);
                m_Max.X = Math.Max(m_Max.X, x);
                m_Max.Y = Math.Max(m_Max.Y, y);
            }
        }

        /// <summary>
        /// Checks if two windows overlap (or touch).
        /// </summary>
        /// <param name="e">The extent to compare with</param>
        /// <returns>True if this extent overlaps (or touches) the supplied extent</returns>
        public bool IsOverlap(IWindow e)
        {
            if (Math.Max(m_Min.X, e.Min.X) > Math.Min(m_Max.X, e.Max.X) ||
		        Math.Max(m_Min.Y, e.Min.Y) > Math.Min(m_Max.Y, e.Max.Y))
                return false;
            else
                return true;
        }

        /// <summary>
        /// Checks whether a position falls inside (or on) this extent.
        /// </summary>
        /// <param name="p">The position to test</param>
        /// <returns>True if the supplied position is inside (or on the edge of) this extent</returns>
        public bool IsOverlap(IPosition p)
        {
            double x = p.X;
            double y = p.Y;

            // May want to have some sort of tolerance here
            return (x>=m_Min.X && x<=m_Max.X && y>=m_Min.Y && y<=m_Max.Y);
        }

        /// <summary>
        /// Checks whether this window is entirely enclosed by another window.
        /// </summary>
        /// <param name="other">The window to compare with this one</param>
        /// <returns>True is this window is entirely enclosed</returns>
        public bool IsEnclosedBy(IWindow other)
        {
            return (other.Min.X <= this.Min.X
            &&      other.Min.Y <= this.Min.Y
            &&      other.Max.X >= this.Max.X
            &&      other.Max.Y >= this.Max.Y);
        }

        /// <summary>
        /// Expands this extent by a factor. For example, if you want a
        /// 10% margin all the way round the extent, specify a factor
        /// of 0.1. Specify a negated factor to shrink the extent.
        /// Does nothing if this extent is undefined.
        /// </summary>
        /// <param name="factor">The expansion factor</param>
        public void Expand(double factor)
        {
            if (this.IsEmpty)
                return;

            // Figure out how much to expand (double, since the margin is on both sides)
            double dx = this.Width;
            double dy = this.Height;
            dx += (dx * factor * 2.0);
            dy += (dy * factor * 2.0);

            // For negated factors, never allow the window to shrink too much
            if (factor < 0.0)
            {
                dx = Math.Max(0.0, dx);
                dy = Math.Max(0.0, dy);
            }

            // Apply offsets from the centre
            IPosition centre = this.Center;
            m_Min.X = centre.X - (dx*0.5);
            m_Min.Y = centre.Y - (dy*0.5);
            m_Max.X = m_Min.X + dx;
            m_Max.Y = m_Min.Y + dy;
        }

        public void Expand(ILength d)
        {
            double value = d.Meters;

            m_Min.X -= value;
            m_Min.Y -= value;
            m_Max.X += value;
            m_Max.Y += value;

            if (value < 0.0)
            {
                if (m_Min.X > m_Max.X)
                    m_Min.X = m_Max.X = 0.5 * (m_Min.X + m_Max.X);

                if (m_Min.Y > m_Max.Y)
                    m_Min.Y = m_Max.Y = 0.5 * (m_Min.Y + m_Max.Y);
            }
        }

        /// <summary>
        /// Translates this extent
        /// </summary>
        /// <param name="dx">X-shift to add</param>
        /// <param name="dy">Y-shift to add</param>
        internal void Shift(double dx, double dy)
        {
            m_Min.X += dx;
            m_Min.Y += dy;
            m_Max.X += dx;
            m_Max.Y += dy;
        }
    }
}
