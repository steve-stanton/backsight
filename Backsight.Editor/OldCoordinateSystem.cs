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

using Backsight.Editor.Properties;

namespace Backsight.Editor
{
    public class OldCoordinateSystem : ISpatialSystem
    {
        #region Class data

        /// <summary>
        /// Mean elevation of the map.
        /// </summary>
	    ILength m_MeanElevation;

        /// <summary>
        /// Geoid separation.
        /// </summary>
	    ILength m_GeoidSeparation;

        /// <summary>
        /// Scaling factor on central meridian.
        /// </summary>
	    readonly double m_ScaleFactor;

        /// <summary>
        /// UTM zone number (if applicable).
        /// </summary>
	    byte m_Zone;

        /// <summary>
        /// Name of the ellipsoid.
        /// </summary>
	    string m_Ellipsoid;

        /// <summary>
        /// Name of the map projection.
        /// </summary>
	    string m_Projection;

        /// <summary>
        /// Semi-major axis, in meters.
        /// </summary>
	    double m_A;

        /// <summary>
        /// Semi-minor axis, in meters.
        /// </summary>
	    double m_B;

        /// <summary>
        /// The central meridian, in radians.
        /// </summary>
	    double m_CentralMeridian;

        /// <summary>
        /// The false easting.
        /// </summary>
	    double m_FalseEasting;

        /// <summary>
        /// The false northing.
        /// </summary>
        //double m_FalseNorthing;

        #endregion

        #region Constructors

        /// <summary>
        /// Defines a UTM coordinate system in the northern hemisphere (on NAD83).
        /// The zone is obtained from application settings.
        /// </summary>
        public OldCoordinateSystem()
        {
            m_ScaleFactor = 0.9996;
            m_Ellipsoid = "NAD83";
            m_Projection = "UTM";
            m_A = 6378137.0;
            m_B = 6356752.3141;
            m_FalseEasting = 500000.0;
            m_Zone = Settings.Default.Zone;
            m_CentralMeridian = ZoneToRadians(m_Zone);
            m_MeanElevation = new Length(Settings.Default.MeanElevation);
            m_GeoidSeparation = new Length(Settings.Default.GeoidSeparation);
        }

        #endregion

        public override string ToString()
        {
            return String.Format("{0}.{1} ({2})", m_Projection, m_Zone, m_Ellipsoid);
        }

        public ILength MeanElevation
        {
            get { return m_MeanElevation; }
            set { m_MeanElevation = value; }
        }

        public ILength GeoidSeparation
        {
            get { return m_GeoidSeparation; }
            set { m_GeoidSeparation = value; }
        }

        public byte Zone
        {
            get { return m_Zone; }
            internal set
            {
                if (m_Zone<=0 || m_Zone>60)
                    throw new ArgumentOutOfRangeException("UTM zone number must be in range [1,60]");

                m_Zone = value;
                m_CentralMeridian = ZoneToRadians(m_Zone);
            }
        }

        public string Ellipsoid
        {
            get { return m_Ellipsoid; }
            internal set { m_Ellipsoid = value; }
        }

        public string Projection
        {
            get { return m_Projection; }
            internal set { m_Projection = value; }
        }

        private double ZoneToRadians(byte zoneNumber)
        {
            return (-183.0 + ((double)zoneNumber * 6.0)) * Constants.DEGTORAD;
        }

        /// <summary>
        /// Converts a projected position into geographic
        /// </summary>
        /// <param name="p">The XY position to convert</param>
        /// <returns>The corresponding geographic position (longitude is X, latitude is Y)</returns>
        public IPosition GetGeographic(IPosition p)
        {
            double latitude, longitude;
            TMToLatLong(p.X, p.Y, out latitude, out longitude);
            return new Position(longitude, latitude);
        }

