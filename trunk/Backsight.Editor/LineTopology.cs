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
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology that relates to a complete line.
    /// Base class for <see cref="LineDivider"/> and <see cref="LineOverlap"/>.
    /// </summary>
    /// <seealso cref="SectionTopology"/>
    [Serializable]
    abstract class LineTopology : Topology, IDivider
    {
        #region Class data

        // none

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineTopology</c> that relates to a complete line.
        /// Base class for <see cref="LineDivider"/> and <see cref="LineOverlap"/>.
        /// </summary>
        /// <param name="line">The line the topology relates to.</param>
        /// <seealso cref="SectionTopology"/>
        protected LineTopology(LineFeature line)
            : base(line)
        {
        }

        #endregion

        /// <summary>
        /// The polygon ring to the left of the line.
        /// </summary>
        abstract public Ring Left { get; set; } // IDivider

        /// <summary>
        /// The polygon ring to the right of the line.
        /// </summary>
        abstract public Ring Right { get; set; } // IDivider

        /// <summary>
        /// The geometry of the line that is associated with this topology.
        /// </summary>
        public LineGeometry LineGeometry // IIntersectable, IDivider
        {
            get { return Line.LineGeometry; }
        }

        /// <summary>
        /// The position of the start of this divider (coincides with the start
        /// of the associated line)
        /// </summary>
        public ITerminal From
        {
            get { return Line.StartPoint; }
        }

        /// <summary>
        /// The position of the end of this divider (coincides with the end
        /// of the associated line)
        /// </summary>
        public ITerminal To
        {
            get { return Line.EndPoint; }
        }

        /// <summary>
        /// The divider at the start of the associated line
        /// </summary>
        internal override IDivider FirstDivider
        {
            get { return this; }
        }

        /// <summary>
        /// The divider at the end of the associated line.
        /// </summary>
        internal override IDivider LastDivider
        {
            get { return this; }
        }

        /// <summary>
        /// Returns an enumerator that identifies this instance as the one and only divider
        /// in this topology.
        /// </summary>
        /// <returns>This</returns>
        public override IEnumerator<IDivider> GetEnumerator()
        {
            yield return this;
        }

        public override string ToString()
        {
            return String.Format("L={0} R={1}", (Left==null ? "n/a" : Left.ToString())
                                              , (Right==null ? "n/a" : Right.ToString()));
        }

        /// <summary>
        /// Implements <see cref="IDivider"/> method by returning <c>false</c>,
        /// indicating that this topology is not involved in any sort of overlap.
        /// The <see cref="LineOverlap"/> class overrides.
        /// </summary>
        public virtual bool IsOverlap
        {
            get { return false; }
        }

        /// <summary>
        /// Splits this divider at the locations noted in the supplied data
        /// </summary>
        /// <param name="s">Information about the splits that need to be made.</param>
        public void Cut(SplitData s) // IDivider
        {
            Debug.Assert(!(this is LineOverlap));

            // Return if nothing to do
            IntersectionResult xres = s.Intersections;
            Debug.Assert(xres.IntersectedObject == this);
            List<IntersectionData> data = xres.Intersections;
            if (data==null || data.Count==0)
                return;

            // Ensure that any polygons known to this boundary have been
            // marked for deletion (need to do this in case the intersects
            // we have are only at the line end points, in which case we
            // wouldn't actually change anything).
            Topology.MarkPolygons(this);

            // We'll need the map for creating intersection points
            CadastralMapModel map = CadastralMapModel.Current;

            // Create list of resultant sections
            List<IDivider> result = new List<IDivider>();
            ITerminal from, to;
            from = to = this.From;

            for (int i=0; i<data.Count; i++, from=to)
            {
                // Get the intersection data.
                IntersectionData x = data[i];

                if (x.IsGraze)
                {
                    // There are 4 sorts of graze to deal with:
                    // 1. The graze covers the complete line.
                    // 2. The graze is at the start of the line.
                    // 3. The graze is along some interior portion of the line.
                    // 4. The graze is at the end of the line.

                    // If it's a total graze, there should only be ONE intersection.
                    if (x.IsTotalGraze)
                    {
                        Debug.Assert(data.Count==1);
                        if (data.Count!=1)
                            throw new Exception("LineTopology.Cut - Multiple overlaps detected");

                        // Mark all polygons incident on the terminals.
                        From.MarkPolygons();
                        To.MarkPolygons();

                        // If the graze is total make a non-topological section.
                        result.Add(new LineOverlap(Line));
                        to = this.To;
                    }
                    else if (x.IsStartGraze)
                    {
                        Debug.Assert(i==0);
                        Debug.Assert(from == this.From);

                        // Mark all polygons incident at the start terminal
                        From.MarkPolygons();

                        // Create an overlap at the start of this divider
                        to = map.GetTerminal(x.P2);
                        if (from != to)
                            result.Add(new SectionOverlap(Line, from, to));
                    }
                    else if (x.IsInteriorGraze)
                    {
                        // Add a section from the current tail to the start of the graze
                        // 05-APR-2003 (somehow got a simple x-sect followed by a graze, so ensure we don't add a null section)
                        to = map.GetTerminal(x.P1);
                        if (from != to)
                            result.Add(new SectionDivider(Line, from, to));

                        // Add the overlap
                        from = to;
                        to = map.GetTerminal(x.P2);
                        if (from != to)
                            result.Add(new SectionOverlap(Line, from, to));
                    }
                    else if (x.IsEndGraze)
                    {
                        // Mark all polygons incident on the end terminal
                        To.MarkPolygons();

                        // Add a topological section up to the start of the graze
                        to = map.GetTerminal(x.P1);
                        if (from != to)
                            result.Add(new SectionDivider(Line, from, to));

                        // Add overlap all the way to the end of this divider
                        from = to;
                        to = this.To;
                        if (from != to)
                            result.Add(new SectionOverlap(Line, from, to));

                        // That should be the LAST cut.
                        Debug.Assert((i+1)==data.Count);
                    }
                    else
                    {
                        throw new Exception("LineTopology.Cut - Unexpected graze");
                    }
                }
                else if (!x.IsEnd)
                {
                    // If the intersection is not at either end of the
                    // divider, make a split (both portions topological). Skip
                    // if the sort value is the same as the previous one.

                    to = map.GetTerminal(x.P1);
                    if (from != to)
                        result.Add(new SectionDivider(Line, from, to));
                }
            }

            // Add the last section if we're not already at the end (we'll be at the end if
            // an overlap ran to the end)
            from = to;
            to = this.To;
            if (from != to)
                result.Add(new SectionDivider(Line, from, to));

            // Refer the associated line to the new sections
            if (result.Count>0)
            {
                LineFeature line = this.Line;
                SectionTopologyList newTop = new SectionTopologyList(line, result);
                line.ReplaceTopology(this, newTop);
            }
        }
    }
}
