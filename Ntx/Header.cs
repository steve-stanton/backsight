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

namespace Ntx
{
    public class Header
    {
        #region Class data

        /// <summary>
        /// File title (char[80])
        /// </summary>
        string m_Title;
        const int TitleLength=80;

        /// <summary>
        /// File ID (char[12])
        /// </summary>
        string m_FileId;
        const int FileIdLength=12;

        /// <summary>
        /// Planimetric resolution
        /// </summary>
	    double m_XYResolution;

        /// <summary>
        /// Resolution for elevations
        /// </summary>
	    double m_ZResolution;

        /// <summary>
        /// Ellipsoid (char[4])
        /// </summary>
	    string m_Ellipsoid;
        const int EllipsoidLength=4;

        /// <summary>
        /// Projection (char[2] + two bytes padding)
        /// </summary>
	    string m_Projection;
        const int ProjectionLength=4;

        /// <summary>
        /// Scale
        /// </summary>
	    double m_Scale;

        /// <summary>
        /// Scaling factor on central meridian
        /// </summary>
	    double m_ScaleFactor;

        /// <summary>
        /// Longitude of central meridian
        /// </summary>
	    double m_CentralMeridian;

        /// <summary>
        /// UTM zone number (if projection is UTM)
        /// </summary>
	    uint m_Zone;

        /// <summary>
        /// First scaling latitude (if applicable)
        /// </summary>
	    double m_1stScalingLat;

        /// <summary>
        /// Second scaling latitude (if applicable)
        /// </summary>
	    double m_2ndScalingLat;

        /// <summary>
        /// Coordinate system (e.g. NEMR) (char[4])
        /// </summary>
	    string m_CoordSystem;
        const int CoordSystemLength=4;

        /// <summary>
        /// False easting
        /// </summary>
	    double m_FalseEasting;

        /// <summary>
        /// False northing
        /// </summary>
	    double m_FalseNorthing;

        /// <summary>
        /// Offset for X values
        /// </summary>
	    double m_ShiftX;

        /// <summary>
        /// Offset for Y values
        /// </summary>
	    double m_ShiftY;

        /// <summary>
        /// Offset for Z values
        /// </summary>
	    double m_ShiftZ;

        /// <summary>
        /// What vertical datum? (char[4])
        /// </summary>
        string m_VerticalDatum;
        const int VerticalDatumLength=4;

        /// <summary>
        /// #of 32-bit words defining header
        /// </summary>
	    int m_HeaderSize;

        /// <summary>
        /// #of 32-bit words for descriptors
        /// </summary>
	    int m_DescriptorSize;

        /// <summary>
        /// File format ID
        /// </summary>
	    int m_FormatId;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        internal Header()
        {
            //	Character fields

	        m_Title = String.Empty;
            m_FileId = String.Empty;
            m_Ellipsoid = String.Empty;
            m_Projection = String.Empty;
            m_CoordSystem = String.Empty;
            m_VerticalDatum = String.Empty;

            //	Float fields

	        m_XYResolution = 0.0;
	        m_ZResolution = 0.0;
	        m_Scale = 0.0;
	        m_ScaleFactor = 0.0;
	        m_CentralMeridian = 0.0;
	        m_1stScalingLat = 0.0;
	        m_2ndScalingLat = 0.0;
	        m_FalseEasting = 0.0;
	        m_FalseNorthing = 0.0;
	        m_ShiftX = 0.0;
	        m_ShiftY = 0.0;
	        m_ShiftZ = 0.0;

            //	Integer fields

	        m_Zone = 0;
	        m_HeaderSize = 0;
	        m_FormatId = 0;
	        m_DescriptorSize = 0;
        }
        #endregion

        string Title
        {
            get { return m_Title; }
        }

        string FileId
        {
            get { return m_FileId; }
        }

        string Ellipsoid
        {
            get { return m_Ellipsoid; }
        }

        string Projection
        {
            get { return m_Projection; }
        }

        public double XYResolution
        {
            get { return m_XYResolution; }
        }

        double ZResolution
        {
            get { return m_ZResolution; }
        }

        double Scale
        {
            get { return m_Scale; }
        }

        double ScaleFactor
        {
            get { return m_ScaleFactor; }
        }

        double CentralMeridian
        {
            get { return m_CentralMeridian; }
        }

        double FalseEasting
        {
            get { return m_FalseEasting; }
        }

        double FalseNorthing
        {
            get { return m_FalseNorthing; }
        }

        uint UTMZone
        {
            get { return m_Zone; }
        }

        /// <summary>
        /// Converts internal X to ground
        /// </summary>
        /// <param name="x">The X-position in disk units to convert.</param>
        /// <returns></returns>
        internal double XToGround(int x)
        {
            return ((double)x * m_XYResolution) + m_ShiftX;
        }

        /// <summary>
        /// Converts internal Y to ground
        /// </summary>
        /// <param name="y">The Y-position in disk units to convert.</param>
        /// <returns></returns>
        internal double YToGround(int y)
        {
            return ((double)y * m_XYResolution) + m_ShiftY;
        }

