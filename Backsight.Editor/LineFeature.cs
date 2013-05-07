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
using System.Drawing;
using System.Drawing.Drawing2D;

using Backsight.Environment;
using Backsight.Forms;
using Backsight.Editor.Forms;
using Backsight.Editor.Observations;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="24-JUL-1997" was="CeArc" />
    /// <summary>
    /// A line feature.
    /// </summary>
    class LineFeature : Feature, IFeatureDependent, IIntersectable, IPersistent
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

        /// <summary>
        /// The point at the start of this line
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// The point at the end of this line
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// Any observed length for the line (on the ground). 
        /// </summary>
        internal Distance ObservedLength { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a <c>LineFeature</c> consisting of a simple line segment.
        /// </summary>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="id">The internal ID of this feature within the project that created it.</param>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        internal LineFeature(Operation creator, InternalIdValue id, IEntity e, PointFeature start, PointFeature end)
            : this(creator, id, e, start, end, new SegmentGeometry(start, end))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="start">The point at the start of the line (not null).</param>
        /// <param name="end">The point at the end of the line (not null).</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        internal LineFeature(IFeature f, PointFeature start, PointFeature end)
            : this(f, start, end, f.EntityType.IsPolygonBoundaryValid)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="start">The point at the start of the line (not null).</param>
        /// <param name="end">The point at the end of the line (not null).</param>
        /// <param name="isTopological">Should the line be tagged as a polygon boundary?</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        internal LineFeature(IFeature f, PointFeature start, PointFeature end, bool isTopological)
            : this(f, start, end, new SegmentGeometry(start, end), isTopological)
        {
        }

        /// <summary>
        /// Creates a <c>LineFeature</c> consisting of a series of connected line segments.
        /// </summary>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="id">The internal ID of this feature within the project that created it.</param>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <param name="data">The positions defining the shape of the line. The first position must
        /// coincide precisely with the supplied <paramref name="start"/>, and the last position
        /// must coincide precisely with <paramref name="end"/>. Expected to be more than two positions.</param>
        internal LineFeature(Operation creator, InternalIdValue id, IEntity e,
                                            PointFeature start, PointFeature end, PointGeometry[] data)
            : this(creator, id, e, start, end, new MultiSegmentGeometry(start, end, data))
        {
            Debug.Assert(data.Length>2);
            Debug.Assert(start.Geometry.IsCoincident(data[0]));
            Debug.Assert(end.Geometry.IsCoincident(data[data.Length-1]));
        }

        internal LineFeature(Operation creator, InternalIdValue id, SectionGeometry section)
            : this(creator, id, section.BaseLine.EntityType, (PointFeature)section.Start,
                    (PointFeature)section.End, section)
        {
        }

        /// <summary>
        /// Creates a new <c>LineFeature</c>
        /// </summary>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="id">The internal ID of this feature within the
        /// project that created it.</param>
        /// <param name="e">The entity type for the feature (not null)</param>
        /// <param name="g">The geometry defining the shape of the line (not null)</param>
        /// <note>To ensure that the start and end of all lines are instances of <see cref="PointFeature"/>,
        /// this constructor should always remain private.</note>
        protected LineFeature(Operation creator, InternalIdValue id, IEntity e, PointFeature start, PointFeature end, LineGeometry g)
            : base(creator, id, e, null)
        {
            if (g==null)
                throw new ArgumentNullException();

            m_From = start;
            m_To = end;
            m_Geom = g;
            m_Topology = null;
            AddReferences();

            // If the entity type denotes a topological boundary, initialize the topology.
            if (e.IsPolygonBoundaryValid)
                SetTopology(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineFeature"/> class that corresponds to
        /// a section of another line, and records it as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="baseLine">The line that this section is part of</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal LineFeature(IFeature f, LineFeature baseLine, PointFeature start, PointFeature end, bool isTopological)
            : this(f, start, end, new SectionGeometry(baseLine, start, end), isTopological)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="iid">The internal ID for the feature.</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature. If not null,
        /// this will be modified by cross-referencing it to the newly created feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="start">The point at the start of the line (not null)</param>
        /// <param name="end">The point at the end of the line (not null)</param>
        /// <param name="g">The geometry for the line (could be null, although this is only really
        /// expected during deserialization)</param>
        /// <param name="isTopological">Does the line form part of a polygon boundary?</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> or <paramref name="start"/> or <paramref name="end"/> is null.
        /// </exception>
        protected LineFeature(IFeature f, PointFeature start, PointFeature end, LineGeometry g, bool isTopological)
            : base(f)
        {
            if (start == null || end == null)
                throw new ArgumentNullException();

            m_From = start;
            m_To = end;
            m_Geom = g;
            m_Topology = null;

            // Don't cross-reference if we're dealing with a temporary feature
            if (!f.InternalId.IsEmpty)
                AddReferences();

            if (isTopological)
                SetTopology(true);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineFeature"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal LineFeature(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            bool isTopological;
            ReadData(editDeserializer, out m_From, out m_To, out isTopological, out m_Geom);
            AddReferences();
            SetTopology(isTopological);
        }

        #endregion

        /// <summary>
        /// The geometry for this line
        /// </summary>
        public LineGeometry LineGeometry // IIntersectable
        {
            get { return m_Geom; }
            set { m_Geom = value; }
        }

        /// <summary>
        /// The point feature at the start of the line.
        /// </summary>
        internal PointFeature StartPoint
        {
            get { return m_From; }
        }

        /// <summary>
        /// The point feature at the end of the line.
        /// </summary>
        internal PointFeature EndPoint
        {
            get { return m_To; }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
            EditingController ec = EditingController.Current;

            if (style is HighlightStyle)
            {
                // If we're highlighting a non-topological line, always draw it in turquoise,
                // regardless of the supplied style.
                if (IsTopological)
                    RenderLine(display, style);
                else
                {
                    Color oldCol = style.LineColor;
                    style.LineColor = Color.Turquoise;
                    m_Geom.Render(display, style);
                    style.LineColor = oldCol;
                }

                bool showPoints = (style as HighlightStyle).ShowLineEndPoints;

                // If we're highlighting, and points are displayed, render the end points too
                // (if the line is not a polygon boundary, draw hatched ends).
                if (showPoints && display.MapScale < ec.Project.Settings.ShowPointScale)
                {
                    if (IsTopological)
                    {
                        StartPoint.Draw(display, Color.DarkBlue);
                        EndPoint.Draw(display, Color.Cyan);
                    }
                    else
                    {
                        StartPoint.Draw(display, HatchStyle.DarkUpwardDiagonal, Color.DarkBlue);
                        EndPoint.Draw(display, HatchStyle.DarkUpwardDiagonal, Color.Cyan);
                    }
                }
            }
            else
            {
                //RenderLine(display, style);
                if (IsTopological)
                    RenderLine(display, style);
                else
                {
                    Color oldCol = style.LineColor;
                    style.LineColor = Color.Turquoise;
                    m_Geom.Render(display, style);
                    style.LineColor = oldCol;
                }
            }

            // Return if we're not showing actual or observed lengths
            if (!ec.AreLineAnnotationsDrawn)
                return;

        	// See if there is an observed distance.
            Distance dist = this.ObservedLength;

            LineAnnotationStyle annoStyle = ec.LineAnnotationStyle;
            if (annoStyle.ShowAdjustedLengths)
            {
                // Pass in any observed distance, to govern the number of
                // decimal places to show.
                m_Geom.RenderDistance(display, style, dist, false);
            }
            else if (annoStyle.ShowObservedLengths)
            {
                // Display observed distance only if the line has one.
                if (dist != null)
                    m_Geom.RenderDistance(display, style, dist, true);
            }
        }

        /// <summary>
        /// Draws a topological line on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        void RenderLine(ISpatialDisplay display, IDrawStyle style)
        {
            // If we're dealing with a line that's been divided into at least two sections,
            // and the line is marked as trimmed, ensure we only render the non-dangling
            // sections.
            /*
            if (IsTrimmed && (m_Topology is SectionTopologyList))
                (m_Topology as SectionTopologyList).RenderTrimmed(display, style);
            else
                m_Geom.Render(display, style);
             */

            if (m_Topology is SectionTopologyList)
            {
                if (IsTrimmed)
                    (m_Topology as SectionTopologyList).RenderTrimmed(display, style);
                else
                {
                    foreach (IDivider d in m_Topology)
                        RenderDivider(d, display, style);
                }
            }
            else if (m_Topology is IDivider && !style.IsFixed)
                RenderDivider(m_Topology as IDivider, display, style);
            else
                m_Geom.Render(display, style);
        }

        /// <summary>
        /// Draws a portion of this topological line on the specified display
        /// </summary>
        /// <param name="d">The divider that corresponds to a portion of this line (could
        /// be the entire line)</param>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        internal void RenderDivider(IDivider d, ISpatialDisplay display, IDrawStyle style)
        {
            // If we're highlightling, don't attempt to pick up any other display color
            if (style is HighlightStyle || style.IsFixed)
            {
                d.LineGeometry.Render(display, style);
                return;
            }

            Style s = EntityUtil.GetStyle(d, EditingController.Current.ActiveLayer);
            Pen oldPen = style.Pen;

            try
            {
                ScaleSpecificPen ssPen = s.GetPen(display);
                style.Pen = ssPen.Pen;
                d.LineGeometry.Render(display, style);
            }

            finally
            {
                style.Pen = oldPen;
            }
        }

        /// <summary>
        /// Is a divider associated with this line visible?
        /// </summary>
        /// <param name="d">One of the dividers associated with the topology for this line</param>
        /// <returns>False if this line is marked as trimmed, and the specified divider is not
        /// currently drawn.</returns>
        internal bool IsVisible(IDivider d)
        {
            // The logic here should mimic that of RenderTopologicalLine above.
            if (IsTrimmed && (m_Topology is SectionTopologyList))
                return (m_Topology as SectionTopologyList).IsVisible(d);
            else
                return true;
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
                ISpatialSystem sys = SpatialSystem;
                double sfac = sys.GetLineScaleFactor(StartPoint, EndPoint);
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
                string msg = String.Format("Ignoring zero-length line at {0:0.0000}N {1:0.0000}E",
                                            m_From.Y, m_From.X);
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

                if (m_From!=m_To && !m_From.IsCoincident(m_To))
                    return true;

                // Do it the robust way
                return (Length.Meters > Constants.TINY);
            }
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

        /// <summary>
        /// The covering rectangle that encloses this feature.
        /// </summary>
        public sealed override IWindow Extent
        {
            get { return m_Geom.Extent; }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>
        /// The shortest distance between the specified position and this object
        /// </returns>
        public sealed override ILength Distance(IPosition point)
        {
            return m_Geom.Distance(point);
        }

        /// <summary>
        /// Adds references to the points at the ends of this line.
        /// </summary>
        internal virtual void AddReferences()
        {
            StartPoint.AddReference(this);
            EndPoint.AddReference(this);
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public virtual Feature[] GetRequiredFeatures()
        {
            return new Feature[] { StartPoint, EndPoint };
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
        /// <param name="creator">The operation that should be noted as the creator of the new line.</param>
        /// <param name="id">The internal ID to assign to the sub-section.</param>
        /// <param name="section">The geometry for the new line.</param>
        /// <returns>The new line</returns>
        internal LineFeature MakeSubSection(Operation creator, InternalIdValue id, SectionGeometry section)
        {
            LineFeature result = new LineFeature(creator, id, section);

            //PointFeature start = (PointFeature)section.Start;
            //PointFeature end = (PointFeature)section.End;

            //LineFeature result = new LineFeature(this.EntityType, op, start, end, section);
            DefineFeature(result);

            // The resultant sub-section should always have the same topological status
            //if (result.IsTopological)
            //    result.m_Topology = Topology.CreateTopology(result);
            result.SetTopology(this.IsTopological);

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
        internal void BuildPolygons(Window bwin, EditingIndex index)
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
        /// Override inserts this line feature into the supplied index, together with any neighbouring
        /// polygons that have not already been added to the index. This should be called shortly after
        /// a model is opened (after a prior call to <c>OnLoad</c>).
        /// </summary>
        /// <param name="index">The spatial index to add to</param>
        /// <returns>True if the feature was indexed. False if the feature is currently inactive (not
        /// added to the index)</returns>
        internal override bool AddToIndex(EditingIndex index)
        {
            // Index this line feature
            if (!base.AddToIndex(index))
                return false;

            // Index any neighbouring polygons
            if (m_Topology!=null)
            {
                foreach (IDivider d in m_Topology)
                    Topology.AddToIndex(d, index);
            }

            return true;
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
                RemoveTopology();

            base.Clean();
        }

        /// <summary>
        /// Marks adjacent polygons (if any) for deletion, and nulls out <see cref="m_Topology"/>
        /// </summary>
        internal void RemoveTopology()
        {
            if (m_Topology!=null)
            {
                m_Topology.OnLineDeactivation();
                m_Topology = null;
            }
        }

        /// <summary>
        /// Toggles the topological status of this line. This should be called only
        /// by <see cref="SetTopologyOperation.Execute"/>
        /// </summary>
        /// <param name="isLoading">Is the map model being loaded (meaning that polygon topology has
        /// not yet been built)?</param>
        internal void SwitchTopology(bool isLoading)
        {
            if (IsTopological)
            {
                // Mark adjacent polygons for a rebuild (and null m_Topology)
                RemoveTopology();

                // Clear the flag bit
                SetTopology(false);
            }
            else
            {
                // Create new m_Topology & set the flag bit
                SetTopology(true);

                // Mark polygons that are incident on the end points. Don't even try to mark
                // polygons at this stage if the model is being loaded, as it could require access
                // to geometry that has not been calculated yet.
                if (!isLoading)
                    MarkPolygons();

                // Treat the line as "moved" to force re-intersection
                IsMoved = true;
            }
        }

        /// <summary>
        /// Do any splits needed upon addition of a new line.
        /// </summary>
        /// <param name="arePolygonsBuilt">Has polygon topology been built (false if the split
        /// is being done as part of the initial load).</param>
        /// <param name="retrims">Intersecting lines that were split on the invisible
        /// portion of a trimmed line. The lines in this list will need to be re-trimmed.
        /// </param>
        /// <remarks>
        /// 15-NOV-99: This function used to be called all over the place. It should now
        /// be called ONLY by <see cref="CadastralMapModel.Intersect"/>. To indicate that
        /// a line needs to be intersected, set the <see cref="IsMoved"/> property to
        /// true.
        /// </remarks>
        internal void Split(bool arePolygonsBuilt, List<LineFeature> retrims)
        {
            // No need to split lines that aren't topological.
            if (!IsTopological)
                return;

            if (m_Topology==null)
                throw new Exception("LineFeature.Split - topology hasn't been initialized");

            // This method should only be called for complete lines (with no pre-existing
            // topological sections)
            if (!(m_Topology is LineTopology))
            {
                string msg = String.Format("LineFeature.Split - Line {0} is associated with the wrong type of topology ({1})",
                    InternalId, m_Topology.GetType().Name);
                throw new Exception(msg);
            }
            LineTopology lineTop = (LineTopology)m_Topology;

            // Intersect this line with the map (ignoring end to end intersects).
            IntersectionFinder xf = new IntersectionFinder(this, false);

            // Return if no intersections.
            IList<IntersectionResult> intersectedDividers = xf.Intersections;
            if (intersectedDividers.Count==0)
                return;

            // Cut up the things that were intersected, making grazing portions non-topological.
            // Each result object should be associated with an IDivider.
            foreach (IntersectionResult r in intersectedDividers)
                Cut(arePolygonsBuilt, r, retrims);

            // Combine the results and get the splitter to cut itself up.
            IntersectionResult xres = new IntersectionResult(xf);

            if (arePolygonsBuilt)
            {
                // If there is a graze at the start of this line, ensure
                // that all polygons incident on the start location have
                // been marked for deletion. Same for the end.
                if (xres.IsStartGrazing)
                    Topology.MarkPolygons(StartPoint);

                if (xres.IsEndGrazing)
                    Topology.MarkPolygons(EndPoint);
            }

            // Modify the intersection results so that the exit point
            // of each graze will be treated as a simple intersection.
            // While at it, ensure there are no duplicates, and ensure
            // we don't have any intersections at the end of the line.
            if (xres.Simplify()==0)
                return;

            // Create divider sections. We should make at least ONE split.
            Cut(arePolygonsBuilt, xres, retrims);
        }

        /// <summary>
        /// Splits up a series of intersections
        /// </summary>
        /// <param name="arePolygonsBuilt">Has polygon topology been built (false if the split
        /// is being done as part of the initial load).</param>
        /// <param name="xres">The places where intersections have been detected on
        /// some polygon ring divider</param>
        /// <param name="retrims">Lines that need to be re-trimmed (the line associated
        /// with the divider will be appended if an intersection is made on an invisible
        /// portion of the divider). Not currently used.</param>
        static void Cut(bool arePolygonsBuilt, IntersectionResult xres, List<LineFeature> retrims)
        {
            // Grab the thing that's been intersected. It'll be a LineFeature if the
            // intersected object is a new line that's being processed via the Split
            // method.

            IIntersectable ir = xres.IntersectedObject;
            if (ir is LineFeature)
            {
                LineFeature line = (ir as LineFeature);
                Debug.Assert(line.Topology is LineTopology);
                line.Cut(arePolygonsBuilt, (IDivider)line.Topology, xres, retrims);
            }
            else
            {
                Debug.Assert(ir is IDivider);
                IDivider div = (IDivider)ir;
                div.Line.Cut(arePolygonsBuilt, div, xres, retrims);
            }
        }

        /// <summary>
        /// Cuts up a divider that covers this line (or a portion of this line)
        /// </summary>
        /// <param name="arePolygonsBuilt">Has polygon topology been built (false if the split
        /// is being done as part of the initial load).</param>
        /// <param name="div">The divider covering the line (or a portion of this line)</param>
        /// <param name="xres">The places where intersections have been detected on
        /// the divider</param>
        /// <param name="retrims">Lines that need to be re-trimmed (the line associated
        /// with the divider will be appended if an intersection is made on an invisible
        /// portion of the divider). Not currently used.</param>
        void Cut(bool arePolygonsBuilt, IDivider div, IntersectionResult xres, List<LineFeature> retrims)
        {
            Debug.Assert(div.Line == this);

            // Should never need to cut anything that represents an overlap (they're
            // supposed to be treated as non-topological)
            Debug.Assert(!div.IsOverlap);

            // Return if nothing to do
            List<IntersectionData> data = xres.Intersections;
            if (data == null || data.Count == 0)
                return;

            // Ensure that any polygons known to this boundary have been
            // marked for deletion (need to do this in case the intersects
            // we have are only at the line end points, in which case we
            // wouldn't actually change anything).
            if (arePolygonsBuilt)
                Topology.MarkPolygons(div);

            // We'll need the map for creating intersection points
            CadastralMapModel map = CadastralMapModel.Current;

            // Create list of resultant sections
            List<IDivider> result = new List<IDivider>();
            ITerminal from, to;
            from = to = div.From;

            for (int i = 0; i < data.Count; i++, from = to)
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
                        Debug.Assert(data.Count == 1);
                        if (data.Count != 1)
                            throw new Exception("LineFeature.Cut - Multiple overlaps detected");

                        // Mark all polygons incident on the terminals.
                        Topology.MarkPolygons(div.From);
                        Topology.MarkPolygons(div.To);

                        // Define overlap for the entire divider
                        if (div is LineTopology)
                            result.Add(new LineOverlap(div.Line));
                        else
                            result.Add(new SectionOverlap(this, div.From, div.To));

                        to = div.To;
                    }
                    else if (x.IsStartGraze)
                    {
                        Debug.Assert(i == 0);
                        Debug.Assert(from == div.From);

                        // Mark all polygons incident at the start terminal
                        if (arePolygonsBuilt)
                            Topology.MarkPolygons(div.From);

                        // Create an overlap at the start of this divider
                        to = map.GetTerminal(x.P2);
                        if (from != to)
                            result.Add(new SectionOverlap(this, from, to));
                    }
                    else if (x.IsInteriorGraze)
                    {
                        // Add a section from the current tail to the start of the graze
                        // 05-APR-2003 (somehow got a simple x-sect followed by a graze, so ensure we don't add a null section)
                        to = map.GetTerminal(x.P1);
                        if (from != to)
                            result.Add(new SectionDivider(this, from, to));

                        // Add the overlap
                        from = to;
                        to = map.GetTerminal(x.P2);
                        if (from != to)
                            result.Add(new SectionOverlap(this, from, to));
                    }
                    else if (x.IsEndGraze)
                    {
                        // Mark all polygons incident on the end terminal
                        if (arePolygonsBuilt)
                            Topology.MarkPolygons(div.To);

                        // Add a topological section up to the start of the graze
                        to = map.GetTerminal(x.P1);
                        if (from != to)
                            result.Add(new SectionDivider(this, from, to));

                        // Add overlap all the way to the end of this divider
                        from = to;
                        to = div.To;
                        if (from != to)
                            result.Add(new SectionOverlap(this, from, to));

                        // That should be the LAST cut.
                        Debug.Assert((i + 1) == data.Count);
                    }
                    else
                    {
                        throw new Exception("LineFeature.Cut - Unexpected graze");
                    }
                }
                else if (!x.IsEnd)
                {
                    // If the intersection is not at either end of the
                    // divider, make a split (both portions topological).

                    to = map.GetTerminal(x.P1);
                    if (from != to)
                        result.Add(new SectionDivider(this, from, to));
                }
            }

            // Add the last section if we're not already at the end (we'll be at the end if
            // an overlap ran to the end)
            from = to;
            to = div.To;
            if (from != to)
            {
                // Just return if the terminals correspond to the two ends of this line. This
                // is intended to cover the case where we have a single interesection at the
                // start or end of the line.
                if (from.IsCoincident(StartPoint) && to.IsCoincident(EndPoint))
                {
                    Debug.Assert(result.Count==0);
                    return;
                }

                result.Add(new SectionDivider(this, from, to));
            }

            // Refer the associated line to the new sections
            if (result.Count > 0)
            {
                if (m_Topology is LineTopology)
                    m_Topology = new SectionTopologyList(this, result);
                else
                {
                    SectionTopologyList container = (SectionTopologyList)m_Topology;
                    container.ReplaceDivider(div, result);
                }
            }
        }

        /// <summary>
        /// The topology for this line (null if the line isn't currently topological).
        /// </summary>
        internal Topology Topology
        {
            get { return m_Topology; }
        }

        /// <summary>
        /// Intersect this line with another line.
        /// </summary>
        /// <param name="line">The line to intersect with (not equal to THIS line)</param>
        /// <param name="closeTo">The point that the intersection should be closest to.
        /// Specify null if you don't care. In that case, if there are multiple intersections,
        /// you get the intersection that is closest to one of 3 points: the start of the
        /// direction line, the start of the line, or the end of the line.</param>
        /// <param name="xsect">The position of the intersection (if any). Null if not found.</param>
        /// <param name="closest">The default point that is closest to the intersection. Null if
        /// intersection wasn't found.</param>
        /// <returns>True if intersection was found.</returns>
        internal bool Intersect( LineFeature line
                               , PointFeature closeTo
                               , out IPosition xsect
                               , out PointFeature closest)
        {
            // Initialize results
            xsect = null;
            closest = null;

            // Don't intersect a line with itself.
            if (Object.ReferenceEquals(this, line))
                throw new Exception("Cannot intersect a line with itself.");

            // Intersect this line with the other one.
            IntersectionResult xres = new IntersectionResult(this);
            uint nx = line.LineGeometry.Intersect(xres);
            if (nx==0)
                return false;

            // Determine which terminal point is the best.
            double mindsq = Double.MaxValue;

            if (xres.GetCloserPoint(this.StartPoint, ref mindsq, ref xsect))
                closest = this.StartPoint;

            if (xres.GetCloserPoint(this.EndPoint, ref mindsq, ref xsect))
                closest = this.EndPoint;

            if (xres.GetCloserPoint(line.StartPoint, ref mindsq, ref xsect))
                closest = line.StartPoint;

            if (xres.GetCloserPoint(line.EndPoint, ref mindsq, ref xsect))
                closest = line.EndPoint;

            // If a close-to point has been specified, that overrides
            // everything else (however, doing the above has the desired
            // effect of defining the best of the default points). In
            // this case, we allow an intersection that coincides with
            // the line being intersected.

            if (closeTo!=null)
                xres.GetClosest(closeTo, out xsect, 0.0);

            return (xsect!=null);
        }

        /// <summary>
        /// Is the start of this line a topological dangle?
        /// </summary>
        /// <returns>True if this line has topology, but the start is not connected to any other line</returns>
        internal bool IsStartDangle()
        {
            if (m_Topology==null)
                return false;

            IDivider d = m_Topology.FirstDivider;
            return Topology.IsDangle(d, d.From);
        }

        /// <summary>
        /// Is the end of this line a topological dangle?
        /// </summary>
        /// <returns>True if this line has topology, but the end is not connected to any other line</returns>
        internal bool IsEndDangle()
        {
            if (m_Topology==null)
                return false;

            IDivider d = m_Topology.LastDivider;
            return Topology.IsDangle(d, d.To);
        }

        /// <summary>
        /// The name of the derived entity type associated with this line. If
        /// the line has been divided into two or more topological sections
        /// (or it's a non-topological line), the derived entity type name is
        /// blank. A non-blank value will only be returned when dealing with
        /// a topological line that hasn't been sectioned.
        /// </summary>
        public string DerivedEntityTypeName
        {
            get
            {
                if (m_Topology is IDivider)
                    return EntityUtil.GetDerivedType(m_Topology as IDivider,
                        EditingController.Current.ActiveLayer);
                else
                    return String.Empty;
            }
        }

        /*
        /// <summary>
        /// Alters the end points for this line (probably moving the line in the process)
        /// </summary>
        /// <param name="newStart">The new start point</param>
        /// <param name="newEnd">The new end point</param>
        /// <remarks>TODO: This method is potentially dangerous and should be axed</remarks>
        internal virtual void ChangeEnds(PointFeature newStart, PointFeature newEnd)
        {
            // Return if there is no change to make.
            if (Object.ReferenceEquals(StartPoint, newStart) && Object.ReferenceEquals(EndPoint, newEnd))
                return;

            // This is a bit rough, but I think I better solution needs quite a re-think
            if (!(m_Geom is SegmentGeometry))
                throw new Exception("LineFeature.ChangeEnds - Only segment geometry is supported");

            try
            {
                // Remove this line from the spatial index.
                PreMove();

                if (!Object.ReferenceEquals(StartPoint, newStart))
                {
                    StartPoint.CutReference(this);
                    newStart.AddReference(this);
                    m_Geom.StartTerminal = newStart;
                    m_From = newStart;
                }

                if (!Object.ReferenceEquals(EndPoint, newEnd))
                {
                    EndPoint.CutReference(this);
                    newEnd.AddReference(this);
                    m_Geom.EndTerminal = newEnd;
                    m_To = newEnd;
                }
            }

            finally
            {
                // Re-add this line to the spatial index.
                PostMove();
            }
        }
        */

        /*
//	@mfunc	Change the location of one end of this line. This is
//			used by implementations of the ChangeEnds function.
//
//	@parm	The old end location.
//	@parm	The new end location.
//
//	@rdesc	Pointer to the new end location.
CeLocation* CeLine::ChangeEnd ( CeLocation& oldend
							  , CeLocation& newend ) {

	// Cut the reference from the old end location to this line.
	oldend.CutObject(*this);

	// Reference the new end location to this line.
	newend.AddObject(*this);
	return &newend;

} // end of ChangeEnd
         */

        /// <summary>
        /// Deactivates this line.
        /// </summary>
        /// <remarks>This method is currently used only during deactivation during
        /// startup data loading, ensuring that any topological placeholder has been
        /// nulled (removal of any associated Topology is normally done by cleaning
        /// at the end of an edit, but startup doesn't involve cleaning). Should
        /// revisit this.</remarks>
        internal override void Deactivate()
        {
            //this.IsInactive = true;
            base.Deactivate();
            RemoveTopology();
        }

        /// <summary>
        /// Returns the first predecessor (if any) for this line
        /// </summary>
        /// <returns>The predecessor, or null if none.</returns>
        internal LineFeature GetPredecessor()
        {
            // Get the op to find the line (if any) on which it was based.
            return Creator.GetPredecessor(this);
        }

        /// <summary>
        /// Attempts to locate the circular arc (if any) that this line is based on.
        /// </summary>
        /// <returns>The circular arc this line is based on (may well be null, it's only not null
        /// if this line is a section based on an arc). The <see cref="ArcFeature"/> class provides an override.</returns>
        internal virtual ArcFeature GetArcBase()
        {
            SectionGeometry s = (m_Geom as SectionGeometry);
            return (s==null ? null : s.BaseLine.GetArcBase());
        }

        /// <summary>
        /// Performs any processing that needs to be done just before the position of
        /// a referenced feature is changed.
        /// </summary>
        /// <param name="f">The feature that is about to be moved  - something that
        /// the <c>IFeatureDependent</c> is dependent on (not null).</param>
        /// <param name="ctx">The context in which the move is being made (not null).</param>
        public override void OnFeatureMoving(Feature f, UpdateEditingContext ctx)
        {
            // Remove line from spatial index, so long as the feature being moved is
            // an end point.
            bool isEndPointMoving = (f == this.StartPoint || f == this.EndPoint);
            if (isEndPointMoving)
                base.OnFeatureMoving(f, ctx);

            // If we have a line that's been cut up into a series of
            // dividers, remove them all.
            ResetTopology();

            // If the feature that's being changed is a point that isn't
            // one of this line's end points, remove the point->line reference
            if (!isEndPointMoving && (f is PointFeature))
                (f as PointFeature).CutReference(this);
        }

        internal void ResetTopology()
        {
            if (m_Topology != null)
            {
                RemoveTopology();
                m_Topology = Topology.CreateTopology(this);
                IsMoved = true;
            }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<PointFeature>(DataField.From, m_From);
            editSerializer.WriteFeatureRef<PointFeature>(DataField.To, m_To);
            editSerializer.WriteBool(DataField.Topological, IsTopological);

            // For simple line segments (perhaps 80% of the case when dealing with cadastral
            // data), we already have what we need with the from & to points.
            if (!(m_Geom is SegmentGeometry))
                editSerializer.WritePersistent<LineGeometry>(DataField.Type, m_Geom);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="from">The point at the start of the line</param>
        /// <param name="to">The point at the end of the line</param>
        /// <param name="isTopological">Does the line act as a polygon boundary </param>
        /// <param name="geom">The geometry for the line.</param>
        static void ReadData(EditDeserializer editDeserializer, out PointFeature from, out PointFeature to, out bool isTopological,
                                out LineGeometry geom)
        {
            from = editDeserializer.ReadFeatureRef<PointFeature>(DataField.From);
            to = editDeserializer.ReadFeatureRef<PointFeature>(DataField.To);
            isTopological = editDeserializer.ReadBool(DataField.Topological);

            if (editDeserializer.IsNextField(DataField.Type))
            {
                geom = editDeserializer.ReadPersistent<LineGeometry>(DataField.Type);

                // Ensure terminals have been defined (since there was no easy way to pass them
                // through to the LineGeometry constructor).
                geom.StartTerminal = from;
                geom.EndTerminal = to;

                // If we're dealing with a circular arc, and the bc/ec points have defined
                // positions (e.g. coming from an import), we can calculate the radius now.
                // Otherwise we'll need to wait until geometry has been calculated.

                // The radius is defined here only because it's consistent with older code.
                // It may well be better to leave the definition of circle radius till later
                // (i.e. do all circles after geometry has been calculated).

                ArcGeometry arc = (geom as ArcGeometry);
                if (arc != null)
                {
                    Circle c = (Circle)arc.Circle;
                    PointFeature center = c.CenterPoint;

                    if (center.PointGeometry != null && from.PointGeometry != null)
                        c.Radius = Geom.Distance(center.PointGeometry, from.PointGeometry);
                }

            }
            else
            {
                geom = new SegmentGeometry(from, to);
            }
        }
    }
}
