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

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="13-FEB-1998" was="CeXData" />
    /// <summary>
    /// Information about an intersection.
    /// </summary>
    public class IntersectionData : IComparable<IntersectionData>
    {
        #region Class data

        /// <summary>
        /// Initial intersection point.
        /// </summary>
        IPosition m_X1;

        /// <summary>
        /// Secondary intersection point.
        /// </summary>
        IPosition m_X2;

        /// <summary>
        /// A value to be used in sorting the intersections.
        /// </summary>
        double m_SortValue;

        /// <summary>
        /// The way this intersect info relates to a primary line.
        /// </summary>
        IntersectionType m_Context1;

        /// <summary>
        /// The way this intersect info relates to a secondary line.
        /// </summary>
        IntersectionType m_Context2;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        IntersectionData()
        {
            Reset();
        }

        /// <summary>
        /// Constructor for a simple intersection (with a default sort value of 0.0)
        /// </summary>
        /// <param name="x">The position of the intersection.</param>
        internal IntersectionData(IPosition x)
        {
            m_X1 = x;
            m_X2 = null;
            m_SortValue = 0.0;
            m_Context1 = 0;
            m_Context2 = 0;
        }

        /// <summary>
        /// Constructor for a simple intersection (with a default sort value of 0.0)
        /// </summary>
        /// <param name="xi">X-value of intersection.</param>
        /// <param name="yi">Y-value of intersection.</param>
        IntersectionData(double xi, double yi)
            : this(xi, yi, 0.0)
        {
        }

        /// <summary>
        /// Constructor for simple intersection with a specific sort value.
        /// </summary>
        /// <param name="xi">X-value of intersection.</param>
        /// <param name="yi">Y-value of intersection.</param>
        /// <param name="sortval">Some value to associate with the intersection</param>
        internal IntersectionData(double xi, double yi, double sortval)
        {
            m_X1 = new Position(xi, yi);
            m_X2 = null;
            m_SortValue = sortval;
            m_Context1 = 0;
            m_Context2 = 0;
        }

        /// <summary>
        /// Constructor for a grazing intersection. If the supplied positions are
        /// actually closer than the coordinate resolution (1 micron), a simple
        /// intersection will be defined.
        /// </summary>
        /// <param name="x1">X-value of 1st intersection.</param>
        /// <param name="y1">Y-value of 1st intersection.</param>
        /// <param name="x2">X-value of 2nd intersection.</param>
        /// <param name="y2">Y-value of 2nd intersection.</param>
        internal IntersectionData(double x1, double y1, double x2, double y2)
        {
            IPointGeometry p1 = new PointGeometry(x1, y1);
            IPointGeometry p2 = new PointGeometry(x2, y2);

            m_X1 = p1;
            if (!p1.IsCoincident(p2))
                m_X2 = p2;

            m_SortValue = 0.0;
            m_Context1 = 0;
            m_Context2 = 0;
        }

        /// <summary>
        /// Constructor for a grazing intersection. If the supplied positions are
        /// actually closer than the coordinate resolution (1 micron), a simple
        /// intersection will be defined.
        /// </summary>
        /// <param name="p1">The 1st intersection.</param>
        /// <param name="p2">The 2nd intersection.</param>
        IntersectionData(IPointGeometry p1, IPointGeometry p2)
        {
            m_X1 = p1;
            if (!p1.IsCoincident(p2))
                m_X2 = p2;

            m_SortValue = 0.0;
            m_Context1 = 0;
            m_Context2 = 0;
        }

        #endregion

        internal bool IsGraze
        {
            get { return (m_X1!=null && m_X2!=null); }
        }

        double SortValue
        {
            get { return m_SortValue; }
            set { m_SortValue = value; }
        }

        #region IComparable<IntersectionData> Members

        /// <summary>
        /// Collating function ensures that grazes sort to the end if the sort
        /// values are the same.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(IntersectionData other)
        {
            if (Math.Abs(m_SortValue - other.m_SortValue) < Constants.TINY)
                return (this.IsGraze ? 1 : 0); // THIS sorts 1st if it is NOT a graze
            else
                return m_SortValue.CompareTo(other.m_SortValue);
        }

        #endregion

        void Reverse()
        {
            IPosition temp = m_X1;
            m_X1 = m_X2;
            m_X2 = temp;

            // Not sure why this was commented out...
            //	UINT2 context = m_Context1;
            //	m_Context1 = m_Context2;
            //	m_Context2 = context;
        }

        /// <summary>
        /// Reverses the context codes.
        /// </summary>
        internal void ReverseContext()
        {
            if (m_Context1 != m_Context2)
            {
                IntersectionType temp = m_Context1;
                m_Context1 = m_Context2;
                m_Context2 = temp;
            }
        }

        internal IPosition P1
        {
            get { return m_X1; }
        }

        internal IPosition P2
        {
            get { return m_X2; }
        }

        /// <summary>
        /// Do two lines meet end to end?
        /// </summary>
        internal bool IsEndEnd
        {
            get
            {
                IntersectionType mask = (IntersectionType.TouchStart | IntersectionType.TouchEnd);
                return ((m_Context1 & mask)!=0 && (m_Context2 & mask)!=0);
            }
        }

        internal bool IsEnd
        {
            get { return ((m_Context1 & (IntersectionType.TouchStart | IntersectionType.TouchEnd)) != 0); }
        }

        internal bool IsStartGraze
        {
            get { return ((m_Context1 & IntersectionType.GrazeStart) != 0); }
        }

        internal bool IsEndGraze
        {
            get { return ((m_Context1 & IntersectionType.GrazeEnd) != 0); }
        }

        bool IsInteriorGraze
        {
            get { return ((m_Context1 & IntersectionType.GrazeOther) != 0); }
        }

        bool IsTotalGraze
        {
            get { return ((m_Context1 & IntersectionType.GrazeTotal) != 0); }
        }

        internal double GetDeltaSort(IntersectionData other)
        {
            return (other.m_SortValue - m_SortValue);
        }

        /// <summary>
        /// Return XY position of the 1st intersection. You get back (0,0) if the
        /// intersection has not been defined.
        /// </summary>
        /// <param name="x1">The X-value of the first intersection (0 if none)</param>
        /// <param name="y1">The Y-value of the first intersection (0 if none)</param>
        void GetXY1(out double x1, out double y1)
        {
            if (m_X1==null)
            {
                x1 = 0.0;
                y1 = 0.0;
            }
            else
            {
                x1 = m_X1.X;
                y1 = m_X1.Y;
            }
        }

        /// <summary>
        /// Return XY position of the 2nd intersection. You get back (0,0) if the
        /// intersection has not been defined.
        /// </summary>
        /// <param name="x2">The X-value of the first intersection (0 if none)</param>
        /// <param name="y2">The Y-value of the first intersection (0 if none)</param>
        void GetXY2(out double x2, out double y2)
        {
            if (m_X2==null)
            {
                x2 = 0.0;
                y2 = 0.0;
            }
            else
            {
                x2 = m_X2.X;
                y2 = m_X2.Y;
            }
        }

        /// <summary>
        /// Checks if this intersection refers to a specific position.
        /// The match has to be exact.
        /// </summary>
        /// <param name="p">The position to look for</param>
        /// <returns>True if the position was found.</returns>
        internal bool IsReferredTo(IPosition p)
        {
            // The value used here is intended to be consistent with the use
            // of 1 micron precision for positions

            if (m_X1!=null && m_X1.IsAt(p, 0.0000009))
                return true;

            if (m_X2!=null && m_X2.IsAt(p, 0.0000009))
                return true;

            return false;
        }

        /// <summary>
        /// Resets this intersection so that it has undefined values.
        /// </summary>
        internal void Reset()
        {
            m_X1 = null;
            m_X2 = null;
            m_SortValue = 0.0;
            m_Context1 = 0;
            m_Context2 = 0;
        }

        /// <summary>
        /// Is this intersection defined? (meaning it has a defined position for the 1st intersection)
        /// </summary>
        internal bool IsDefined
        {
            get { return (m_X1!=null); }
        }

        /// <summary>
        /// Define the relationship that this intersection has to a pair of lines.
        /// </summary>
        /// <param name="line1">The 1st line.</param>
        /// <param name="line2">The 2nd line.</param>
        internal void SetContext(LineFeature line1, LineFeature line2)
        {
	        m_Context1 = 0;
	        m_Context2 = 0;

	        if (this.IsGraze)
            {
                IPointGeometry loc1 = new PointGeometry(m_X1);
                IPointGeometry loc2 = new PointGeometry(m_X2);
		        m_Context1 = GetContext(loc1,loc2,line1);
		        m_Context2 = GetContext(loc1,loc2,line2);
	        }
	        else
            {
                IPointGeometry loc = new PointGeometry(m_X1);
		        m_Context1 = GetContext(loc,line1);
		        m_Context2 = GetContext(loc,line2);
	        }
        }

        /// <summary>
        /// Returns the context code for a simple intersection with a line.
        /// </summary>
        /// <param name="loc">The location of the intersection.</param>
        /// <param name="line">The line to compare with.</param>
        /// <returns>The context code.</returns>
        static IntersectionType GetContext(IPointGeometry loc, LineFeature line)
        {
            IntersectionType context = 0;

            if (loc.IsCoincident(line.Start))
                context |= IntersectionType.TouchStart;

            if (loc.IsCoincident(line.End))
                context |= IntersectionType.TouchEnd;

            if (context==0)
                context = IntersectionType.TouchOther;

            return context;
        }

        /// <summary>
        /// Returns the context code for a grazing intersection with a line.
        /// </summary>
        /// <param name="loc1">The 1st intersection.</param>
        /// <param name="loc2">The 2nd intersection.</param>
        /// <param name="line">The line the context code is for.</param>
        /// <returns>The context code.</returns>
        static IntersectionType GetContext(IPointGeometry loc1, IPointGeometry loc2, LineFeature line)
        {
            // Get the context of the start and end of the graze.
            IntersectionType context1 = GetContext(loc1, line);
            IntersectionType context2 = GetContext(loc2, line);

            if (context1 == IntersectionType.TouchOther)
            {
                if (context2 == IntersectionType.TouchStart)
                    return IntersectionType.GrazeStart;

                if (context2 == IntersectionType.TouchOther)
                    return IntersectionType.GrazeOther;

                return IntersectionType.GrazeEnd;
            }
            else if (context1 == IntersectionType.TouchStart)
            {
                if (context2 == IntersectionType.TouchStart)
                    return IntersectionType.GrazeTotal;

                if (context2 == IntersectionType.TouchOther)
                    return IntersectionType.GrazeStart;

                return IntersectionType.GrazeTotal;
            }

            // context1 == IntersectionType.TounchEnd

            if (context2 == IntersectionType.TouchStart)
                return IntersectionType.GrazeTotal;

            if (context2 == IntersectionType.TouchOther)
                return IntersectionType.GrazeEnd;

            return IntersectionType.GrazeTotal;
        }
    }
}