        /// <summary>
        /// Computes the latitude and longitude for a given northing and easting on the Transverse Mercator
        /// projection. Based on Thomas (1952), and FORTRAN code written by R.R.Steeves (May,1977).
        /// </summary>
        /// <param name="x1">TM easting</param>
        /// <param name="y1">TM northing</param>
        /// <param name="phi">Resultant latitude (in radians).</param>
        /// <param name="olam">Resultant longitude (in radians).</param>
        private void TMToLatLong(double x1, double y1, out double phi, out double olam)
        {
            /*
            	X=(X1-XO)/SF
            	Y=Y1/SF
            	E=DSQRT((A**2-B**2)/A**2)
            	CALL FPLAT(A,B,Y,PHI1)
            	T=DTAN(PHI1)
            	SP=DSIN(PHI1)
            	CP=DCOS(PHI1)
            	ETA=DSQRT((A**2-B**2)/B**2*CP**2)
            	DN=A/DSQRT(1.0D0-E**2*SP**2)
            	XN = X/DN
            	DM=A*(1.0D0-E**2)/DSQRT((1.0D0-E**2*SP**2)**3)
	            PHI= PHI1
	            - T*X**2/2.0D0/DM/DN
	            + T*X**4/24.0D0/DM/DN**3*
	            (5.0D0 + 3.0D0*T**2 - ETA**2 - 4.0D0*ETA**4 - 9.0D0*ETA**2*T**2)
	            - T*X**6/720.0D0/DM/DN**5 *
	            (61.0D0 + 90.0D0*T**2 + 46.0D0*ETA**2 +45.0D0*T**4 
	            -252.0D0*T**2*ETA**2 - 3.0D0*ETA**4 +100.0D0*ETA**6 - 66.0D0*T**2*ETA**4
	            -90.0D0*T**4*ETA**2 +88.0D0*ETA**8 +225.0D0*T**4*ETA**4
	            +84.0D0*T**2*ETA**6 - 192.0D0*T**2*ETA**8)

                PHI=PHI+T*XN**7*X/40320.0D0/DM*(1385.0D0+3633.0D0*T**2+4095.0D0
	            1    *T**4+1575.0D0*T**6)
	            DLAM=(X/DN-(X/DN)**3/6.0D0*(1.0D0+2.0D0*T**2+ETA**2)+(X/DN)**5/
	            1   120.0D0*(5.0D0+6.0D0*ETA**2+28.0D0*T**2-3.0D0*ETA**4+8.0D0*T**2
	            2   *ETA**2+24.0D0*T**4-4.0D0*ETA**6+4.0D0*T**2*ETA**4+24.0D0*T**2*
	            3   ETA**6)-(X/DN)**7/5040.0D0*(61.0D0+662.0D0*T**2+1320.0D0*T**4+
	            4   720.0D0*T**6))/CP
	            OLAM=CMRAD+DLAM
	            X=X*SF+XO
	            Y=Y*SF
            */

	        double x = (x1-m_FalseEasting)/m_ScaleFactor;
            double y = y1/m_ScaleFactor;
            double asq = m_A*m_A;
            double bsq = m_B*m_B;
            double esq = (asq-bsq)/asq;
            double e = Math.Sqrt(esq);

            // Get foot-point latitude
            double phi1 = GetFootPointLat(y);

            double t = Math.Tan(phi1);
            double sp = Math.Sin(phi1);
            double cp = Math.Cos(phi1);
            double spsq = sp*sp;
            double cpsq = cp*cp;
            double eta = Math.Sqrt((asq-bsq)/bsq*cpsq);
            double dn = m_A/Math.Sqrt(1.0 - esq * spsq);
            double xn = x/dn;

            // Do some big-daddy calculations. Here's the originals ...
            // In Fortran, ** has the highest precedence.
            /*
	            DM=A*(1.0D0-E**2)/DSQRT((1.0D0-E**2*SP**2)**3)

	            PHI=PHI1-T*X**2/2.0D0/DM/DN+T*X**4/24.0D0/DM/DN**3*(5.0D0+3.0D0*
	            1   T**2-ETA**2-4.0D0*ETA**4-9.0D0*ETA**2*T**2)-T*X**6/720.0D0/DM/
	            2   DN**5*(61.0D0+90.0D0*T**2+46.0D0*ETA**2+45.0D0*T**4-252.0D0*T**
	            3   2*ETA**2-3.0D0*ETA**4+100.0D0*ETA**6-66.0D0*T**2*ETA**4-90.0D0
	            4   *T**4*ETA**2+88.0D0*ETA**8+225.0D0*T**4*ETA**4+84.0D0*T**2*
	            5   ETA**6-192.0D0*T**2*ETA**8)

	            PHI=PHI+T*XN**7*X/40320.0D0/DM*(1385.0D0+3633.0D0*T**2+4095.0D0
	            1    *T**4+1575.0D0*T**6)

	            DLAM=(X/DN-(X/DN)**3/6.0D0*(1.0D0+2.0D0*T**2+ETA**2)+(X/DN)**5/
	            1   120.0D0*(5.0D0+6.0D0*ETA**2+28.0D0*T**2-3.0D0*ETA**4+8.0D0*T**2
	            2   *ETA**2+24.0D0*T**4-4.0D0*ETA**6+4.0D0*T**2*ETA**4+24.0D0*T**2*
	            3   ETA**6)-(X/DN)**7/5040.0D0*(61.0D0+662.0D0*T**2+1320.0D0*T**4+
	            4   720.0D0*T**6))/CP

	            OLAM=CMRAD+DLAM
            */
	        double eta2 = eta*eta;
            double eta4 = eta2*eta2;
            double eta6 = eta4*eta2;
            double eta8 = eta6*eta4; // SS20090428 - UM, IS THIS SUPPOSED TO BE *eta2 ? (old CEdit code had *eta4)

            double x2 = x*x;
            double x4 = x2*x2;
            double x6 = x4*x2;

            double t2 = t*t;
            double t4 = t2*t2;
            double t6 = t4*t2;

            double dm = m_A * (1.0-esq)/Math.Sqrt(Math.Pow(1.0-esq*spsq, 3.0));

            phi = phi1
                -	(t * x2/2.0/dm/dn)
                +	(t * x4/24.0/dm/Math.Pow(dn,3.0) * (5.0 + 3.0*t2 - eta2 - 4.0*eta4 - 9.0*eta2*t2))
                -	(t * x6/720.0/dm/Math.Pow(dn, 5.0) * (61.0 + 90.0*t2 + 46.0*eta2 +45.0*t4 - 
					                 252.0*t2*eta2 - 3.0*eta4 +100.0*eta6 - 66.0*t2*eta4 -
					                 90.0*t4*eta2 + 88.0*eta8 +225.0*t4*eta4 +
					                 84.0*t2*eta6 - 192.0*t2*eta8));

            phi += (t * Math.Pow(xn, 7.0) * x/40320.0/dm* (1385.0
							           +	3633.0*t2
							           +	4095.0*t4
							           +	1575.0*t6 ));

	        double xdn = x/dn;
            double xdn3 = Math.Pow(xdn, 3.0);
            double xdn5 = Math.Pow(xdn, 5.0);
            double xdn7 = Math.Pow(xdn, 7.0);

	        double dlam =
		        ( xdn
		        - xdn3/6.0 * ( 1.0 + 2.0*t2 + eta2 )
		        + xdn5/120.0 * ( 5.0 + 6.0*eta2 + 28.0*t2
				                 - 3.0*eta4 + 8.0*t2*eta2 + 24.0*t4
					           - 4.0*eta6 + 4.0*t2*eta4 + 24.0*t2*eta6 )
		        - xdn7/5040.0 * (61.0 + 662.0*t2 + 1320.0*t4 + 720.0*t6)
		        ) / cp;

	        olam = m_CentralMeridian + dlam;
        }

