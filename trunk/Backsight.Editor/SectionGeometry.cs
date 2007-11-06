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

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="06-AUG-2007" />
    /// <summary>
    /// Geometry for a section of a line.
    /// </summary>
    [Serializable]
    class SectionGeometry : LineGeometry, ISection
    {
        #region Class data

        /// <summary>
        /// The geometry that the section is based on (not an instance of <c>SectionGeometry</c>).
        /// </summary>
        readonly UnsectionedLineGeometry m_Base;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineSection</c>
        /// </summary>
        /// <param name="line">The line the section refers to (not null).</param>
        /// <param name="start">The point at the start of the section (coincident with
        /// the specified line)</param>
        /// <param name="end">The point at the end of the section (coincident with
        /// the specified line)</param>
        /// <exception cref="ArgumentNullException">If a null line was specified</exception>
        internal SectionGeometry(LineFeature line, PointFeature start, PointFeature end)
            : base(start, end)
        {
            if (line==null)
                throw new ArgumentNullException();

            // Get the geometry the section is ultimately based on.
            m_Base = line.LineGeometry.SectionBase;
        }

        #endregion

        #region ISection Members

        /// <summary>
        /// The start position for the section.
        /// </summary>
        public ITerminal From
        {
            get { return (ITerminal)Start; }
        }

        /// <summary>
        /// The end position for the section.
        /// </summary>
        public ITerminal To
        {
            get { return (ITerminal)End; }
        }

        #endregion

        /// <summary>
        /// The geometry that acts as the base for this section.
        /// </summary>
        internal override UnsectionedLineGeometry SectionBase
        {
            get { return m_Base; }
        }

        /// <summary>
        /// Creates concrete geometry that corresponds to this line section.
        /// </summary>
        /// <returns>Geometry corresponding to this section.</returns>
        internal UnsectionedLineGeometry Make()
        {
            return m_Base.Section(this);
        }

        public override ILength Length
        {
            get { return Make().Length; }
        }

        public override IWindow Extent
        {
            get { return Make().Extent; }
        }

        public override ILength Distance(IPosition point)
        {
            return Make().Distance(point);
        }

        internal override bool GetPosition(ILength dist, out IPosition pos)
        {
            return Make().GetPosition(dist, out pos);
        }

        internal override uint IntersectSegment(IntersectionResult results, ILineSegmentGeometry seg)
        {
            return Make().IntersectSegment(results, seg);
        }

        internal override uint IntersectMultiSegment(IntersectionResult results, IMultiSegmentGeometry line)
        {
            return Make().IntersectMultiSegment(results, line);
        }

        internal override uint IntersectArc(IntersectionResult results, ICircularArcGeometry arc)
        {
            return Make().IntersectArc(results, arc);
        }

        internal override uint IntersectCircle(IntersectionResult results, ICircleGeometry circle)
        {
            return Make().IntersectCircle(results, circle);
        }

        internal override ILength GetLength(IPosition asFarAs)
        {
            return Make().GetLength(asFarAs);
        }

        internal override IPosition GetOrient(bool fromStart, double crvDist)
        {
            return Make().GetOrient(fromStart, crvDist);
        }

        internal override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            Make().Render(display, style);
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
            Make().GetGeometry(out win, out area, out length);
        }

        /// <summary>
        /// Gets the most easterly position for this line section. If more than one position has the
        /// same easting, one of them will be picked arbitrarily.
        /// </summary>
        /// <returns>The most easterly position</returns>
        internal override IPosition GetEastPoint()
        {
            return Make().GetEastPoint();
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
            return Make().GetSide(hr);
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
            return Make().GetCloser(s, ref e, out status);
        }

        /// <summary>
        /// Loads a list of positions with data for this line.
        /// </summary>
        /// <param name="positions">The list to append to</param>
        /// <param name="reverse">Should the data be appended in reverse order?</param>
        /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
        /// <param name="arcTol">Tolerance for approximating circular arcs (used only if this section
        /// is based on an instance of <see cref="ArcGeometry"/>)</param>
        internal override void AppendPositions(List<IPosition> positions, bool reverse, bool wantFirst, ILength arcTol)
        {
            Make().AppendPositions(positions, reverse, wantFirst, arcTol);
        }

        internal override uint Intersect(IntersectionResult results)
        {
            return Make().Intersect(results);
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
            return Make().GetClosest(p, tol);
        }

        /// <summary>
        /// Assigns sort values to the supplied intersections (each sort value
        /// indicates the distance from the start of this line).
        /// </summary>
        /// <param name="data">The intersection data to update</param>
        internal override void SetSortValues(List<IntersectionData> data)
        {
            Make().SetSortValues(data);
        }
    }
}
