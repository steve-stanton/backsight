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
	/// <written by="Steve Stanton" on="07-OCT-1997" />
    /// <summary>
    /// A vertex that is expressed in terms of a position with respect to some other vertex.
    /// Position is represented by a quadrant number, along with a relative coordinate.
    /// Quadrants are referred to using the <c>Quadrant</c> enumeration, which starts in the north-east
    /// corner, and works round clockwise.
    /// 
    /// The relative coordinate is expressed in an IJ system, with the I-axis corresponding to
    /// the start of each quadrant, and the J-axis corresponding to the start of the next quadrant.
    /// The IJ values are always positive, so J/I will give Tan(angle) with respect to the start
    /// of the quadrant, reckoned clockwise.
    /// 
    /// For example, if the origin is at (0,0), a position of X=0,Y=10 belongs in the north-eastern
    /// quadrant, and will have I=10,J=0. A position at X=10,Y=0 belongs to the south-eastern quadrant,
    /// and will also have I=10,J=0. This convention means that J/I will give Tan(angle), ranging from
    /// 0 at the start of a quadrant, to just less than infinity at the end of a quadrant.
    /// </summary>
    public class QuadVertex
    {
        #region Class data

        /// <summary>
        /// Origin of the position.
        /// </summary>
        IPosition m_Origin;

        /// <summary>
        /// Relative position of the vertex, with respect to the origin.
        /// </summary>
        double m_DeltaI;
        double m_DeltaJ;

        /// <summary>
        /// The quadrant of the point.
        /// </summary>
        Quadrant m_Quadrant;
 
        /// <summary>
        /// Length of vector from origin to the position. If non-zero, the
        // IJ values will be normalized.
        /// </summary>
        double m_Length;

        #endregion

        #region Constructors

        public QuadVertex()
        {
	        m_DeltaI = 0.0;
	        m_DeltaJ = 0.0;
	        m_Quadrant = Quadrant.All;
	        m_Length = 0.0;
        }

        public QuadVertex(IPosition origin, IPosition vtx)
            : this(origin, vtx, 0.0)
        {
        }

        /// <summary>
        /// Constructor accepting two vertices, and an optional vector length.
        /// </summary>
        /// <param name="origin">The origin of the position.</param>
        /// <param name="vtx">The vertex to represent.</param>
        /// <param name="length">The length of the vector from origin to the vertex (specify 0 if the
        /// deltas don't need to be normalized).</param>
        public QuadVertex(IPosition origin, IPosition vtx, double length)
        {
            // Remember the origin.
	        m_Origin = origin;

            // Figure out the deltas with respect to the origin.
	        double dx = vtx.X - origin.X;
	        double dy = vtx.Y - origin.Y;

            // Remember the length. If specified, normalize the deltas.

	        m_Length = length;
	        if (m_Length > Double.Epsilon)
            {
		        double inv = 1.0/length;
		        dx *= inv;
		        dy *= inv;

		        // 16-NOV-99: Make absolutely sure that normalized deltas
		        // never exceed +/- 1.0. If this happened (and it did. with
		        // a roundoff of about 1e-9, things like Math.Asin return
		        // indefinite results.
		        CheckNormalVal(ref dx);
		        CheckNormalVal(ref dy);
        	}

	        if (Math.Abs(dx)<Double.Epsilon) dx = 0.0;
            if (Math.Abs(dy)<Double.Epsilon) dy = 0.0;

            // What quadrant are we in?
	        m_Quadrant = WhatQuadrant(dx,dy);

            // Convert the deltas into the IJ system.

	        switch ( m_Quadrant )
            {
                case Quadrant.NE:
                {
                    m_DeltaI = dy;
                    m_DeltaJ = dx;
                    break;
                }

                case Quadrant.SE:
                {
                    m_DeltaI = dx;
                    m_DeltaJ = -dy;
                    break;
                }

                case Quadrant.SW:
                {
                    m_DeltaI = -dy;
                    m_DeltaJ = -dx;
                    break;
                }

                case Quadrant.NW:
                {
                    m_DeltaI = -dx;
                    m_DeltaJ = dy;
                    break;
                }

                default:
                {
                    m_DeltaI = 0.0;
                    m_DeltaJ = 0.0;
                    break;
                }
            }
        }

        #endregion

        public Quadrant Quadrant
        {
            get { return m_Quadrant; }
        }

        public double GetTanAngle()
        {
            return m_DeltaJ / m_DeltaI;
        }

        public bool IsValid()
        {
            return (m_Quadrant!=Quadrant.All);
        }

        private void CheckNormalVal(ref double val )
        {
            if ( val<-1.0 )
                val=-1.0;
            else if ( val>1.0 )
                val=1.0;
        }


        /// <summary>
        /// Figure out what quadrant a relative position lies in.
        /// 
        /// Each quadrant includes the initial axis, but not the axis which follows
        /// (e.g. quadrant 1 extends clockwise from the Y-axis to just above the X-axis).
        /// </summary>
        /// <param name="dx"></param>
        /// <param name="dy"></param>
        /// <returns></returns>
        private Quadrant WhatQuadrant(double dx, double dy)
        {
            // Quadrant 1 = NE
            // Quadrant 2 = SE
            // Quadrant 3 = SW
            // Quadrant 4 = NE

            //	Strictly speaking, testing values for equality with zero
            //	may be dependent on the implementation of the compiler and
            //	the floating point representation of floats.

            if (dx>=0.0 && dy> 0.0)
                return Quadrant.NE;

            if (dx> 0.0 && dy<=0.0)
                return Quadrant.SE;

            if (dx<=0.0 && dy< 0.0)
                return Quadrant.SW;

            if (dx< 0.0 && dy>=0.0)
                return Quadrant.NW;

            return Quadrant.All;	// just in case
        }

        /// <summary>
        /// The bearing of the vertex, in radians
        /// </summary>
        //public IAngle Bearing
        //{
        //    get
        //    {
        //        double ang = this.BearingInRadians;
        //        return new RadianValue(ang);
        //    }
        //}

        public double BearingInRadians
        {
            get
            {
                double ang = Math.Atan(m_DeltaJ/m_DeltaI);
                if (ang<MathConstants.TINY)
                    ang = 0.0;

                switch (m_Quadrant)
                {
                    case Quadrant.NE:
                        return ang;

                    case Quadrant.SE:
                        return ang + MathConstants.PIDIV2;

                    case Quadrant.SW:
                        return ang + MathConstants.PI;

                    case Quadrant.NW:
                        return ang + MathConstants.PIMUL1P5;

                    default:
                        return 0.0;
                }
            }
        }

        /// <summary>
        /// The area of the region to left of a circular arc that extends from the start of
        /// the quadrant to the stored position. The region extends all the way to the Y-axis.
        /// <para/>
        /// By convention, the areas for points in the north-west and south-west quadrants are
        /// returned as negative values, while the other two are positive (assuming that the
        /// circle is to the right of the Y-axis). This same convention is followed by
        /// <c>Circle.GetQuadrantArea</c>.
        /// </summary>
        public double CurveArea
        {
            get
            {
                // If the size of the vector from the origin to the point
                // has not been determined, work it out now, and normalize
                // the deltas.

	            if ( m_Length < MathConstants.TINY )
                {
		            m_Length = Math.Sqrt(m_DeltaI*m_DeltaI + m_DeltaJ*m_DeltaJ);
		            double inv = 1.0/m_Length;
		            m_DeltaI *= inv;
		            m_DeltaJ *= inv;

		            // 16-NOV-99: Make absolutely sure that normalized deltas
		            // never exceed +/- 1.0. If this happened (and it did. with
		            // a roundoff of about 1e-9, things like asin return
		            // indefinite results.

		            CheckNormalVal(ref m_DeltaI);
		            CheckNormalVal(ref m_DeltaJ);
	            }

                // Calculate the angle from the start of the quadrant.
	            double ang = Math.Asin(m_DeltaJ);

                // Note the centre of the circle
                double xc = m_Origin.X;

                // Calculate the area to the left of a curve that stretches
                // from the start of the quadrant to the calculated angle.
                // The area we are talking about stretches all the way
                // to the Y-axis.

                // The formala for each quadrant is expanded a bit in the
                // following switch statement. If we ignore the signing
                // convention, and the fact that the IJ values have been
                // normalized, they look like this:
                //
                //	NE  +ang*R*R/2 - ij/2 + Xc*(R-i)
                //	SE	+ang*R*R/2 + ij/2 + Xc*j
                //	SW  -ang*R*R/2 + ij/2 + Xc*(R-i)
                //	NW  -ang*R*R/2 - ij/2 + Xc*j

	            switch ( m_Quadrant )
                {
	                case Quadrant.NE:                		
	                  return ( m_Length*m_Length*0.5*(ang-m_DeltaI*m_DeltaJ)
					                + m_Length*xc*(1.0-m_DeltaI) );

	                case Quadrant.SE:
	                  return ( m_Length*m_Length*0.5*(ang+m_DeltaI*m_DeltaJ)
					                + m_Length*xc*m_DeltaJ );

	                case Quadrant.SW:
	                  return -( m_Length*m_Length*-0.5*(ang-m_DeltaI*m_DeltaJ)
					                + m_Length*xc*(1.0-m_DeltaI) );

	                case Quadrant.NW:
	                  return -( m_Length*m_Length*-0.5*(ang+m_DeltaI*m_DeltaJ)
					                + m_Length*xc*m_DeltaJ );

	                default:            		
	                  return 0.0;
	            }
            }
        }

        /// <summary>
        /// Assigns a <c>QuadVertex</c> to this one.
        /// </summary>
        /// <param name="rhs">The object to copy values from</param>
        public void Assign(QuadVertex rhs)
        {
            m_Origin = rhs.m_Origin;
            m_DeltaI = rhs.m_DeltaI;
            m_DeltaJ = rhs.m_DeltaJ;
            m_Quadrant = rhs.m_Quadrant;
            m_Length = rhs.m_Length;
        }

        /// <summary>
        /// Given a quadrant, return the ID of the next clockwise quadrant.
        /// </summary>
        /// <param name="quadrant">The quadrant to start from.</param>
        /// <returns></returns>
        public static Quadrant NextQuadrant(Quadrant quadrant)
        {
        	switch(quadrant)
            {
	            case Quadrant.NE: return Quadrant.SE;
	            case Quadrant.SE: return Quadrant.SW;
	            case Quadrant.SW: return Quadrant.NW;
	            case Quadrant.NW: return Quadrant.NE;
	            default:  return Quadrant.All;
	        }
        }
    }
}