        /// <summary>
        /// Computes the foot-point latitude required to transform Transverse Mercator plane 
        /// coordinates into ellipsoidal coordinates.
        /// Based on FORTRAN code by R.R.Steeves (June, 1977).
        /// </summary>
        /// <param name="y">Northing of the point for which the foot point latitude is to be computed.</param>
        /// <returns>The foot point latitude, in radians.</returns>
        private double GetFootPointLat(double y)
        {
	        double e2 = (m_A*m_A - m_B*m_B)/(m_A*m_A);
            double e4 = e2*e2;
            double e6 = e4*e2;
            double e8 = e6*e2;

            //	The latitude is worked out using some sort of series expansion ...

            double a0 = 1.0 - e2/4.0 - 3.0*e4/64.0 - 5.0*e6/256.0 - 175.0*e8/16384.0;
            double a2 = 3.0/8.0 * (e2 + e4/4.0 + 15.0*e6/128.0 - 455.0*e8/4096.0);
            double a4 = 15.0/256.0 * (e4 + 3.0*e6/4.0 - 77.0*e8/128.0);
            double a6 = 35.0/3072.0 * (e6 - 41.0*e8/32.0);
            double a8 = -315.0 * (e8/131072.0);

            double phi = y/m_A;			// Initial approximation of latitude.
            double dphi=9999.0;			// Define some arbitrary big number for initial latitude correction.
            double tol = 1.0e-13;	    // Define exit tolerance.

            //	Iterate until the latitude difference is neglibable.

	        while ( Math.Abs(dphi)>tol )
            {
                double top = m_A * ( a0*phi
						           - a2*Math.Sin(2.0*phi)
						           + a4*Math.Sin(4.0*phi)
						           - a6*Math.Sin(6.0*phi)
						           + a8*Math.Sin(8.0*phi)) - y;

		        double bot = m_A * ( a0
						           - 2.0*a2*Math.Cos(2.0*phi)
						           + 4.0*a4*Math.Cos(4.0*phi)
						           - 6.0*a6*Math.Cos(6.0*phi)
						           + 8.0*a8*Math.Cos(8.0*phi));

		        dphi = top/bot;
		        phi -= dphi;
        	}

	        return phi;
        }

