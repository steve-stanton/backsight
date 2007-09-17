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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="24-JUL-1997" was="CeArc" />
    /// <summary>
    /// A line feature.
    /// </summary>
    [Serializable]
    class LineFeature : Feature, IFeatureDependent
    {
        #region Class data

        /// <summary>
        /// The topology (if any) for this line. Null if this line is non-topological.
        /// </summary>
        /// <remarks>
        /// I tried using an array of Boundary (with an array length of 1), and noted that
        /// the size of my test file (containing 6727 lines) grew by 12%. On calculating
        /// the overhead, it seems that each array costs in the region of 50 bytes. This
        /// seems like a lot of overhead, I wonder why.
        /// </remarks>
        IPossibleList<Boundary> m_Topology;

        /// <summary>
        /// The geometry for this line.
        /// </summary>
        LineGeometry m_Geom;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>LineFeature</c>
        /// </summary>
        /// <param name="e">The entity type for the feature.</param>
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
        /// The (extended) geometry for this line
        /// </summary>
        internal LineGeometry LineGeometry
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
            if ((style is HighlightStyle) && display.MapScale < MapModel.ShowPointScale)
            {
                StartPoint.Draw(display, Color.DarkBlue);
                EndPoint.Draw(display, Color.LightBlue);
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
        /// Is this line currently a polygon boundary?
        /// </summary>
        internal bool IsPolygonBoundary
        {
            get { return (m_Topology!=null); }
        }

        /// <summary>
        /// Marks polygons affected by the introduction of a new line (possibly via
        /// <c>LineFeature.Restore</c>).
        /// 
        /// If the new line falls completely inside an existing polygon, we do not mark
        /// that polygon. What will happen is that this arc will end up being identified
        /// as an island of that polygon.
        /// </summary>
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
            throw new NotImplementedException();
            /*
	// Get the integer code required by GetNextArc.
	INT1 endCode = 1;
	if ( isStart ) endCode = -1;

	// Get the layers for this arc.
	CeLayerList layers(*this);

	// Repeat until we have found all layers (if all else fails,
	// we should ultimately detect THIS arc).
	while ( !layers.IsNull() ) {

		// Get a connecting arc.
		INT1 dir;
		const CeArc* const pConnect = GetNextArc(layers,endCode,&dir);

		// Break if we didn't find anything (something screwed up).
		if ( !pConnect ) break;

		// Return if we've somehow got back to this arc.
		if ( pConnect == this ) break;

		// If we connected to the start of the connecting arc,
		// mark it's polygons to the right. Otherwise the left.
		if ( dir<0 )
			pConnect->MarkRight(layers);
		else
			pConnect->MarkLeft(layers);

		// If the layers for the connecting arc are identical
		// to those we were looking for, that's us done.
		CeLayerList conlayers(*pConnect);
		if ( layers == conlayers ) break;

		// Subtract the layers we've covered.
		layers.Subtract(conlayers);
	}
             */
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
        internal override void SetDeleted()
        {
            if (IsDeleted)
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
	// will be nulled out when we call CeFeature::SetDeleted).
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
            base.SetDeleted();
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

        public void SetSortValues(List<IntersectionData> results)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public uint SelfIntersect(List<IntersectionData> results)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        /// <summary>
        /// Appends polygon boundaries that are associated with this line, and which
        /// terminate at the specified point.
        /// </summary>
        /// <param name="result">The list to append to</param>
        /// <param name="layers">The layers the boundaries must overlap</param>
        /// <param name="t">The point the boundaries must terminate at</param>
        internal void AddIncidentBoundaries(List<Boundary> result, LayerList layers, ITerminal t)
        {
            if (m_Topology!=null)
            {
                foreach (Boundary b in m_Topology)
                {
                    if (b.Start.IsCoincident(t) || b.End.IsCoincident(t))
                        result.Add(b);
                }
            }
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
                result.m_Topology = new Boundary(this);

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
            {
                //m_Topology = new Boundary[1];
                //m_Topology[0] = new Boundary(this);
                m_Topology = new Boundary(this);
            }
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

            foreach(Boundary b in m_Topology)
                b.BuildPolygons(bwin, index);
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
                foreach (Boundary b in m_Topology)
                    b.OnLoad();
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
                foreach (Boundary b in m_Topology)
                    b.AddToIndex(index);
            }
        }

        public string BoundaryString
        {
            get
            {
                if (m_Topology==null)
                    return String.Empty;

                if (m_Topology.Count==1)
                    return m_Topology[0].ToString();

                return "(more than one boundary section)";
            }
        }

        /// <summary>
        /// Restores (un-deletes) this line.
        /// </summary>
        /// <returns>True if line restored. False if the line wasn't marked as deleted.</returns>
        internal override bool Restore()
        {
            if (!base.Restore())
                return false;

            // If this was a polygon boundary, create an undefined boundary, mark
            // affected polygons, and mark the line as "moved" (to force re-intersection
            // with the map).

            if (IsTopological)
            {
                m_Topology = new Boundary(this);
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
            {
                if (m_Topology!=null)
                {
                    foreach (Boundary b in m_Topology)
                        b.OnDeleteLine();

                    m_Topology = null;
                }
            }

            base.Clean();
        }
    }
}