        /// <summary>
        /// Converts internal Z to ground
        /// </summary>
        /// <param name="z">The Z-position in disk units to convert. The Z is
        /// expected to be valid.</param>
        /// <returns></returns>
        internal float ZToGround(int z)
        {
            return (float)(((double)z * m_ZResolution) + m_ShiftZ);
        }

        /// <summary>
        /// Converts internal distance to ground units. 
        /// </summary>
        /// <param name="idist">The distance, in disk units.</param>
        /// <returns></returns>
        internal double DistanceToGround(int idist)
        {
            return (double)idist * m_XYResolution;
        }

        /// <summary>
        /// Parses buffer containing mainheader data.
        /// </summary>
        /// <param name="buf"></param>
        /// <returns></returns>
        unsafe private bool Parse(int[] buf)
        {
            fixed (int* pbuf = &buf[0])
            {
                return Parse(pbuf);
            }
        }

        unsafe private bool Parse(int* buf)
        {
            //	Get integer fields
            m_HeaderSize = buf[Pointers.PHMLEN];
            m_DescriptorSize = buf[Pointers.PHDLEN];
            m_FormatId = buf[Pointers.PHFMT];

            if (m_DescriptorSize != File.DESL)
                return false;

            //	Get character strings

            m_Title = Util.I4ch(&buf[Pointers.PHFTTL], TitleLength);
            m_FileId = Util.I4ch(&buf[Pointers.PHFID], FileIdLength);
            m_Ellipsoid = Util.I4ch(&buf[Pointers.PHELPS], EllipsoidLength);
            m_Projection = Util.I4ch(&buf[Pointers.PHPROJ], ProjectionLength);
            m_CoordSystem = Util.I4ch(&buf[Pointers.PHXCRD], CoordSystemLength);
            m_VerticalDatum = Util.I4ch(&buf[Pointers.PHDATM], VerticalDatumLength);

            //	Get float fields

            m_XYResolution = GetFloat(&buf[Pointers.PHXRES]);
            m_ZResolution = GetFloat(&buf[Pointers.PHZRES]);
            m_Scale = GetFloat(&buf[Pointers.PHSCAL]);
            m_ScaleFactor = GetFloat(&buf[Pointers.PHSFAC]);
            m_CentralMeridian = GetFloat(&buf[Pointers.PHCLON]);
            m_1stScalingLat = GetFloat(&buf[Pointers.PHSLT1]);
            m_2ndScalingLat = GetFloat(&buf[Pointers.PHSLT2]);
            m_FalseEasting = GetFloat(&buf[Pointers.PHFLSX]);
            m_FalseNorthing = GetFloat(&buf[Pointers.PHFLSY]);
            m_ShiftX = GetFloat(&buf[Pointers.PHXOFF]);
            m_ShiftY = GetFloat(&buf[Pointers.PHYOFF]);
            m_ShiftZ = GetFloat(&buf[Pointers.PHZOFF]);

            //	Get derived fields (zone number for UTM only)
            SetUTMZone();

            return true;
        }

        /// <summary>
        /// Gets a float field from header.
        /// </summary>
        /// <param name="buf">The array containing a character representation of the float</param>
        /// <returns></returns>
        unsafe private double GetFloat(int* buf)
        {
            string str = Util.I4ch(buf, 24);

            // We sometimes get 000000D+00, which the parser doesn't like
            str = str.ToUpper();
            int d = str.IndexOf("D+");
            if (d>0)
                str = str.Substring(0, d) + "E+" + str.Substring(d+2);

            return Double.Parse(str);
        }

        /// <summary>
        /// Sets UTM zone (if applicable).
        /// </summary>
	    private void SetUTMZone()
        {
            //	It has to be UTM
            m_Zone = 0;
            if (m_Projection != "UM")
                return;

            //	The central meridian must be the same as an integer
            double cm = Math.Floor(m_CentralMeridian);
            if (Math.Abs(m_CentralMeridian - cm) > 0.0000001)
                return;

            // Must be divisible by 6 with respect to the longitude of zone 1
            int icm = 177 + (int)cm;
            if ((icm % 6) != 0)
                return;

            //	Looks OK
            int izone = (icm/6) + 1;
            if (izone<0)
                return;

            m_Zone = (uint)izone;
        }

        /// <summary>
        /// Reads mainheader from NTX file & parse it.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        unsafe internal bool Read(File file)
        {
            // Read the number of words in the mainheader. Return
            // if the length is illegal.
    		int hlen;
		    file.Read(1,&hlen);
		    if (hlen<=1)
                return false;

            // Allocate memory for the header, read it in, and parse it.
		    int[] buf = new int[hlen];
		    buf[0] = hlen;
            fixed (int* pbuf = &buf[1]) { file.Read((uint)hlen-1, pbuf); }
            this.Parse(buf);
            return true;
        }
    }
}
