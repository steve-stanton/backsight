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
using System.Diagnostics;

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="03-OCT-1997" was="CeMultiSegment" />
    /// <summary>
    /// A line that consists of a series of connected line segments (referred to in other
    /// worlds as a "LineString"). The implementation aims to hold the data in packed form,
    /// by holding common high-order bytes just once (if there are no common bytes, the
    /// space required will actually be 2 bytes more than the unpacked data).
    /// </summary>
    class MultiSegmentGeometry : UnsectionedLineGeometry, IMultiSegmentGeometry
    {
        #region Data

        /// <summary>
        /// The data for the line. The first byte holds the number of high-order bytes
        /// that are identical for all X-positions along the line, followed by the
        /// corresponding values of the high-order bytes. The following bytes serve
        /// the same purpose for Y (i.e. count, followed by high-order byte values).
        /// The data after that consists of the low-order bytes for the X,Y positions
        /// defining the line.
        /// </summary>
        byte[] m_Data; // readonly

        #endregion

        #region Constructors

        /// <summary>
        /// Constructs a new <c>MultiSegmentGeometry</c>
        /// </summary>
        /// <param name="start">The point at the start of the connection.</param>
        /// <param name="end">The point at the end of the connection.</param>
        /// <param name="positions">The ground positions to pack. The first & last positions must exactly
        /// match the position of the supplied end points.</param>
        internal MultiSegmentGeometry(ITerminal start, ITerminal end, IPointGeometry[] positions)
            : base(start, end)
        {
            if (positions==null || positions.Length==0)
                throw new ArgumentNullException();

            if (positions.Length<=2)
                throw new ArgumentException("Not enough points for a multi-segment");

            if (!start.IsCoincident(positions[0]))
                throw new ArgumentException("Start point doesn't coincide with first position");

            if (!end.IsCoincident(positions[positions.Length-1]))
                throw new ArgumentException("End point doesn't coincide with last position");

            SetPackedData(positions);
        }

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public MultiSegmentGeometry()
        {
        }

        #endregion

        /// <summary>
        /// Defines the packed data that defines this geometry
        /// </summary>
        /// <param name="positions">The expanded positions to pack</param>
        void SetPackedData(IPointGeometry[] positions)
        {
            // Express ground positions as arrays of micron values
            long[] x = new long[positions.Length];
            long[] y = new long[positions.Length];

            for (int i=0; i<positions.Length; i++)
            {
                x[i] = positions[i].Easting.Microns;
                y[i] = positions[i].Northing.Microns;
            }

            // Get the number of common high-order bytes for X and Y
            byte[] commonX = GetCommonHighOrderBytes(x);
            byte[] commonY = GetCommonHighOrderBytes(y);

            int sizeHeader = 2 + commonX.Length + commonY.Length;
            int sizeDataX = positions.Length * (8-commonX.Length);
            int sizeDataY = positions.Length * (8-commonY.Length);
            m_Data = new byte[sizeHeader + sizeDataX + sizeDataY];

            // Define the header

            m_Data[0] = (byte)commonX.Length;
            m_Data[1] = (byte)commonY.Length;

            int to=2;
            for (int from=0; from<commonX.Length; from++, to++)
                m_Data[to] = commonX[from];

            //to = 2+commonX.Length;
            for (int from=0; from<commonY.Length; from++, to++)
                m_Data[to] = commonY[from];

            // Copy over the low-order bytes for each (X,Y)

            byte[] data = new byte[8];
            //to = sizeHeader;
            for (int i=0; i<x.Length; i++)
            {
                ToByteArray(x[i], ref data);
                for (int j=commonX.Length; j<8; j++)
                {
                    m_Data[to] = data[j];
                    to++;
                }

                ToByteArray(y[i], ref data);
                for (int j=commonY.Length; j<8; j++)
                {
                    m_Data[to] = data[j];
                    to++;
                }
            }

            Debug.Assert(m_Data.Length==to);
        }

        /// <summary>
        /// Convert this data into a series of ground positions.
        /// </summary>
        /// <returns>The unpacked version of this data.</returns>
        internal PointGeometry[] GetUnpackedData()
        {
            return GetDataInMicrons();
        }

        PointGeometry[] GetDataInMicrons()
        {
            int numCommonX = (int)m_Data[0];
            int numCommonY = (int)m_Data[1];

            // Grab the common high-order bytes
            long commonX = HighData(2, numCommonX);
            long commonY = HighData(2+numCommonX, numCommonY);

            int sizeHeader = (2 + numCommonX + numCommonY);
            int startIndex =  sizeHeader;
            int numDataByte = m_Data.Length - sizeHeader;
            int numBytePerPosition = (8-numCommonX + 8-numCommonY);
            Debug.Assert((numDataByte % numBytePerPosition)==0);
            int numPoint = numDataByte / numBytePerPosition;
            PointGeometry[] result = new PointGeometry[numPoint];

            // Now grab the actual positions in microns

            for (int i=0; i<result.Length; i++)
            {
                long xmic = LowData(startIndex, (8-numCommonX));
                startIndex += (8-numCommonX);

                long ymic = LowData(startIndex, (8-numCommonY));
                startIndex += (8-numCommonY);

                result[i] = new PointGeometry(commonX | xmic, commonY | ymic);
            }

            Debug.Assert(startIndex==m_Data.Length);
            return result;
        }

        /// <summary>
        /// Returns the long value that corresponds to a series of high-order bytes.
        /// </summary>
        /// <param name="startIndex">The array index of the initial byte in <c>m_Data</c> that
        /// should be copied (the byte at this position will be the high-order byte
        /// in the result)</param>
        /// <param name="numByte">The number of bytes to copy (in range [0,8])</param>
        /// <returns>The corresponding value, in microns on the ground</returns>
        private long HighData(int startIndex, int numByte)
        {
            Debug.Assert(startIndex >= 2);
            Debug.Assert(startIndex < m_Data.Length);
            Debug.Assert(numByte >= 0 && numByte <= 8);
            Debug.Assert(startIndex+numByte <= m_Data.Length);

            long result = 0;

            #if _ALLOW_POINTERS

            unsafe
            {
                byte* b = (byte*)&result;
                for (int i=startIndex, ib=7, ic=0; ic<numByte; i++, ic++, ib--)
                    b[ib] = m_Data[i];
            }

            #else

            for (int i=startIndex, shift=56; i<startIndex+numByte; i++, shift-=8)
            {
                long val = (long)m_Data[i];
                result |= (val << shift);
            }

            #endif

            return result;
        }

        /// <summary>
        /// Returns the long value that corresponds to a series of low-order bytes.
        /// </summary>
        /// <param name="startIndex">The array index of the initial byte in <c>m_Data</c> that
        /// should be copied (the byte at this position is the most significant byte
        /// in the result)</param>
        /// <param name="numByte">The number of bytes to copy (in range [0,8])</param>
        /// <returns>The corresponding value, in microns on the ground</returns>
        private long LowData(int startIndex, int numByte)
        {
            Debug.Assert(startIndex >= 2);
            Debug.Assert(startIndex < m_Data.Length);
            Debug.Assert(numByte >= 0 && numByte <= 8);
            Debug.Assert(startIndex+numByte <= m_Data.Length);

            long result = 0;

            #if _ALLOW_POINTERS

            unsafe
            {
                byte* b = (byte*)&result;
                for (int i=startIndex, ib=numByte-1; ib>=0; i++, ib--)
                    b[ib] = m_Data[i];
            }

            #else

            for (int i=startIndex, shift=(numByte-1)*8; i<startIndex+numByte; i++, shift-=8)
            {
                long val = (long)m_Data[i];
                result |= (val << shift);
            }

            #endif

            return result;
        }

        /// <summary>
        /// Converts the supplied long value into a byte array.
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <param name="result">The corresponding bytes, starting with the
        /// high-order bytes</param>
        private static void ToByteArray(long data, ref byte[] result)
        {
            Debug.Assert(result.Length>=8);

            #if _ALLOW_POINTERS

            unsafe
            {
                byte* b = (byte*)&data;
                for (int i=7; i>=0; i--, b++)
                    result[i] = (*b);
            }

            #else

            long mask = 0xFF;
            for (int i=7; i>=0; i--, data>>=8)
                result[i] = (byte)(data & mask);

            #endif
        }

        /// <summary>
        /// Determines the high-order bytes that are common to a series of values.
        /// </summary>
        /// <param name="data">The values to analyze.</param>
        /// <returns>The common high-order bytes, starting with the most significant
        /// one. Will contain 8 or fewer elements (may be an empty array).</returns>
        private static byte[] GetCommonHighOrderBytes(long[] data)
        {
            byte[] common = new byte[8];
            ToByteArray(data[0], ref common);
            int nCommon = 8;

            byte[] current = new byte[8];
            for (int i=1; i<data.Length && nCommon>0; i++)
            {
                ToByteArray(data[i], ref current);
                for (int j=0; j<nCommon; j++)
                {
                    if (common[j] != current[j])
                    {
                        nCommon = j;
                        break;
                    }
                }
            }

            byte[] result = new byte[nCommon];
            for (int i=0; i<nCommon; i++)
                result[i] = common[i];

            return result;
        }

        public override ILength Distance(IPosition point)
        {
            return LineStringGeometry.GetDistance(this, point);
        }

        public override IWindow Extent
        {
            get { return LineStringGeometry.GetExtent(this); }
        }

        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            LineStringGeometry.Render(this, display, style);
        }

        public override ILength Length
        {
            get { return LineStringGeometry.GetLength(this, null, null); }
        }

        internal override ILength GetLength(IPosition asFarAs)
        {
            ILength tol = new Length(Constants.XYTOL);
            return LineStringGeometry.GetLength(this, asFarAs, tol);
        }

        /// <summary>
        /// Checks whether a position is coincident with a line segment, using a tolerance that's
        /// consistent with the resolution of data.
        /// </summary>
        /// <param name="testX">The position to test</param>
        /// <param name="testY"></param>
        /// <param name="xs">Start of line segment.</param>
        /// <param name="ys"></param>
        /// <param name="xe">End of line segment.</param>
        /// <param name="ye"></param>
        /// <returns>True if the distance from the test position to the line segment is less
        /// than <c>Constants.XYTOL</c> (3 microns on the ground)</returns>
        // Redundant?
        private static bool IsCoincident(double testX, double testY, double xs, double ys, double xe, double ye)
        {
            return (Geom.DistanceSquared(testX, testY, xs, ys, xe, ye) < Constants.XYTOLSQ);
        }

        /// <summary>
        /// Gets the position that is a specific distance from the start of this line.
        /// </summary>
        /// <param name="dist">The distance from the start of the line.</param>
        /// <param name="result">The position found</param>
        /// <returns>True if the distance is somewhere ON the line. False if the distance
        /// was less than zero, or more than the line length (in that case, the position
        /// found corresponds to the corresponding terminal point).</returns>
        internal override bool GetPosition(ILength distance, out IPosition result)
        {
            IPosition[] data = GetUnpackedData();

            if (data.Length==2)
                return LineSegmentGeometry.GetPosition(data[0], data[1], distance.Meters, out result);
            else
                return GetPosition(data, distance.Meters, out result);
        }

        static bool GetPosition(IPosition[] line, double d, out IPosition result)
        {
            if (line.Length<2)
                throw new Exception("Line contains fewer than 2 positions");

            // Check for invalid distance
            if (d < 0.0)
            {
                result = line[0];
                return false;
            }

	        double walked = 0.0;	// Distance walked so far

            // Loop through the segments until we get to a distance that
            // exceeds the required distance.
	        for (int i=1; i<line.Length; i++)
            {
                // Update total length walked to end of segment
                double seglen = Geom.Distance(line[i-1], line[i]);
                walked += seglen;

                // If the accumulated distance exceeds the required distance,
                // the point we want is somewhere in the current segment.
		        if (walked >= d)
                {
			        double ratio = (walked-d)/seglen;

                    // Check whether the ratio yields a length that matches segment length to within
                    // the order of the data precision (1 micron). If so, return the start of the segment.
                    // If you don't do this, minute shifts occur when points are inserted into the
                    // multisegment. This means that if you repeat the GetPosition call with the SAME distance,
                    // you may actually get a point that is fractionally different from the initially returned
                    // point.

			        if ((seglen - ratio*seglen) < 0.000002)
                        result = line[i-1];
            		else
                    {
                        // Make damn sure!

                        double xs = line[i-1].X;
                        double ys = line[i-1].Y;
                        double xe = line[i].X;
                        double ye = line[i].Y;
                        double dx = xe - xs;
		                double dy = ye - ys;
				        double xres = xe-ratio*dx;
				        double yres = ye-ratio*dy;

                        if (Math.Abs(xres-xe)<0.000002 && Math.Abs(yres-ye)<0.000002)
                            result = line[i];
                        else
                            result = new Position(xres, yres);
        			}

		        	return true;
		        }
	        }

            // Got to the end, so the distance is too much
            result = line[line.Length-1];
            return false;
        }

        public IPointGeometry[] Data // implements IMultiSegmentGeometry
        {
            get { return GetUnpackedData(); }
        }

        internal override uint IntersectSegment(IntersectionResult results, ILineSegmentGeometry that)
        {
            return IntersectionHelper.Intersect(results, that, (IMultiSegmentGeometry)this);
        }

        internal override uint IntersectMultiSegment(IntersectionResult results, IMultiSegmentGeometry that)
        {
            return IntersectionHelper.Intersect(results, (IMultiSegmentGeometry)this, that);
        }

        internal override uint IntersectArc(IntersectionResult results, ICircularArcGeometry that)
        {
            return IntersectionHelper.Intersect(results, (IMultiSegmentGeometry)this, that);
        }

        internal override uint IntersectCircle(IntersectionResult results, ICircleGeometry that)
        {
            return IntersectionHelper.Intersect(results, (IMultiSegmentGeometry)this, that);
        }

        internal override IPosition GetOrient(bool fromStart, double crvDist)
        {
            IPointGeometry[] d = Data;

            if (fromStart)
                return d[1];
            else
                return d[d.Length-2];
        }

        /// <summary>
        /// The line geometry that corresponds to a section of this line.
        /// </summary>
        /// <param name="s">The required section</param>
        /// <returns>The corresponding geometry for the section</returns>
        internal override UnsectionedLineGeometry Section(ISection s)
        {
            IPointGeometry[] data = this.Data;

            // Locate the segments where the section starts and ends
            int sIndex = FindSegment(data, true, 0, s.From);
            if (sIndex<0)
                throw new Exception("Bad line section (start)");

            int eIndex = FindSegment(data, true, sIndex, s.To);
            if (eIndex<0)
                throw new Exception("Bad line section (end)");

            Debug.Assert(eIndex>=sIndex);

            // Copy over the relevant data. Note that if the to-point coincides with
            // the very end of a segment, the eIndex we have refers to the start of
            // the segment. We want everything up to the start of the eIndex segment,
            // then we'll append the end of the section.
            int len = eIndex - sIndex + 2;

            if (len > 2)
            {
                IPointGeometry[] result = new IPointGeometry[len];
                Array.Copy(data, sIndex, result, 0, len-1);

                // And ensure the result terminates at the section terminals.
                result[0] = s.From;
                result[len-1] = s.To;
                return new MultiSegmentGeometry(s.From, s.To, result);
            }

            return new SegmentGeometry(s.From, s.To);
        }

        /// <summary>
        /// Finds the line segment that a position lies on (if any).
        /// </summary>
        /// <param name="data">The data to search</param>
        /// <param name="doFirst">Should the start of the first segment be considered? (default=true)</param>
        /// <param name="startIndex">The array index of the element in <c>data</c> where
        /// the search should start (default=0).</param>
        /// <param name="find">The position to find</param>
        /// <returns>The index of the segment which the search point is coincident with (-1 if not found)</returns>
        static int FindSegment(IPointGeometry[] data, bool doFirst, int startIndex, IPointGeometry find)
        {
            // There have to be at least 2 positions.
            if (data.Length<2)
                return -1;

            // Get the position of the vertex to find
            double x = find.X;
            double y = find.Y;

            // Loop through each line segment. The search point must lie somewhere
            // on, or within the window of the segment.

            int seg;			// Index to the vertex at the END of a segment
            double xs, ys;		// Start of segment
            double xe, ye;		// End of segment

            // If the first point is to be excluded, and it matches the
            // search position, start on the second line segment instead.
            if (!doFirst && data[startIndex].IsCoincident(find))
            {
                xs = data[startIndex+1].X;
                ys = data[startIndex+1].Y;
                seg = startIndex+2;
            }
            else
            {
                xs = data[startIndex].X;
                ys = data[startIndex].Y;
                seg = startIndex+1;
            }

            for (; seg<data.Length; seg++)
            {
                // Get the easting and northing at the end of the segment
                xe = data[seg].X;
                ye = data[seg].Y;

                // If the point to find lies within the window of the segment,
                // determine the perpendicular distance (squared) between the
                // vertex and the line segment. If within tol, return the
                // current segment number.

                if (x >= Math.Min(xs, xe)-Constants.XYTOL &&
                    x <= Math.Max(xs, xe)+Constants.XYTOL &&
			        y >= Math.Min(ys, ye)-Constants.XYTOL &&
                    y <= Math.Max(ys, ye)+Constants.XYTOL)
                {
                    if (Geom.DistanceSquared(x, y, xs, ys, xe, ye) < Constants.XYTOLSQ)
                        return seg-1;
                }

                // The end of this line segment is the start of the next one.
                xs = xe;
                ys = ye;
            }

            // No match found
            return -1;
        }

        /// <summary>
        /// Gets geometric info for this geometry. For use during the formation
        /// of <c>Polygon</c> objects.
        /// </summary>
        /// <param name="window">The window of the geometry</param>
        /// <param name="area">The area (in square meters) between the geometry and the Y-axis.</param>
        /// <param name="length">The length of the geometry (in meters on the (projected) ground).</param>
        internal override void GetGeometry(out IWindow win, out double area, out double length)
        {
            // Get a list of vertices for this line
            IPosition[] vlist = Data;

            // Get the window
	        win = new Window(vlist);

            // Initialize the other return data
	        area = 0.0;
	        length = 0.0;

            // Get the initial XY
            double xprv = vlist[0].X;
            double yprv = vlist[0].Y;
            double xcur, ycur;

            // Loop through each line segment, accumulating the length
            // and area to the left of the line.

	        for (int i=1; i<vlist.Length; i++, xprv=xcur, yprv=ycur)
            {
                // Get position at the end of the line segment.
                xcur = vlist[i].X;
                ycur = vlist[i].Y;

                // Figure out the delta XY for the segment
		        double dx = xprv-xcur;
		        double dy = yprv-ycur;

                // Add the length of this segment to the total.
                length += Math.Sqrt(dx*dx + dy*dy);

                // Use the mid-X of the segment to get the area left (signed).
                // If the line is directed up the way, it contributes a positive
                // area. If directed down, it contributes a negative area. So,
                // if flat, it contributes nothing (what we are actually
                // calculating here is double the area; we will adjust this
                // when we are done with the loop).

		        area += (dy*(xcur+xprv));
	        }

            // Adjust the total area
	        area *= 0.5;
        }

        /// <summary>
        /// Gets the most easterly position for this line. If more than one position has the
        /// same easting, one of them will be picked arbitrarily.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal override IPosition GetEastPoint()
        {
            IPointGeometry[] pts = Data;
            IPointGeometry mosteast = pts[0];
            long mx = mosteast.Easting.Microns;

            for (int i=1; i<pts.Length; i++)
            {
                if (pts[i].Easting.Microns > mx)
                {
                    mosteast = pts[i];
                    mx = mosteast.Easting.Microns;
                }
            }

            return mosteast;
        }

        /// <summary>
        /// Determines which side of a line a horizontal line segment lies on.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="hr">The horizontal line segment</param>
        /// <returns>Code indicating the position of the horizontal segment with respect to this line.
        /// Side.Left if the horizontal segment is to the left of this line; Side.Right if to the
        /// right of this line; Side.Unknown if the side cannot be determined (this line is
        /// horizontal).
        /// </returns>
        internal override Side GetSide(HorizontalRay hr)
        {
            // Find the segment containing the end of the horizontal segment
            IPointGeometry[] data = Data;
            int seg = FindSegment(data, true, 0, hr.End);
            if (seg<0)
                return Side.Unknown; // not found => big problem!

            // What's the northing of the horizontal segment guy?
            double hy = hr.Y;

            // Scan the remainder of the line, looking for a point which is
            // off the horizontal. If below the horizontal, it's to the right
            // of the line (& above=>left).

            for (int i=seg+1; i<data.Length; i++)
            {
                double y = data[i].Y;
                if ((hy-y) > Constants.TINY)	// y<hy
                    return Side.Right;
                if ((y-hy) > Constants.TINY)	// y>hy
                    return Side.Left;
            }

            // If we didn't find any suitable points, try traversing back down
            // the line, and adopt the reverse logic for figuring out the side.

            for (int i=seg; i>=0; i--)
            {
                double y = data[i].Y;
                if ((y-hy) > Constants.TINY)	// y>hy
                    return Side.Right;
                if ((hy-y) > Constants.TINY)	// y<hy
                    return Side.Left;
            }

            // The line is completely horizontal, so we have an intersection
            // at the start or the end of the line. This should have already
            // been caught, so we're fucked now.

            return Side.Unknown;
        }

        /// <summary>
        /// Cuts back a horizontal line segment to the closest intersection with this line.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="s">Start of horizontal segment.</param>
        /// <param name="e">End of segment (will be modified if segment intersects this line)</param>
        /// <param name="status">Return code indicating whether an error has arisen (returned
        /// as 0 if no error).</param>
        /// <returns>True if the horizontal line was cut back.</returns>
        internal override bool GetCloser(IPointGeometry s, ref PointGeometry e, out uint status)
        {
            status = 0;

            // Remember the initial end of segment
            PointGeometry initEnd = new PointGeometry(e);

            // Represent the horizontal segment in a class of its own
            HorizontalRay hseg = new HorizontalRay(s, e.X-s.X);
            if (!hseg.IsValid)
            {
                status = 1;
                return false;
            }

            IPointGeometry[] data = this.Data;

            // Get relative position code for the start of the line. If it's
            // somewhere on the horizontal segment, cut the line back.
            byte scode = Geom.GetPositionCode(data[0], s, e);
            if (scode==0)
                e = new PointGeometry(Start);

            // Loop through each line segment, testing the end of each segment
            // against the horizontal segment.
            byte ecode;
            for (int i=1; i<data.Length; scode=ecode, i++)
            {
                // Get the position code for the end of the line segment
                ecode = Geom.GetPositionCode(data[i], s, e);

                // If it's coincident with the horizontal segment, cut the
                // line back. Otherwise see whether there is any potential
                // intersection to cut back to.

                if (ecode==0)
                    e = new PointGeometry(data[i]);
                else if ((scode & ecode)==0)
                {
                    IPosition x=null;
                    if (hseg.Intersect(data[i-1], data[i], ref x))
                        e = new PointGeometry(x);
                }
            }

            // Return flag to indicate whether we got closer or not.
            return (!e.IsCoincident(initEnd));
        }

        /// <summary>
        /// Loads a list of positions with data for this line.
        /// </summary>
        /// <param name="positions">The list to append to</param>
        /// <param name="reverse">Should the data be appended in reverse order?</param>
        /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
        /// <param name="arcTol">Tolerance for approximating circular arcs (used only if the
        /// geometry is an instance of <see cref="ArcGeometry"/>)</param>
        internal override void AppendPositions(List<IPosition> positions, bool reverse, bool wantFirst, ILength arcTol)
        {
            AppendPositions(this.Data, positions, reverse, wantFirst);
        }

        /// <summary>
        /// Loads a list of positions with data for a line.
        /// </summary>
        /// <param name="data">The data for a line</param>
        /// <param name="positions">The list to append to</param>
        /// <param name="reverse">Should the data be appended in reverse order?</param>
        /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
        /// <remarks>This method is also used by <c>ArcGeometry</c></remarks>
        internal static void AppendPositions(IPointGeometry[] data, List<IPosition> positions, bool reverse, bool wantFirst)
        {
            if (reverse)
            {
                for (int i = (wantFirst ? data.Length-1 : data.Length-2); i>=0; i--)
                    positions.Add(data[i]);
            }
            else
            {
                for (int i = (wantFirst ? 0 : 1); i<data.Length; i++)
                    positions.Add(data[i]);
            }
        }

        internal override uint Intersect(IntersectionResult results)
        {
            return results.IntersectMultiSegment(this);
        }

        /// <summary>
        /// Gets the point on this line that is closest to a specified position.
        /// </summary>
        /// <param name="p">The position to search from.</param>
        /// <param name="tol">Maximum distance from line to the search position</param>
        /// <returns>The closest position (null if the line is further away than the specified
        /// max distance)</returns>
        internal override IPosition GetClosest(IPointGeometry p, ILength tol)
        {
            return FindClosest(p, tol, true);
        }

        /// <summary>
        /// Gets the point on this line that is closest to a specified position.
        /// </summary>
        /// <param name="p">The position to search from.</param>
        /// <param name="tol">Maximum distance from line to the search position</param>
        /// <param name="doLast">Specify true to consider the last segment. False to ignore the
        /// last segment.</param>
        /// <returns>The closest position (null if the line is further away than the specified
        /// max distance)</returns>
        IPosition FindClosest(IPointGeometry p, ILength tol, bool doLast)
        {
            IPointGeometry[] data = Data;

            // Initial "best" distance squared cannot be greater than the square of the match tolerance.
            double tm = tol.Meters;
            double bestdsq = tm * tm;

            // Best segment number so far (valid segment numbers start at 1).
            int best = 0;

            // Pull out the XY of the search vertex
            double vx = p.X;
            double vy = p.Y;

            // Get start of the initial line segment
            double x1 = data[0].X;
            double y1 = data[0].Y;

            // Only do the last segment when required.
            int nv = data.Length;
            if (!doLast)
                nv--;

            for (int i=1; i<nv; i++)
            {
                // Pick up the end of the segment & get the window of
                // the segment, expanded by the match tolerance.
                double x2 = data[i].X;
                double y2 = data[i].Y;

                double xmin = Math.Min(x1, x2) - tm;
                double ymin = Math.Min(y1, y2) - tm;
                double xmax = Math.Max(x1, x2) + tm;
                double ymax = Math.Max(y1, y2) + tm;

                // If the search vertex falls within the expanded window,
                // and the distance (squared) to the perpendicular point
                // is better than what we already got, remember the index
                // number of this segment.

                if (vx>xmin && vx<xmax && vy>ymin && vy<ymax)
                {
                    double dsq = Geom.DistanceSquared(vx, vy, x1, y1, x2, y2);
                    if (dsq < bestdsq)
                    {
                        bestdsq = dsq;
                        best = i;
                    }
                }

                // End of segment is start of next one
                x1 = x2;
                y1 = y2;
            }

            // Return if we did not locate a suitable point
            if (best==0)
                return null;

            // Get the position of the perpendicular point.
            double xp, yp;
            IPointGeometry s = data[best-1];
            IPointGeometry e = data[best];
            Geom.GetPerpendicular(vx, vy, s.X, s.Y, e.X, e.Y, out xp, out yp);
            return new Position(xp, yp);
        }

        /// <summary>
        /// Assigns sort values to the supplied intersections (each sort value
        /// indicates the distance from the start of this line).
        /// </summary>
        /// <param name="data">The intersection data to update</param>
        internal override void SetSortValues(List<IntersectionData> dataList)
        {
            // Get an array of cumulative distances for each segment.
            IPointGeometry[] data = this.Data;
            double[] cumdist = GetCumDist(data);

            foreach (IntersectionData xd in dataList)
            {
                // Get the first intersection
                IPointGeometry pos = PointGeometry.Create(xd.P1);
                double xi = pos.X;
                double yi = pos.Y;

                // Find the segment number
                // @devnote -- we may need to use a different function at
                // this point, because FindSegment currently assumes that
                // the intersection must lie within the window of the segment.
                int segnum = FindSegment(data, true, 0, pos);
                if (segnum<0)
                    throw new Exception("MultiSegmentGeometry.SetSortValues - intersection not on line");

                // Get position of the start of segment
                double xs = data[segnum].X;
                double ys = data[segnum].Y;

                // Get the distance squared to the start of segment
                double dx = xi-xs;
                double dy = yi-ys;
                double dsq = (dx*dx + dy*dy);

                // If we have a graze, process the 2nd intersection too
                // (it MUST lie on the same segment, given that multisegments
                // are intersected by processing each segment in turn).
                // If it's closer than the distance we already have, use
                // the second intersection as the sort value, and treat
                // it subsequently as the first intersection.

                if (xd.IsGraze)
                {
                    xi = xd.P2.X;
                    yi = xd.P2.Y;

                    dx = xi-xs;
                    dy = yi-ys;

                    double dsq2 = (dx*dx + dy*dy);

                    if (dsq2 < dsq)
                    {
                        xd.Reverse();
                        dsq = dsq2;
                    }
                }

                // Set the sort value
                double dset = cumdist[segnum] + Math.Sqrt(dsq);
                xd.SortValue = dset;
            }
        }

        /// <summary>
        /// Creates an array with the cumulative distance to the start of each segment.
        /// </summary>
        /// <param name="data">The data defining a multi-segment</param>
        /// <returns>The distances to each successive point. This has the same number of
        /// elements as the supplied array. The first element will contain a value of 0.0</returns>
        static double[] GetCumDist(IPointGeometry[] data)
        {
            double[] result = new double[data.Length];

            result[0] = 0.0;

            double xs = data[0].X;
            double ys = data[0].Y;

            for (int i=1; i<data.Length; i++)
            {
                double xe = data[i].X;
                double ye = data[i].Y;
                double dx = xe-xs;
                double dy = ye-ys;
                double seglen = Math.Sqrt(dx*dx + dy*dy);
                result[i] = result[i-1] + seglen;
                xs = xe;
                ys = ye;
            }

            return result;
        }

        /// <summary>
        /// Intersects this multi-segment with itself. Only SIMPLE intersections will be
        /// found. There are no special checks for multi-segments that graze themselves.
        /// </summary>
        /// <param name="xsect">The intersection results.</param>
        /// <returns>The number of self-intersections found.</returns>
        uint SelfIntersect(IntersectionResult xsect)
        {
            uint nx = 0;

            // Get an array of cumulative distances for each segment.
            IPointGeometry[] data = this.Data;
            double[] cumdist = GetCumDist(data);

            // Note start of initial segment (treat as the end of some imaginary line prior to the start).
            double xs;
            double ys;
            double xe = data[0].X;
            double ye = data[0].Y;

            // How many line segments have we got?
            int nseg = data.Length-1;

            // Loop through each segment, intersecting it with all subsequent
            // segments, except for the one that immediately follows.
            for (int iseg=1; iseg<=(nseg-2); iseg++)
            {
                // The start of this segment is the end of the previous one.
                xs = xe;
                ys = ye;

                // Get the position of the end of the test segment
                xe = data[iseg].X;
                ye = data[iseg].Y;

                // Compare against subsequent segments (except the next one)
                for (int jseg=iseg+2; jseg<=nseg; jseg++)
                {
                    IPointGeometry start = data[jseg-1];
                    IPointGeometry end = data[jseg];
                    double xi, yi;

                    if (Geom.CalcIntersect(start.X, start.Y, end.X, end.Y, xs, ys, xe, ye, out xi, out yi, true)!=0)
                    {
                        // Define distance to the intersection on the i-segment
                        double dx = xi-xs;
                        double dy = yi-ys;
                        double ilen = cumdist[iseg-1] + Math.Sqrt(dx*dx + dy*dy);

                        // Likewise for the j-segment
                        dx = xi - start.X;
                        dy = yi - start.Y;
                        double jlen = cumdist[jseg-1] + Math.Sqrt(dx*dx + dy*dy);

                        // Append TWO intersections.
                        xsect.Append(xi, yi, ilen);
                        xsect.Append(xi, yi, jlen);
                        nx += 2;
                    }
                }
            }

            // Sort the intersections (DON'T set new sort values).
            xsect.Sort(false);

            // Return the number of intersections
            return nx;
        }

        /// <summary>
        /// Calculates an angle that is parallel to this line (suitable for adding text)
        /// </summary>
        /// <param name="p">A significant point on the line. In the case of lines
        /// that are multi-segments, the individual line segment that contains this
        /// position should be used to obtain the angle.</param>
        /// <returns>The rotation (in radians, clockwise from horizontal)</returns>
        internal override double GetRotation(IPointGeometry p)
        {
            // Locate the segment containing the supplied position.
            // If not found, return horizontal rotation.
            IPointGeometry[] data = this.Data;
            int segnum = FindSegment(data, true, 0, p);
            if (segnum < 0)
                return 0.0;

            double xs = data[segnum].X;
            double ys = data[segnum].Y;
            double xe = data[segnum + 1].X;
            double ye = data[segnum + 1].Y;
            double dx = xe - xs;
            double dy = ye - ys;

            // Horizontal (to nearest mm)
            double ady = Math.Abs(dy);
            if (ady < 0.001)
                return 0.0;

            // Vertical (to nearest mm)
            double adx = Math.Abs(dx);
            if (adx < 0.001)
                return MathConstants.PIDIV2;

            // Get result in range (0,PIDIV2)
            double rotation = Math.Atan(ady / adx);

            // Stuff in the NE and SW quadrants needs to be tweaked.
            if ((dx < 0.0 && dy < 0.0) || (dx > 0.0 && dy > 0.0))
            {
                rotation = -rotation;
                if (rotation < 0.0)
                    rotation += MathConstants.PIMUL2;
            }

            return rotation;
        }

        /// <summary>
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            // Write out array of expanded positions (there aren't that many
            // multi-segments in a cadastral database).

            PointGeometry[] data = GetUnpackedData();
            writer.WriteArray("PositionArray", "Position", data);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);

            // Locate the feature that's being read
            LineFeature f = reader.FindParent<LineFeature>();
            Debug.Assert(f!=null);

            // Read in the positions of the line
            PointGeometry[] data = reader.ReadArray<PointGeometry>("PositionArray", "Position");
            Debug.Assert(data.Length>2);

            // Confirm that the terminal positions match the position of the line end points (assumes
            // that the line feature has picked up the end points before parsing the geometry)
            Debug.Assert(data[0].IsCoincident(f.StartPoint));
            Debug.Assert(data[data.Length-1].IsCoincident(f.EndPoint));

            // Pack the data
            SetPackedData(data);
        }
    }
}
