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
using System.Collections.Generic;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="24-JUL-1997" />
    /// <summary>
    /// Provides information about the relative position of a polygon ring divider
    /// with respect to a topological node.
    /// <para/>
    /// Utilized by the <see cref="ConnectionFinder"/> class to hold information used to determine
    /// the cyclic order at a topological node. Nobody else in the world should be
    /// using this class.
    /// </summary>
    class Orientation
    {
        #region Class data

        /// <summary>
        /// The divider entering the node. Null if the orientation relates to an
        /// instance of <see cref="HorizontalRay"/>.
        /// </summary>
        readonly IDivider m_Divider;

        /// <summary>
        /// Are we talking about the start of divider?
        /// </summary>
        readonly bool m_IsStart;

        /// <summary>
        /// Relative position of orientation point
        /// </summary>
	    double m_DeltaI;
	    double m_DeltaJ;

        /// <summary>
        /// The quadrant of the orientation point
        /// </summary>
	    Quadrant m_Quadrant;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates new <c>Orientation</c> for one end of a divider
        /// </summary>
        /// <param name="d">The divider we are interested in (not null)</param>
        /// <param name="isStart">At start of the divider?</param>
        /// <exception cref="ArgumentNullException">If <paramref name="d"/> is null</exception>
        internal Orientation(IDivider d, bool isStart)
        {
            if (d==null)
                throw new ArgumentNullException("Divider cannot be null");

            m_Divider = d;
            m_IsStart = isStart;

            // Get orientation point for the divider
            IPosition orient = d.LineGeometry.GetOrient(isStart, 0.0);

            // Set the orientation info.
	        SetOrient(orient);
        }

        /// <summary>
        /// Creates new <c>Orientation</c> based on the supplied horizontal ray.
        /// </summary>
        /// <param name="hr"></param>
        internal Orientation(HorizontalRay hr)
        {
            // Remember the stuff we were supplied (since we are not starting
            // with a divider, it has to be null).

            m_Divider = null;
            m_IsStart = false;

            // The horizontal segment is ALWAYS at the very start of the
            // north-west quadrant.
            m_Quadrant = Quadrant.NW;

            // Convert the deltas into the IJ coordinate system for the
            // north-west quadrant (the angle defined with I/J should
            // evaluate to zero, since the horizontal segment is at the
            // start of the quadrant).

            m_DeltaI = 0.0;
            m_DeltaJ = hr.EndX - hr.StartX;
        }

        #endregion

        internal IDivider Divider
        {
            get { return m_Divider; }
        }

        internal bool IsStart
        {
            get { return m_IsStart; }
        }

        internal Quadrant Quadrant
        {
            get { return m_Quadrant; }
        }

        internal double Angle
        {
            get { return m_DeltaI/m_DeltaJ; }
        }

        internal double LengthSquared
        {
            get { return (m_DeltaI*m_DeltaI + m_DeltaJ*m_DeltaJ); }
        }

        /// <summary>
        /// Figure out what quadrant a relative position lies in. Each quadrant includes
        /// the initial axis, but not the axis which follows (e.g. The NE quadrant extends
        /// clockwise from the Y-axis to just above the X-axis).
        /// </summary>
        /// <param name="dx">Delta-X</param>
        /// <param name="dy">Delta-Y</param>
        /// <returns></returns>
        Quadrant WhatQuadrant(double dx, double dy)
        {
            // Strictly speaking, testing values for equality with zero
            // may be dependent on the implementation of the compiler and
            // the floating point representation of floats.

            bool dxzero = (Math.Abs(dx) < Constants.TINY);
            bool dyzero = (Math.Abs(dy) < Constants.TINY);

            if ((dx>0.0 || dxzero) && dy>0.0)
                return Quadrant.NE;

            if (dx>0.0 && (dy<0.0 || dyzero))
                return Quadrant.SE;

            if ((dx<0.0 || dxzero) && dy< 0.0)
                return Quadrant.SW;

            if (dx<0.0 && (dy>0.0 || dyzero))
                return Quadrant.NW;

            return Quadrant.All; // just in case
        }

        /// <summary>
        /// Checks whether two orientation objects have the same direction.
        /// </summary>
        /// <param name="other">The other orientation to compare with.</param>
        /// <returns></returns>
        bool IsSameAs(Orientation other)
        {
            // Not the same if in different quadrants.
	        if (m_Quadrant != other.m_Quadrant)
                return false;

            // If the direction is the same, then di1/dj1 = di2/dj2
            // => di1*dj2 == di2*dj1 => di1*dj2 - di2*dj1 == 0.
	        return (Math.Abs(m_DeltaI*other.m_DeltaJ - m_DeltaJ*other.m_DeltaI) < Constants.TINY);
        }

        /// <summary>
        /// Defines orientation points on curves with respect to this orientation object.
        /// </summary>
        /// <param name="orient">Orientations relating to a node (ONE of these
        ///	will refer to THIS orientation object).</param>
        internal void SetCurves(List<Orientation> orient)
        {
            // Get the shortest distance (squared) to the set of
            // supplied orientation objects (ignoring curves).
            double CRVORIENTSQ = 25.0;	// 5*5 metres
            double mindsq = 1.0 + CRVORIENTSQ;

            foreach(Orientation o in orient)
            {
                if (!o.IsDividerArc)
                {
                    double dsq = o.LengthSquared;
                    if (dsq < mindsq)
                        mindsq = dsq;
                }
            }

            // If the minimum orientation length is more than the
            // usual amount, what we did in the constructor is fine.
            if (mindsq > CRVORIENTSQ)
                return;

            // Use the minimum orientation distance to get an orientation
            // position on each curve.

            double orilen = Math.Sqrt(mindsq);
            foreach(Orientation o in orient)
                o.SetCurve(orilen);
        }

        /// <summary>
        /// Does this orientation relate to a divider that coincides with a circular arc?
        /// </summary>
        internal bool IsDividerArc
        {
            get
            {
                if (m_Divider==null)
                    return false;
                else
                    return (m_Divider.Line is ArcFeature);
            }
        }

        /// <summary>
        /// Defines non-standard orientation point for a curve.
        /// </summary>
        /// <param name="orilen">The orientation length to use.</param>
        void SetCurve(double orilen)
        {
            // Return if this orientation isn't for a curve.
            if (!IsDividerArc)
                return;

            // Get the orientation position.
            IPosition oripos = m_Divider.LineGeometry.GetOrient(m_IsStart, orilen);

            // And set the orientation info.
            SetOrient(oripos);
        }

        /// <summary>
        /// Defines orientation info.
        /// </summary>
        /// <param name="orient">The orientation position.</param>
        void SetOrient(IPosition orient)
        {
        	if (m_Divider==null)
                return;

            // Get the position of the point that the divider meets.
            IPointGeometry loc = (m_IsStart ? m_Divider.From : m_Divider.To);

	        // Figure out the deltas of the orientation point with respect to the point.
	        double dx = orient.X - loc.X;
	        double dy = orient.Y - loc.Y;

	        // What quadrant are we in?
	        m_Quadrant = WhatQuadrant(dx,dy);

	        // Convert the deltas into an IJ coordinate system. When we
	        // calculate I/J, it will always give us tan(angle) in each
	        // successive quadrant, with the start of the quadrant resulting
	        // in an angle of zero, and the end of the quadrant resulting in
	        // an angle just less than infinity.

            switch (m_Quadrant)
            {
                case Quadrant.NE:
                {
                    m_DeltaI =  dx;
                    m_DeltaJ =  dy;
                    return;
                }

                case Quadrant.SE:
                {
                    m_DeltaI = -dy;
                    m_DeltaJ =  dx;
                    return;
                }

                case Quadrant.SW:
                {
                    m_DeltaI = -dx;
                    m_DeltaJ = -dy;
                    return;
                }

                case Quadrant.NW:
                {
                    m_DeltaI =  dy;
                    m_DeltaJ = -dx;
                    return;
                }
            }

		    string msg = String.Format("Orientation.SetOrient - Quadrant error at {0:0.0000}N {1:0.0000}E",
                            loc.Y, loc.X);
            throw new Exception(msg);
        }
    }
}
