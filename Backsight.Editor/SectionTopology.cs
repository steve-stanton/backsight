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
using System.Diagnostics;
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="29-OCT-2007" />
    /// <summary>
    /// Topology for a section of line.
    /// Base class for <see cref="SectionDivider"/> and <see cref="SectionOverlap"/>.
    /// </summary>
    /// <seealso cref="LineTopology"/>
    /// <seealso cref="SectionTopologyList"/>
    [Serializable]
    abstract class SectionTopology : ISection, IDivider
    {
        #region Class data

        /// <summary>
        /// The line this topological section coincides with. The geometry for this feature
        /// may be an instance of <see cref="SectionGeometry"/> (consequently, the geometry
        /// for this <c>SectionTopology</c> object may be a section on a section).
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// The start position for the topological section.
        /// </summary>
        readonly ITerminal m_From;

        /// <summary>
        /// The end position for the topological section.
        /// </summary>
        readonly ITerminal m_To;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SectionTopology</c>
        /// </summary>
        /// <param name="line">The line this topological section partially coincides with.</param>
        /// <param name="from">The start position for the topological section.</param>
        /// <param name="to">The end position for the topological section.</param>
        protected SectionTopology(LineFeature line, ITerminal from, ITerminal to)
        {
            m_Line = line;
            m_From = from;
            m_To = to;
        }

        #endregion

        /// <summary>
        /// The polygon ring to the left of this section of line
        /// </summary>
        abstract public Ring Left { get; set; } // IDivider

        /// <summary>
        /// The polygon ring to the right of this section of line
        /// </summary>
        abstract public Ring Right { get; set; } // IDivider

        /// <summary>
        /// The line the section partially coincides with.
        /// </summary>
        public LineFeature Line // IDivider
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The start position for the section.
        /// </summary>
        public ITerminal From // ISection
        {
            get { return m_From; }
        }

        /// <summary>
        /// The end position for the section.
        /// </summary>
        public ITerminal To // ISection
        {
            get { return m_To; }
        }

        /// <summary>
        /// The geometry of the section of the line feature associated with this topology.
        /// </summary>
        public LineGeometry LineGeometry // IIntersectable, IDivider
        {
            get
            {
                // Note that the geometry associated with the boundary line may be an instance
                // of SectionGeometry (in that case, we need to return a section on a section).
                return m_Line.LineGeometry.SectionBase.Section(this);
            }
        }
        /// <summary>
        /// Implements <see cref="IDivider"/> method by returning <c>false</c>,
        /// indicating that this topology is not involved in any sort of overlap.
        /// The <see cref="SectionOverlap"/> class overrides.
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
            Debug.Assert(!(this is SectionOverlap)); // diff

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
                            throw new Exception("SectionTopology.Cut - Multiple overlaps detected");

                        // Mark all polygons incident on the terminals.
                        From.MarkPolygons();
                        To.MarkPolygons();

                        // If the graze is total make a non-topological section.
                        result.Add(new SectionOverlap(Line, From, To)); // diff
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
                // diff...
                SectionTopologyList container = (SectionTopologyList)this.Line.Topology;
                container.ReplaceDivider(this, result);
            }
        }
    }
}
