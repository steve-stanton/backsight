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
    /// <written by="Steve Stanton" on="21-AUG-2007" />
    /// <summary>
    /// A series of connected boundaries, forming the edge of a polygon.
    /// </summary>
    [Serializable]
    abstract class Ring : RingMetrics, ISpatialObject
    {
        static int s_TotalRing = 0; // for testing

        #region Statics

        /// <summary>
        /// Creates a new <c>Ring</c> that's either a <see cref="Polygon"/> or an <see cref="Island"/>.
        /// </summary>
        /// <param name="edge">The boundaries that define the perimeter of the ring</param>
        internal static Ring Create(List<BoundaryFace> edge)
        {
            RingMetrics rm = new RingMetrics(edge);

            if (rm.SignedArea <= 0.0)
                return new Island(rm, edge);
            else
                return new Polygon(rm, edge);
        }

        #endregion

        #region Class data

        /// <summary>
        /// The boundaries defining the edge of the ring.
        /// </summary>
        readonly Boundary[] m_Edge;

        /// <summary>
        /// Flag bits
        /// </summary>
        RingFlag m_Flag;

        /// <summary>
        /// A numeric ID for this ring, useful for debugging.
        /// </summary>
        int m_TestId;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor used by <c>Ring.Create</c>.
        /// </summary>
        /// <param name="edge">The boundaries that define the perimeter of the ring</param>
        protected Ring(RingMetrics metrics, List<BoundaryFace> edge)
            : base(metrics)
        {
            s_TotalRing++;
            m_TestId = s_TotalRing;

            m_Edge = new Boundary[edge.Count];
            m_Flag = 0;

            for (int i=0; i<m_Edge.Length; i++)
            {
                BoundaryFace face = edge[i];
                Boundary b = face.Boundary;

                // Check for a boundary that's already marked as being totally built.
                if (b.IsBuilt)
                    throw new Exception("Polygon - Wrong build status for component boundary");

                // Remember the boundary and update polygon geometry
                m_Edge[i] = b;

                if (face.IsLeft)
                    b.Left = this;
                else
                    b.Right = this;
            }
        }

        #endregion

        public override string ToString()
        {
            return m_TestId.ToString();
        }

        public int TestId
        {
            get { return m_TestId; }
        }

        /// <summary>
        /// The area of this ring, excluding any islands. This version is suitable only for
        /// rings that are instances of <c>Island</c> (the <c>Polygon</c> class overrides).
        /// </summary>
        internal virtual double AreaExcludingIslands
        {
            get { return Area; }
        }

        /// <summary>
        /// The boundaries defining the edge of the ring.
        /// </summary>
        internal Boundary[] Edge
        {
            get { return m_Edge; }
        }

        #region ISpatialObject Members

        /// <summary>
        /// The spatial type is <c>Polygon</c> for both the <see cref="Polygon"/>
        /// and the <see cref="Island"/> classes.
        /// </summary>
        public SpatialType SpatialType
        {
            get { return SpatialType.Polygon; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        abstract public void Render(ISpatialDisplay display, IDrawStyle style);

        /// <summary>
        /// The shortest distance from the specified position to the perimeter of this ring.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>The shortest distance to a component boundary</returns>
        public ILength Distance(IPosition point)
        {
            // I guess the sensible thing to do would be to find the shortest distance
            // to the component boundaries. Not sure whether Polygon class would override
            // to also consider island boundaries.

            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        /// <summary>
        /// Is a flag bit set?
        /// </summary>
        /// <param name="flag">The flag(s) to check for (may be a combination of more
        /// than one flag)</param>
        /// <returns>True if any of the supplied flag bits are set</returns>
        protected bool IsFlagSet(RingFlag flag)
        {
            return ((m_Flag & flag)!=0);
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        protected void SetFlag(RingFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
        }

        /// <summary>
        /// Has this ring been spatially indexed?
        /// </summary>
        internal bool IsIndexed
        {
            get { return IsFlagSet(RingFlag.Indexed); }
            set { SetFlag(RingFlag.Indexed, value); }
        }

        /// <summary>
        /// Has this ring been marked for deletion? Marking a ring for deletion
        /// does not change the value of the <see cref="IsIndexed"/> property, although
        /// it is likely that any index entry will be removed shortly thereafter.
        /// </summary>
        internal bool IsDeleted
        {
            get { return IsFlagSet(RingFlag.Deleted); }
            set { SetFlag(RingFlag.Deleted, value); }
        }

        /// <summary>
        /// Checks if this ring encloses a specific position. 
        /// </summary>
        /// <param name="p">The position to check</param>
        /// <returns>True is this ring encloses the position. False if the position
        /// is outside, or ON the edge.</returns>
        /// <remarks>Do not override. Any island polygons are intentionally ignored.</remarks>
        internal bool IsRingEnclosing(IPosition p)
        {
            // Return if the position doesn't fall within the window of this polygon
            if (!this.Extent.IsOverlap(p))
                return false;

            // Set up a search line from the test position to the edge of this
            // polygon's window.
            IPosition east = new Position(this.Extent.Max.X, p.Y);

            // Round off to the nearest micron on the ground
            PointGeometry vs = new PointGeometry(p);
            PointGeometry ve = new PointGeometry(east);

            Boundary closest = null;

            // For each boundary we have in the list, locate the closest
            // point of intersection with the search line.
            foreach (Boundary b in m_Edge)
            {
                uint error;
                if (b.GetCloser(vs, ref ve, out error))
                    closest = b;

                // Return if the vertex coincides with the closest point
                // on the boundary (point exactly on the edge of this polygon).
                if (error!=0 || vs.IsCoincident(ve))
                    return false;
            }

            // We SHOULD have got something.
            if (closest==null)
                return false;

            // Which side of the boundary does the directed line hit?
            Boundary sideBoundary;
            Side side = closest.GetSide(vs, ve, out sideBoundary);

            // Check if we're hitting the boundary from the inside. If this
            // ring is an island, things are backwards.
            if (this is Island)
            {
                if (side==Side.Right && sideBoundary.Left==this)
                    return true;
                if (side==Side.Left && sideBoundary.Right==this)
                    return true;
            }
            else
            {
                if (side==Side.Left && sideBoundary.Left==this)
                    return true;
                if (side==Side.Right && sideBoundary.Right==this)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Inserts this polygon into the supplied index, so long as it's not already
        /// marked as indexed. Then marks it as indexed.
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        /// <returns>True if entry added to index. False if this ring is already marked
        /// as indexed.</returns>
        internal bool AddToIndex(IEditSpatialIndex index)
        {
            if (IsIndexed)
                return false;

            index.Add(this);
            IsIndexed = true;
            return true;
        }

        static internal int RingCount
        {
            get { return s_TotalRing; }
        }

        /// <summary>
        /// The perimeter of this polygon as a single array of positions.
        /// </summary>
        /// <param name="curvetol">Approximation tolerance for any circular arcs along
        /// the edge of this ring.</param>
        /// <returns></returns>
        internal IPosition[] GetOutline(ILength curvetol)
        {
            return GetOutline(curvetol, this, m_Edge);
        }

        /// <summary>
        /// The perimeter of a polygon as a single array of positions.
        /// </summary>
        /// <param name="curvetol">Approximation tolerance for any circular arcs along
        /// the edge of the ring.</param>
        /// <param name="ring">The ring of interest that the <paramref name="edge"/> contains</param>
        /// <param name="edge">The boundaries to consider</param>
        /// <returns></returns>
        internal static IPosition[] GetOutline(ILength curvetol, Ring ring, Boundary[] edge)
        {
            List<IPosition> pts = new List<IPosition>(1000);

            // When doing the 1st arc, we need to utilize the very first position
            bool isFirst = true;

            // Loop through each boundary in the ring to create a list of
            // positions defining the polygon...
            foreach (Boundary b in edge)
            {
                // Skip boundary lines if they have the same ring on both sides. This is
                // meant to exclude boundaries that radiate out from islands. Note that
                // potential bridges towards the interior of polygons are expected to
                // be weeded out prior to call.
                if (b.Left==ring && b.Right==ring)
                    continue;

                // See which way the positions should be arranged.
                bool reverse = (b.Right==ring ? false : true);

                // Get the geometric primitive for the boundary
                LineGeometry line = b.GetLineGeometry();
                line.AppendPositions(pts, reverse, isFirst, curvetol);
                isFirst = false;
            }

            return pts.ToArray();
        }

        /// <summary>
        /// Ensures this ring is clean after some sort of edit. If this ring has been marked
        /// for deletion, references from component boundaries will be nulled out.
        /// <para/>
        /// Any override should first do it's stuff, then call this implementation.
        /// </summary>
        internal virtual void Clean()
        {
            // Return if this ring hasn't been marked for deletion
            if (!IsDeleted)
                return;

            // Ensure component boundaries no longer refer to this ring.
            foreach (Boundary b in m_Edge)
            {
                if (b.Left == this)
                    b.Left = null;

                if (b.Right == this)
                    b.Right = null;
            }
        }
    }
}
