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
using System.ComponentModel;
using System.Collections.Generic;
using System.Diagnostics;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on=20-JUL-1997" />
    /// <summary>
    /// Topological area. A polygon refers to two collections; a collection of
    /// the <see cref="IDivider"/> objects that define the outer perimeter of the area,
    /// and a collection of any islands that may exist within the area. Islands are also
    /// <c>Polygon</c> objects, but they always have an area less than or equal to zero.
    /// </summary>
    [Serializable]
    class Polygon : Ring
    {
        #region Class data

        /// <summary>
        /// Any islands (null if there aren't any).
        /// </summary>
        List<Island> m_Islands;

        /// <summary>
        /// Associated insertion point (may be null)
        /// </summary>
        TextFeature m_Label;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Polygon</c> without any islands, and with undefined insertion point.
        /// </summary>
        /// <param name="rm">The metrics for this polygon</param>
        /// <param name="edge">The boundaries that define the outer perimeter of the polygon</param>
        internal Polygon(RingMetrics rm, List<Face> edge)
            : base(rm, edge)
        {
            Debug.Assert(rm.SignedArea > 0.0);
            m_Islands = null;
            m_Label = null;
        }

        #endregion

        /// <summary>
        /// Relates this polygon to an island.
        /// </summary>
        /// <param name="island">The island to point to.</param>
        /// <return>True if association successfully made.</return>
        internal void ClaimIsland(Island island)
        {
            // Issue error if the inclusion of the island would reduce the
            // area of this polygon to less than zero.
            if (this.Area - island.Area < 0.0)
                throw new Exception("AddIsland - wrong enclosing polygon");

            // Adjust the area of this enclosing polygon
            SetArea(this.Area - island.Area);

            // Append the island to the list of islands known to this polygon
            if (m_Islands==null)
                m_Islands = new List<Island>(1);

            m_Islands.Add(island);

            // Define this polygon as the container for the island.
            island.Container = this;
        }

        /// <summary>
        /// The area of this polygon, excluding any islands.
        /// </summary>
        internal override double AreaExcludingIslands
        {
            get { return base.Area - TotalIslandArea; }
        }

        /// <summary>
        /// The total area of all islands known to this polygon.
        /// </summary>
        double TotalIslandArea
        {
            get
            {
                double total = 0.0;

                if (m_Islands!=null)
                {
                    foreach (Island i in m_Islands)
                        total += i.Area;
                }

                return total;
            }
        }

        /// <summary>
        /// Does this polygon refer to any islands?
        /// </summary>
        internal bool HasAnyIslands
        {
            get { return (IslandCount>0); }
        }

        /// <summary>
        /// The number of islands this polygon refers to.
        /// </summary>
        public int IslandCount
        {
            get { return (m_Islands==null ? 0 : m_Islands.Count); }
        }

        /// <summary>
        /// Does this polygon contain any island that overlaps the supplied position?
        /// </summary>
        /// <param name="p">The position to check</param>
        /// <returns>True if any island encloses the position.</returns>
        internal bool HasIslandEnclosing(IPosition p)
        {
            if (m_Islands==null)
                return false;

            foreach (Island i in m_Islands)
            {
                if (i.IsRingEnclosing(p))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Associates this polygon with the specified label (any label that was
        /// previously associated with this polygon will be released).
        /// </summary>
        /// <param name="label">The label to associate with this polygon (usually appearing
        /// somewhere inside the polygon).</param>
        internal void ClaimLabel(TextFeature label)
        {
            Debug.Assert(label!=null);

            // Refer the label to this polygon (even if this polygon doesn't end up pointing back)
            label.Container = this;

            /*
            if (m_Label!=null && !Object.ReferenceEquals(m_Label, label))
            {
                string msg = String.Format("Label {0} falls inside same polygon as label {1}",
                                                label.ToString(), m_Label.ToString());
                Trace.TraceWarning(msg);
            }
            */

            // Associate this polygon with the supplied label, but only if this polygon
            // isn't already associated with a label.
            if (m_Label==null)
                m_Label = label;
        }

        /// <summary>
        /// Draws this polygon with a pale yellow fill.
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            // For any circular arcs, we will need to determine a suitable
            // arc tolerance, based on the current draw scale. Try 0.1mm at scale.
            ILength curvetol = new Length(0.0001 * display.MapScale);

            List<IPosition[]> outlines = new List<IPosition[]>(1+IslandCount);

            // Grab the fill outline for this polygon.
            List<IDivider[]> edges = GetSimpleEdges();
            foreach(IDivider[] da in edges)
                outlines.Add(GetOutline(curvetol, this, da));

            // Now do any islands (but ignore any that don't overlap the display window)
            if (m_Islands!=null)
            {
                IWindow drawExtent = display.Extent;
                foreach (Island i in m_Islands)
                {
                    if (i.Extent.IsOverlap(drawExtent))
                        outlines.Add(i.GetOutline(curvetol));
                }
            }

            style.Render(display, outlines.ToArray());
        }

        /// <summary>
        /// Ensures this polygon is clean after some sort of edit. If this polygon has been marked
        /// for deletion, this frees any islands, and any enclosed label.
        /// Then <see cref="Ring.Clean"/> will be called.
        /// </summary>
        internal override void Clean()
        {
            // If this polygon has been marked for deletion, ensure islands don't point back
            if (IsDeleted)
            {
                if (m_Islands!=null)
                {
                    foreach (Island i in m_Islands)
                        i.Container = null;
                }

                if (m_Label!=null && m_Label.Container==this)
                    m_Label.Container = null;
            }

            base.Clean();
        }

        /// <summary>
        /// Releases an island of this polygon. This gets done if the island has been
        /// marked for deletion.
        /// </summary>
        /// <param name="i">The island to release</param>
        internal void Release(Island i)
        {
            if (m_Islands==null)
                throw new Exception("Attempt to release island, but polygon doesn't have any islands");

            if (!m_Islands.Remove(i))
                throw new Exception("Failed to remove island from enclosing polygon");

            // Add the area of the island back to this polygon.
            SetArea(Area + i.Area);
        }

        /// <summary>
        /// The text label that acts as a persistent handle for the polygon (may be null).
        /// </summary>
        public TextFeature Label
        {
            get { return m_Label; }
        }

        /// <summary>
        /// Returns one or more divider arrays, excluding dividers that act as bridges
        /// from this polygon to areas that would otherwise be regarded as islands.
        /// </summary>
        /// <returns>One or more divider arrays, where every divider has a different
        /// polygon on right and left.</returns>
        List<IDivider[]> GetSimpleEdges()
        {
            // TODO: Need to revisit this -- it doesn't cover cases where there's more than
            // one interior edge, connected by a series of bridges...

            IDivider[] edge = this.Edge;
            List<IDivider> outerEdge = new List<IDivider>(edge.Length);
            List<IDivider> innerEdge = null;
            IDivider startInner = null;
            List<IDivider[]> inners = null;

            foreach (IDivider d in Edge)
            {
                // If we're current walking the outer edge (haven't hit a potential bridging line)...
                if (innerEdge==null)
                {
                    // If we've now got a potential bridging line, just remember to
                    // drop through to the else block on the next divider. Otherwise
                    // remember the divider is part of the outer edge.
                    if (d.Left==d.Right)
                    {
                        innerEdge = new List<IDivider>();
                        startInner = d;
                    }
                    else
                        outerEdge.Add(d);
                }
                else
                {
                    // We've been walking an interior edge. If we've now got back to the divider
                    // that marked the start of the interior edge
                    if (d==startInner)
                    {
                        // The inner edge may be empty (e.g. we might be dealing with a
                        // simple dangle into the interior of the polygon)
                        if (innerEdge.Count>0)
                        {
                            if (inners==null)
                                inners = new List<IDivider[]>(1);

                            inners.Add(innerEdge.ToArray());
                        }

                        innerEdge = null;
                        startInner = null;
                    }
                    else
                    {
                        // We're currently an interior edge. However, we only want those dividers
                        // that have a different polygon on both sides (ignore boundaries that radiate
                        // out from the interior edge).
                        if (d.Left!=d.Right)
                            innerEdge.Add(d);
                    }
                }
            }

            // Form the result

            List<IDivider[]> result = new List<IDivider[]>();
            if (outerEdge.Count == edge.Length)
            {
                Debug.Assert(inners==null);
                result.Add(edge);
            }
            else
            {
                result.Add(outerEdge.ToArray());

                if (inners!=null)
                {
                    foreach (IDivider[] da in inners)
                        result.Add(da);
                }
            }

            return result;
        }
    }
}