        /// <summary>
        /// Computes line scale factor (for Transverse Mercator). 
        /// </summary>
        /// <param name="start">The start of the line.</param>
        /// <param name="end">The end of the line.</param>
        /// <returns>The scale factor.</returns>
        public double GetLineScaleFactor(IPosition start, IPosition end)
        {
            double sLat, sLng;
            TMToLatLong(start.X, start.Y, out sLat, out sLng);

            double eLat, eLng;
            TMToLatLong(end.X, end.Y, out eLat, out eLng);

	        return TMLineScaleFactor(sLat, eLat, start.X, end.X);
        }

        /// <summary>
        /// Computes line scale factor (for Transverse Mercator projection). 
        /// Written by R.R.Steeves circa 1978, modified by Bob Bruce 06-FEB-1985,
        /// by taking out the calculation of the arc to chord correction.
        /// </summary>
        /// <param name="phi1">Latitude of point 1 (in radians).</param>
        /// <param name="phi2">Latitude of point 2 (in radians).</param>
        /// <param name="x1">UTM easting of point 1.</param>
        /// <param name="x2">UTM easting of point 2.</param>
        /// <returns>Line scale factor</returns>
        private double TMLineScaleFactor(double phi1, double phi2, double x1, double x2)
        {
            // Get the mid-latitude.
	        double phi = (phi1 + phi2)*0.5;

            // Get eccentricity
	        double asq = m_A*m_A;
	        double esq = (asq - m_B*m_B)/(asq);

            // May need to change this, depending on what the
            // operator precedence was in FORTRAN (my assumption
            // here is that / and ** have the same precedence).

            // Original code:
            // R2=AA**2*(1.D0-ESQ)/(1.D0-ESQ*SIN(PHI)**2)**2

            double sinphi = Math.Sin(phi);
            double fac = (1.0 - esq*sinphi*sinphi);
            double r2 = asq * (1.0-esq)/(fac*fac);

            // Deduct false easting
            double x11 = x1 - m_FalseEasting;
            double x21 = x2 - m_FalseEasting;

            // Do some magic to churn out the scale factor.
	        double xu2 = x11*x11 + x11*x21 +x21*x21;
	        return m_ScaleFactor*(1.0 + xu2/6.0/r2*(1.0+xu2/36.0/r2));
        }

