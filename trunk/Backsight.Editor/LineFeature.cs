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
using System.Drawing;

using Backsight.Environment;
using Backsight.Forms;
using System.Drawing.Drawing2D;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="24-JUL-1997" was="CeArc" />
    /// <summary>
    /// A line feature.
    /// </summary>
    [Serializable]
    class LineFeature : Feature, IFeatureDependent, IIntersectable
    {
        #region Class data

        /// <summary>
        /// The topology (if any) for this line. Null if this line is non-topological.
        /// </summary>
        Topology m_Topology;

        /// <summary>
        /// The geometry for this line.
        /// </summary>
        LineGeometry m_Geom;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineFeature</c>
        /// </summary>
        /// <param name="e">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="g">The geometry defining the shape of the line (not null)</param>
        /// <note>To ensure that the start and end of all lines are instances of <see cref="PointFeature"/>,
        /// this constructor should always remain private.</note>
        private LineFeature(IEntity e, Operation creator, LineGeometry g)
            : base(e, creator)
        {
            if (g==null)
                throw new ArgumentNullException();

            m_Geom = g;
            m_Topology = null;
            AddReferences();

            // If the entity type denotes a topological boundary, initialize the topology.
            if (e.IsPolygonBoundaryValid)
                SetTopology(true);
        }

        /// <summary>
        /// Creates a <c>LineFeature</c> consisting of a simple line segment.
        /// </summary>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        internal LineFeature(IEntity e, Operation creator, PointFeature start, PointFeature end)
            : this(e, creator, new SegmentGeometry(start, end))
        {
        }

        /// <summary>
        /// Creates a <c>LineFeature</c> consisting of a series of connected line segments.
        /// </summary>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <param name="data">The positions defining the shape of the line. The first position must
        /// coincide precisely with the supplied <paramref name="start"/>, and the last position
        /// must coincide precisely with <paramref name="end"/>. Expected to be more than two positions.</param>
        internal LineFeature(IEntity e, Operation creator, PointFeature start, PointFeature end, IPointGeometry[] data)
            : this(e, creator, new MultiSegmentGeometry(start, end, data))
        {
            Debug.Assert(data.Length>2);
            Debug.Assert(start.PointGeometry.IsCoincident(data[0]));
            Debug.Assert(end.PointGeometry.IsCoincident(data[data.Length-1]));
        }

        /// <summary>
        /// Creates a <c>LineFeature</c> that corresponds to a circular arc. This constructor
        /// is used only by <see cref="ArcFeature"/>
        /// </summary>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="c">The circle the arc coincides with</param>
        /// <param name="bc">The point at the start of the arc</param>
        /// <param name="ec">The point at the end of the arc</param>
        /// <param name="isClockwise">True if the arc is directed clockwise from start to end</param>
        internal LineFeature(IEntity e, Operation creator, Circle c, PointFeature bc, PointFeature ec, bool isClockwise)
            : this(e, creator, new ArcGeometry(c, bc, ec, isClockwise))
        {
        }

        #endregion

        /// <summary>
        /// The geometry for this line
        /// </summary>
        public LineGeometry LineGeometry // IIntersectable
        {
            get { return m_Geom; }
        }

        /// <summary>
        /// The position at the start of this line.
        /// </summary>
        public IPointGeometry Start
        {
            get { return m_Geom.Start; }
        }

        /// <summary>
        /// The point feature at the start of the line.
        /// </summary>
        internal PointFeature StartPoint
        {
            get { return (PointFeature)Start; }
        }

        /// <summary>
        /// The position at the end of this line.
        /// </summary>
        public IPointGeometry End
        {
            get { return m_Geom.End; }
        }

        /// <summary>
        /// The point feature at the end of the line.
        /// </summary>
        internal PointFeature EndPoint
        {
            get { return (PointFeature)End; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            m_Geom.Render(display, style);

            // If we're highlighting, and points are displayed, render the end points too
            // (if the line is not a polygon boundary, draw hatched ends).
            if ((style is HighlightStyle) && display.MapScale < MapModel.ShowPointScale)
            {
                if (IsTopological)
                {
                    StartPoint.Draw(display, Color.DarkBlue);
                    EndPoint.Draw(display, Color.LightBlue);
                }
                else
                {
                    StartPoint.Draw(display, HatchStyle.DarkUpwardDiagonal, Color.DarkBlue);
                    EndPoint.Draw(display, HatchStyle.DarkUpwardDiagonal, Color.LightBlue);
                }
            }
        }

        public override SpatialType SpatialType
        {
            get { return SpatialType.Line; }
        }

        /// <summary>
        /// The length of this line (on the map projection).
        /// </summary>
        internal ILength Length
        {
            get { return m_Geom.Length; }
        }

        /// <summary>
        /// The length of this line (on the ground).
        /// </summary>
        internal ILength GroundLength
        {
            get
            {
                ICoordinateSystem sys = CoordinateSystem;
                double sfac = sys.GetLineScaleFactor(Start, End);
                ILength length = Length;
                return new Length(length.Meters/sfac);
            }
        }

        /// <summary>
        /// Is this line currently a polygon boundary (with defined topology)
        /// </summary>
        internal bool HasTopology
        {
            get { return (m_Topology!=null); }
        }

        /// <summary>
        /// Marks polygons affected by the introduction of a new line (possibly via
        /// <c>LineFeature.Restore</c>).
        /// <para/>
        /// If the new line falls completely inside an existing polygon, we do not mark
        /// that polygon. What will happen is that this line will end up being identified
        /// as an island of that polygon.
        /// </summary>
        /// <remarks>This method is meant to cover situations where a new line falls
        /// entirely within a polygon (connecting a pair of points on the perimeter of
        /// the polygon). In that situation, no polygons would otherwise be marked for
        /// deletion until a bit too late in the re-building process. If the new line
        /// actually runs across a polygon boundary, the polygons affected will be
        /// marked when the intersection(s) are detected.
        /// </remarks>
        internal void MarkPolygons()
        {
            // Nothing to do if this is not a polygon boundary
            if (!this.IsTopological)
                return;

            // Zero-length lines?
            if (!HasLength)
            {
                IPosition s = this.Start;
                string msg = String.Format("Ignoring zero-length line at {0:0.0000}N {1:0.0000}E",
                                            s.Y, s.X);
                throw new Exception(msg);
            }

            // Mark polygons at the start of this line.
            MarkPolygons(true);

            // Mark polygons at the end of this arc.
            MarkPolygons(false);
        }

        /// <summary>
        /// Marks polygons at one end of this line.
        /// </summary>
        /// <param name="isStart">Start of this line?</param>
        void MarkPolygons(bool isStart)
        {
            // Get a connecting divider
            IDivider d = (isStart ? GetFirstDivider() : GetLastDivider());
            ConnectionFinder cf = new ConnectionFinder(d, isStart);
            IDivider next = cf.Next;
            if (next==null)
                throw new Exception("LineFeature.MarkPolygons - Cannot determine network cycle");

            // If we connected to the start of the connecting divider,
            // mark it's polygons to the right. Otherwise the left.
            if (cf.IsStart)
                Topology.MarkRight(next);
            else
                Topology.MarkLeft(next);
        }

        /// <summary>
        /// Returns the divider (if any) at the start of this line
        /// </summary>
        /// <returns>The divider at the start (may be null)</returns>
        IDivider GetFirstDivider()
        {
            return (m_Topology==null ? null : m_Topology.FirstDivider);
        }

        /// <summary>
        /// Returns the divider (if any) at the end of this line
        /// </summary>
        /// <returns>The divider at the end (may be null)</returns>
        IDivider GetLastDivider()
        {
            return (m_Topology==null ? null : m_Topology.LastDivider);
        }

        /// <summary>
        /// Does this line have non-zero length?
        /// </summary>
        bool HasLength
        {
            get
            {
                // If the end locations of this line are different, the
                // line will generally have non-zero length. The only
                // possible exception is a situation where a duplicate
                // location has been added.

                IPointGeometry s = Start;
                IPointGeometry e = End;

                if (s!=e && !s.IsCoincident(e))
                    return true;

                // Do it the robust way
                return (Length.Meters > Constants.TINY);
            }
        }

        /// <summary>
        /// Marks this line as "deleted". When you do this, any system-defined split
        /// sections will be marked as well. This gets called during rollback.
        /// </summary>
        internal override void Undo()
        {
            if (IsUndoing)
                return;
            /*
	// If the line primitive is used by any subsequent operation,
	// it CAN'T be marked for deletion. Otherwise the op would
	// eventually refer to an area of deleted memory. The only
	// exception is if the primitive is cross-referenced to a
	// CeSplit operation. In that case, we'll be eliminating
	// that below.

	if ( m_pLine->HasAnyOps(this) ) {
		ShowMessage("Attempt to delete a line that is used by another edit.");
		SetInactive(0);
		return;
	}

	// Remember whether this arc is the result of a split (the info
	// will be nulled out when we call CeFeature::Undo).
	const LOGICAL isSplit = IsSplit();

	// Mark the base class (sets FFL_DELETED and nulls creating op).
	CeFeature::SetDeleted();

	// Mark any adjacent polygons.
	SetPolDeleted();

	// Return if this is the result of a split.
	if ( isSplit ) return;

	// Remove any system-defined arc sections.
	RemoveSplits();

	// Check for overlaps with the line's end locations.
	m_pLine->UndoEndOverlaps(*this);

             */
            base.Undo();
        }

        /// <summary>
        /// The circle this line sits on. This implementation returns null. The
        /// derived <see cref="ArcFeature"/> class overrides.
        /// </summary>
        internal virtual Circle Circle
        {
            get { return null; }
        }

        /// <summary>
        /// Returns the point feature that sits at the center of the circle (if any)
        /// that this line is based on.
        /// </summary>
        /// <param name="op">The operation that must be the creator of the centre
        /// point. Specify null (the default) if the creator doesn't matter.</param>
        /// <param name="onlyActive">True (the default) if the point has to be active.
        /// Specify false if inactive points are ok too.</param>
        /// <returns>The centre point (null if no such point).</returns>
        PointFeature GetCenter(Operation op, bool onlyActive)
        {
            Circle circle = this.Circle;
            if (circle==null)
                return null;

            return circle.GetCenter(op, onlyActive);
        }

        /// <summary>
        /// Locates the intersections of this line with a line segment.
        /// </summary>
        /// <param name="results">The results to append to</param>
        /// <param name="start">The start of the segment</param>
        /// <param name="end">The end of the segment</param>
        /// <returns>The number of intersections added to the results</returns>
        /*
        internal uint Intersect(IntersectionResult results, IPointGeometry start, IPointGeometry end)
        {
            ILineSegmentGeometry seg = new LineSegmentGeometry(start, end);
            return IntersectSegment(results, seg);
        }
        */

        /// <summary>
        /// Appends dividers that are associated with this line, and which
        /// terminate at the specified point.
        /// </summary>
        /// <param name="result">The list to append to</param>
        /// <param name="t">The point the dividers must terminate at</param>
        internal void AddIncidentDividers(List<IDivider> result, ITerminal t)
        {
            if (m_Topology!=null)
                m_Topology.AddIncidentDividers(result, t);
        }

        public sealed override IWindow Extent
        {
            get { return m_Geom.Extent; }
        }

        public sealed override ILength Distance(IPosition point)
        {
            return m_Geom.Distance(point);
        }

        //[Implements(IFeatureDependent)]
        public virtual void AddReferences()
        {
            StartPoint.AddReference(this);
            EndPoint.AddReference(this);
        }

        /// <summary>
        /// Mark this line as inactive.
        /// </summary>
        /// <param name="op">The operation that is doing this</param>
        /*
        internal void SetInactive(Operation op)
        {
            throw new NotImplementedException();
        }
        */

        /*
//	@mfunc	Mark this arc as inactive. When you do this, any
//			system-defined split sections will be eliminated.
//
//	@parm	The operation that is doing this. This operation will
//			hold any sub-features that get created as a result of
//			de-activating this arc. Only SetDeleted is expected to
//			pass in a null value.
//	@parm	Is the arc being de-activated in order to trim a
//			dangling system-generated arc section? If so, DON'T
//			undo any splits that are incident on this arc. A
//			TRUE value should be specified ONLY by <mf CeSplit::
//			Trim>. Default=FALSE.
//
//	@rdesc	Any sub-feature that was created to represent the
//			layers that this feature was NOT de-activated on.

CeFeature* CeArc::SetInactive ( CeOperation* pop
							  , const LOGICAL isTrim ) {

	// Return if this arc is already inactive (or marked for deletion).
	if ( this->IsInactive() ) return 0;

	// De-activate the base class.
	CeFeature* pSub = CeFeature::SetInactive(pop);

	// Ensure polygons on the left and right have been marked
	// for deletion.
	SetPolDeleted();

	// Get any split operation listing arc sections (only returns
	// something if FFL_SYSTOPOL is set).

	CeSplit* pSplit = GetpSplit();

	if ( pSplit ) {

		// Return if the splits have been revealed.
		if ( pSplit->IsRevealed() ) return pSub;

		// Mark all splits for deletion.
		RemoveSplits();

		// If we created a sub-arc to represent layers that are
		// not being de-activated, mark it as moved, to force
		// re-intersection.
		if ( pSub ) pSub->SetMoved();
	}

	// Check for overlaps with the line's end locations.
	if ( !isTrim ) m_pLine->UndoEndOverlaps(*this);

	return pSub;

} // end of SetInactive
         */

        /// <summary>
        /// Make a new line that corresponds to a sub-section of this line.
        /// </summary>
        /// <param name="section">The geometry for the new line.</param>
        /// <param name="op">The operation that should be noted as the creator of the new line.</param>
        /// <returns>The new line</returns>
        internal LineFeature MakeSubSection(SectionGeometry section, Operation op) //const CeSubTheme* const pSubTheme
        {
            LineFeature result = new LineFeature(this.EntityType, op, section);
            DefineFeature(result);

            if (result.IsTopological)
                result.m_Topology = Topology.CreateTopology(result);

            return result;
        }

        // Should try to get rid of this...
        protected void ChangeGeometry(ArcGeometry arc)
        {
            m_Geom = arc;
        }

        /// <summary>
        /// Sets or clears topological status. This override creates or nulls the object
        /// for holding topological data, then calls the implementation in the base class.
        /// </summary>
        /// <param name="topol">True to mark this feature as topological. False to clear topology flag.</param>
        internal override void SetTopology(bool topol)
        {
            if (topol)
                m_Topology = Topology.CreateTopology(this);
            else
                m_Topology = null;

            base.SetTopology(topol);
        }

        /// <summary>
        /// Ensures that the polygon topology for this line has been completely defined.
        /// </summary>
        /// <param name="bwin">The window of any new polygons that got created. This
        /// window is not initialized here. It just gets expanded.</param>
        /// <param name="index">The spatial index to include any newly built polygons.</param>
        internal void BuildPolygons(Window bwin, IEditSpatialIndex index)
        {
            if (m_Topology==null)
            {
                // Inactive topological lines will have null m_Topology
                // Debug.Assert(!IsTopological);
                return;
            }

            if (m_Topology!=null)
                m_Topology.BuildPolygons(bwin, index);
        }

        /// <summary>
        /// Initializes this feature upon loading of an editing operation that involves
        /// this feature.
        /// </summary>
        /// <param name="op">The operation involved</param>
        /// <param name="isCreator">Is the operation the one that originally created this feature?</param>
        internal override void OnLoad(Operation op, bool isCreator)
        {
            base.OnLoad(op, isCreator);

            if (m_Topology!=null)
            {
                foreach (IDivider d in m_Topology)
                    Topology.OnLoad(d);
            }
        }

        /// <summary>
        /// Override inserts this line feature into the supplied index, together with any neighbouring
        /// polygons that have not already been added to the index. This should be called shortly after
        /// a model is opened (after a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        internal override void AddToIndex(IEditSpatialIndex index)
        {
            // Index this line feature
            base.AddToIndex(index);

            // Index any neighbouring polygons
            if (m_Topology!=null)
            {
                foreach (IDivider d in m_Topology)
                    Topology.AddToIndex(d, index);
            }
        }

        public string BoundaryString
        {
            get { return (m_Topology==null ? String.Empty : m_Topology.ToString()); }
        }

        /// <summary>
        /// Restores (un-deletes) this line.
        /// </summary>
        /// <returns>True if line restored. False if the line wasn't marked as deleted.</returns>
        internal override bool Restore()
        {
            if (!base.Restore())
                return false;

            // If this was a polygon boundary, create an undefined divider, mark
            // affected polygons, and mark the line as "moved" (to force re-intersection
            // with the map).

            if (IsTopological)
            {
                m_Topology = Topology.CreateTopology(this);
                MarkPolygons();
                IsMoved = true;
            }

            return true;
        }

        /// <summary>
        /// Ensures this feature is clean after some sort of edit. If this line has been marked inactive,
        /// any adjacent polygons will be marked for deletion, and the polygon boundary object.
        /// </summary>
        internal override void Clean()
        {
            if (IsInactive)
                SetPolDeleted();

            base.Clean();
        }

        /// <summary>
        /// Marks adjacent polygons (if any) for deletion, and nulls out <see cref="m_Topology"/>
        /// </summary>
        void SetPolDeleted()
        {
            if (m_Topology!=null)
            {
                foreach (IDivider d in m_Topology)
                    Topology.MarkPolygons(d);

                m_Topology = null;
            }
        }

        /// <summary>
        /// Toggles the topological status of this line. This should be called only
        /// by <see cref="SetTopologyOperation.Execute"/>
        /// </summary>
        internal void SwitchTopology()
        {
            if (IsTopological)
            {
                // Mark adjacent polygons for a rebuild (and null m_Topology)
                SetPolDeleted();

                // Clear the flag bit
                SetTopology(false);
            }
            else
            {
                // Create new m_Topology & set the flag bit
                SetTopology(true);

                // Mark polygons that are incident on the end points
                MarkPolygons();

                // Treat the line as "moved" to force re-intersection
                IsMoved = true;
            }
        }

        /// <summary>
        /// Do any splits needed upon addition of a new line.
        /// </summary>
        /// <param name="retrims">Intersecting lines that were split on the invisible
        /// portion of a trimmed line. The lines in this list will need to be re-trimmed.
        /// </param>
        /// <remarks>
        /// 15-NOV-99: This function used to be called all over the place. It should now
        /// be called ONLY by <see cref="CadastralMapModel.Intersect"/>. To indicate that
        /// a line needs to be intersected, set the <see cref="IsMoved"/> property to
        /// true.
        /// </remarks>
        internal void Split(List<LineFeature> retrims)
        {
            // Do nothing if not maintaining topology!
            CadastralMapModel map = this.MapModel;
            if (!map.IsMaintainingTopology)
                return;

            // No need to split lines that aren't topological.
            if (!IsTopological)
                return;

            // Should NEVER something that already has more than one section
            // (they should have been removed if the line was moved).
            if (m_Topology is SectionTopologyList)
                throw new Exception("LineFeature.Split - Line has already been split");

            // Intersect this line with the map (ignoring end to end intersects).
            IntersectionFinder xsect = new IntersectionFinder(m_Geom, false);

            // Cut up any intersections.
            //xsect.SplitX(this, retrims);
        }

        /// <summary>
        /// Cuts up this line at a set of intersections. This method is called only for
        /// the line that actually causes the intersections.
        /// </summary>
        /// <param name="xf">The intersections for this line</param>
        /// <exception cref="ArgumentException">If the supplied intersections do not
        /// relate to this line.</exception>
        internal void SplitAtIntersections(IntersectionFinder xf)
        {
            Debug.Assert(m_Topology!=null);
            Debug.Assert(m_Topology is LineTopology);

            // Confirm the intersections relate to this line
            if (!object.ReferenceEquals(this, xf.Intersector))
                throw new ArgumentException("Attempt to split line using invalid intersection data");

            // Combine the intersections
            IntersectionResult xres = new IntersectionResult(this, xf);

            // If there is a graze at the start of this line, ensure
            // that all polygon incident on the start location have
            // been marked for deletion. Same for the end.
            if (xres.IsStartGrazing)
                StartPoint.MarkPolygons();

            if (xres.IsEndGrazing)
                EndPoint.MarkPolygons();

            // Modify the intersection results so that the exit point
            // of each graze will be treated as a simple intersection.
            // While at it, ensure there are no duplicates, and ensure
            // we don't have any intersections at the end of the line.
            if (xres.Simplify()==0)
                return;

            // Create divider sections. We should make at least ONE split.
            Topology sections = m_Topology.CreateSections(xres);
            if (sections==null)
                throw new Exception("LineFeature.Split - Line split failed");

            m_Topology = sections;
        }

        /// <summary>
        /// The topology for this line (null if the line isn't currently topological).
        /// </summary>
        internal Topology Topology
        {
            get { return m_Topology; }
        }
    }
}
