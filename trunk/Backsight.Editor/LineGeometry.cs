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
using System.Collections.Generic;
using System.Diagnostics;

using Backsight.Geometry;
using Backsight.Editor.Observations;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="03-AUG-2007" />
    /// <summary>
    /// Base class for any sort of line geometry.
    /// </summary>
    abstract class LineGeometry : ILineGeometry, IIntersectable
    {
        #region Class data

        /// <summary>
        /// The start of the connection.
        /// </summary>
        ITerminal m_Start;

        /// <summary>
        /// The end of the connection.
        /// </summary>
        ITerminal m_End;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineGeometry</c> using the supplied terminals.
        /// </summary>
        /// <param name="start">The start of the line.</param>
        /// <param name="end">The end of the line.</param>
        protected LineGeometry(ITerminal start, ITerminal end)
        {
            if (start==null || end==null)
                throw new ArgumentNullException("Null terminal for line geometry");

            m_Start = start;
            m_End = end;
        }

        #endregion

        public IPointGeometry Start
        {
            get { return m_Start; }
        }

        public IPointGeometry End
        {
            get { return m_End; }
        }

        internal ITerminal StartTerminal
        {
            get { return m_Start; }
            set { m_Start = value; }
        }

        internal ITerminal EndTerminal
        {
            get { return m_End; }
            set { m_End = value; }
        }

        abstract public ILength Length { get; }
        abstract public IWindow Extent { get; }
        abstract public ILength Distance(IPosition point);

        abstract internal uint Intersect(IntersectionResult results);
        abstract internal uint IntersectSegment(IntersectionResult results, ILineSegmentGeometry seg);
        abstract internal uint IntersectMultiSegment(IntersectionResult results, IMultiSegmentGeometry line);
        abstract internal uint IntersectArc(IntersectionResult results, ICircularArcGeometry arc);
        abstract internal uint IntersectCircle(IntersectionResult results, ICircleGeometry circle);

        /// <summary>
        /// Gets the position that is a specific distance from the start of this line.
        /// </summary>
        /// <param name="dist">The distance from the start of the line.</param>
        /// <param name="result">The position found</param>
        /// <returns>True if the distance is somewhere ON the line. False if the distance
        /// was less than zero, or more than the line length (in that case, the position
        /// found corresponds to the corresponding terminal point).</returns>
        abstract internal bool GetPosition(ILength dist, out IPosition pos);

        /// <summary>
        /// Calculates the distance from the start of this line to a specific position (on the map projection)
        /// </summary>
        /// <param name="asFarAs">Position on the line that you want the length to. Specify
        /// null for the length of the whole line.</param>
        /// <returns>The length. Less than zero if a position was specified and it is
        /// not on the line.</returns>
        abstract internal ILength GetLength(IPosition asFarAs);

        /// <summary>
        /// Gets the orientation point for a line. This is utilized to form
        /// network topology at the ends of a topological line.
        /// </summary>
        /// <param name="fromStart">True if the orientation from the start of the line is
        /// required. False to get the end orientation.</param>
        /// <param name="crvDist">Orientation distance for circular arcs (irrelevant if
        /// the line isn't a circular arc). Default=0.0</param>
        /// <returns>The orientation point.</returns>
        abstract internal IPosition GetOrient(bool fromStart, double crvDist);

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        abstract internal void Render(ISpatialDisplay display, IDrawStyle style);

        // TODO: make abstract
        /// <summary>
        /// Draws a distance alongside this line.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        /// <param name="dist">The observed distance (if any).</param>
        /// <param name="drawObserved">Draw observed distance? Specify <c>false</c> for
        /// actual distance.</param>
        internal virtual void RenderDistance(ISpatialDisplay display, IDrawStyle style,
                                                Distance dist, bool drawObserved)
        {
        }

        /// <summary>
        /// The geometry that acts as the base for this one.
        /// </summary>
        abstract internal UnsectionedLineGeometry SectionBase { get; }

        /// <summary>
        /// Gets geometric info for this geometry. For use during the formation
        /// of <c>Polygon</c> objects.
        /// </summary>
        /// <param name="window">The window of the geometry</param>
        /// <param name="area">The area (in square meters) between the geometry and the Y-axis.</param>
        /// <param name="length">The length of the geometry (in meters on the (projected) ground).</param>
        abstract internal void GetGeometry(out IWindow win, out double area, out double length);

        /// <summary>
        /// Gets the most easterly position for this line. If more than one position has the
        /// same easting, one of them will be picked arbitrarily.
        /// </summary>
        /// <returns>The most easterly position</returns>
        abstract internal IPosition GetEastPoint();

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
        abstract internal Side GetSide(HorizontalRay hr);

        /// <summary>
        /// Cuts back a horizontal line segment to the closest intersection with this line.
        /// Used in point in polygon.
        /// </summary>
        /// <param name="s">Start of horizontal segment.</param>
        /// <param name="e">End of segment (will be modified if segment intersects this line)</param>
        /// <param name="status">Return code indicating whether an error has arisen (returned
        /// as 0 if no error).</param>
        /// <returns>True if the horizontal line was cut back.</returns>
        abstract internal bool GetCloser(IPointGeometry s, ref PointGeometry e, out uint status);

        /// <summary>
        /// Gets the point on this line that is closest to a specified position.
        /// </summary>
        /// <param name="p">The position to search from.</param>
        /// <param name="tol">Maximum distance from line to the search position</param>
        /// <returns>The closest position (null if the line is further away than the specified
        /// max distance)</returns>
        abstract internal IPosition GetClosest(IPointGeometry p, ILength tol);

        /// <summary>
        /// Loads a list of positions with data for this line.
        /// </summary>
        /// <param name="positions">The list to append to</param>
        /// <param name="reverse">Should the data be appended in reverse order?</param>
        /// <param name="wantFirst">Should the first position be appended? (last if <paramref name="reverse"/> is true)</param>
        /// <param name="arcTol">Tolerance for approximating circular arcs (used only if the
        /// geometry is an instance of <see cref="ArcGeometry"/>)</param>
        abstract internal void AppendPositions(List<IPosition> positions, bool reverse, bool wantFirst, ILength arcTol);


        /// <summary>
        /// Implements <see cref="IIntersectable"/> by returning <c>this</c> as the
        /// line geometry that's involved.
        /// </summary>
        LineGeometry IIntersectable.LineGeometry
        {
            get { return this; }
        }

        /// <summary>
        /// Assigns sort values to the supplied intersections (each sort value
        /// indicates the distance from the start of this line).
        /// </summary>
        /// <param name="data">The intersection data to update</param>
        abstract internal void SetSortValues(List<IntersectionData> data);

        /// <summary>
        /// Calculates an angle that is parallel to this line (suitable for adding text)
        /// </summary>
        /// <param name="p">A significant point on the line. In the case of lines
        /// that are multi-segments, the individual line segment that contains this
        /// position should be used to obtain the angle.</param>
        /// <returns>The rotation (in radians, clockwise from horizontal)</returns>
        abstract internal double GetRotation(IPointGeometry p);

        /// <summary>
        /// Gets the distance string to annotate a line with.
        /// </summary>
        /// <param name="len">The adjusted length (in meters on the ground).</param>
        /// <param name="dist">The observed length (if any).</param>
        /// <param name="drawObserved">Draw the observed distance?</param>
        /// <returns>The distance string (null if the distance is supposed to be the
        /// observed distance, but there is no observed distance.</returns>
        protected string GetDistance(double len, Distance dist, bool drawObserved)
        {
            // Return if we are drawing the observed distance, and we don't have one.
            if (drawObserved && dist == null)
                return null;

            // Get the current display units.
            EditingController ec = EditingController.Current;
            DistanceUnit dunit = ec.DisplayUnit;
            string distr = String.Empty;

            // If we are drawing the observed distance
            if (drawObserved)
            {
                // Display the units only if the distance does not
                // correspond to the current data entry units.

                if (dist.EntryUnit != dunit)
                    distr = dist.Format(true); // with units abbreviation
                else
                    distr = dist.Format(false); // no units abbreviation
            }
            else
            {
                // Drawing adjusted distance.

                // If the current display units are "as entered"

                if (dunit.UnitType == DistanceUnitType.AsEntered)
                {
                    // What's the current data entry unit?
                    DistanceUnit eunit = ec.EntryUnit;

                    // Display the units only if the distance does not
                    // correspond to the current data entry units.
                    if (dist != null)
                    {
                        DistanceUnit entryUnit = dist.EntryUnit;
                        if (entryUnit != eunit)
                            distr = entryUnit.Format(len, true); // with abbrev
                        else
                            distr = entryUnit.Format(len, false); // no abbrev
                    }
                    else
                    {
                        // No observed length, so format the actual length using
                        // the current data entry units (no abbreviation).
                        distr = eunit.Format(len, false);
                    }
                }
                else
                {
                    // Displaying in a specific display unit. Format the
                    // result without any units abbreviation.
                    distr = dunit.Format(len, false);
                }
            }

            // Never show distances with a leading negative sign.
            if (distr.StartsWith("-"))
                distr = distr.Substring(1);

            return distr;
        }
    }
}