        /// <summary>
        /// Returns the ground area for the supplied list of vertices (which is assumed to form a closed
        /// shape). All vertices are assumed to be at the mean elevation recorded as part of this coordinate
        ///	system.
        /// <para/>
        ///	The way that positions are converted to ground is described in a monograph entitled "The Use
        ///	of UTM Coordinates - Distance Reductions" issued by the Ministry of Environment, Lands & Parks,
        ///	British Columbia. This may be slightly inconsistent with similar calculations done elsewhere in
        ///	this class.
        /// <para/>
        /// The monograph in question (by Vern Vogt) is mentioned on http://ilmbwww.gov.bc.ca/bmgs/gsr/gsr_papers.htm,
        /// though I don't see a way to access it online.
        /// </summary>
        /// <param name="v">The positions defining a polygon, with the polygon always on the right-hand-side,
        /// and with the first position repeated at the end.</param>
        /// <returns>The corresponding area on the ground (in square meters)</returns>
        public double GetGroundArea(IPosition[] v)
        {
            if (v.Length <= 2)
                return 0.0;

            // Get the ellipsoid scale factor for the map (note that
            // geoid separation is expected to be a positive value in
            // places where the geoid is above the reference ellipsoid).
            double efac = m_A / (m_A + m_MeanElevation.Meters + m_GeoidSeparation.Meters);

            // Get the eccentricity
            //double asq = m_A*m_A;
            //double esq = (asq - m_B*m_B)/(asq);

            // Actually, the documentation says you should divide by b*b,
            // not that it should make much difference
            double bsq = m_B*m_B;
            double esq = (m_A*m_A - bsq)/(bsq);

            // Work with a local origin the corresponds to the first
            // position (this DOES make quite a perceptible difference,
            // especially when the area is fairly small).

            double xo = v[0].X;
            double yo = v[0].Y;

            // The initial position is at the local origin (the
            // multiplying factor won't be used)

            double xs = 0.0;
            double ys = 0.0;
            double f1 = 0.0;

            double f2;
            double xe;
            double ye;

            double area = 0.0;

            for ( int i=1; i<v.Length; i++, f1=f2, xs=xe, ys=ye )
            {
                // Get the scale factor at the end of the current segment
                f2 = 1.0 / (GetScaleFactor(v[i],esq) * efac);
                xe = (v[i].X - xo) * f2;
                ye = (v[i].Y - yo) * f2;

                // Use the mid-X of the segment to get the area left (signed).
                // If the line is directed up the way, it contributes a
                // positive area. If directed down, it contributes a
                // negative area. So, if flat, it contributes nothing (what
                // we are actually calculating here is double the area; we
                // will adjust this when we are done with the loop).

                double extra = (ys-ye) * (xe+xs);
                if ( Math.Abs(extra)<Constants.TINY ) extra = 0.0;
                area += extra;
            }
	
	        return (area * 0.5);
        }

        /// <summary>
        /// Returns the UTM scale factor at a the supplied position.
        /// </summary>
        /// <param name="v">The position to get the scale factor for.</param>
        /// <param name="esq">The eccentricity of the spheroid (supplied simply to avoid repeated calculation)</param>
        /// <returns>The corresponding scale factor</returns>
        private double GetScaleFactor(IPosition v, double esq)
        {
            double lat, lng;
            TMToLatLong(v.X, v.Y, out lat, out lng);

            double dLng = lng - m_CentralMeridian;
            double cosLat = Math.Cos(lat);
            double cosLatSq = cosLat * cosLat;

            double a = dLng * dLng * cosLatSq * 0.5;
            double b = 1.0 + (esq * cosLatSq);

            return (1.0 + (a * b)) * m_ScaleFactor;
        }

        public string Name
        {
            get { return String.Format("UTM {0}N - NAD83", this.Zone); }
        }

        public int EpsgNumber
        {
            get { return 0; }
        }
    }
}
