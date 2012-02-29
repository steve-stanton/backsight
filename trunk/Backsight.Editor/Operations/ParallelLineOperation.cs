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

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;


namespace Backsight.Editor.Operations
{
	/// <written by="Steve Stanton" on="01-OCT-1999" was="CeArcParallel"/>
    /// <summary>
    /// Operation to create a parallel line.
    /// </summary>
    class ParallelLineOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        // Required input ...

        /// <summary>
        /// The reference line for the parallel.
        /// </summary>
        LineFeature m_RefLine; // m_pRefArc

        /// <summary>
        /// The offset to the parallel (either a <c>Distance</c>, or an <c>OffsetPoint</c>).
        /// </summary>
        Observation m_Offset;        

        // Optional input ...

        /// <summary>
        /// The 1st terminal arc (if any).
        /// </summary>
        LineFeature m_Term1;

        /// <summary>
        /// The 2nd terminal line (if any). May be the same as <c>m_Term1</c> (e.g. could be a
        /// multi-segment that bends round).
        /// </summary>
        LineFeature m_Term2;
        
        /// <summary>
        /// Flag bits. Currently a value of 0x1 is the only defined value, and means
        /// that the parallel for a circular arc should go in a direction that
        /// is opposite to that of the reference line.
        /// </summary>
        uint m_Flags;

        // Created features ...

        /// <summary>
        /// The parallel that got created.
        /// </summary>
        LineFeature m_ParLine; // m_pParArc

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelLineOperation"/> class
        /// </summary>
        /// <param name="refLine">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c> or
        /// an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on.</param>
        /// <param name="term2">A line that the parallel should end on.</param>
        /// <param name="isArcReversed">Should circular arc be reversed?</param>
        internal ParallelLineOperation(LineFeature refLine, Observation offset, LineFeature term1, LineFeature term2,
                                            bool isArcReversed)
            : base()
        {
            m_RefLine = refLine;
            m_Offset = offset;
            m_Term1 = term1;
            m_Term2 = term2;
            m_Flags = (uint)(isArcReversed ? 1 : 0);
        }

        #endregion

        // Needed by ParallelLineUI for updating ...

        internal LineFeature ReferenceLine { get { return m_RefLine; } }
        internal LineFeature ParallelLine
        {
            get { return m_ParLine; }
            set { m_ParLine = value; }
        }
        internal Observation Offset { get { return m_Offset; } }

        /// <summary>
        /// The point (if any) that was used to define the offset to the parallel line.
        /// </summary>
        internal PointFeature OffsetPoint
        {
            get
            {
                OffsetPoint op = (m_Offset as OffsetPoint);
                return (op==null ? null : op.Point);
            }
        }

        internal bool IsArcReversed { get { return (m_Flags==1); } }
        internal LineFeature Terminal1 { get { return m_Term1; } }
        internal LineFeature Terminal2 { get { return m_Term2; } }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Parallel"; }
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(7);

                if (m_ParLine!=null)
                {
                    result.Add(m_ParLine);

                    // If the point features at the end of the arc were
                    // created by this op, append them too (inactive
                    // points should be considered).

                    PointFeature ps = CreatedStartPoint;
                    if (ps!=null)
                        result.Add(ps);

                    PointFeature pe = CreatedEndPoint;
                    if (pe!=null)
                        result.Add(pe);
                }

                return result.ToArray();
            }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            base.OnRollback();

            // If the offset was an offset point, get rid of reference to this op.
            m_Offset.OnRollback(this);

            // Cut the reference from the reference line to this op. And
	        // refs from any terminal lines.
            if (m_RefLine!=null)
                m_RefLine.CutOp(this);

            if (m_Term1!=null)
                m_Term1.CutOp(this);

            if (m_Term2!=null)
                m_Term2.CutOp(this);

            // Rollback the points at the end of the parallel (if they
	        // were created by this operation).
	        Rollback(CreatedStartPoint);
	        Rollback(CreatedEndPoint);


            // Mark the parallel line as deleted.
	        Rollback(m_ParLine);
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            /*
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the end positions of the parallel line. This
            // will throw an exception if the parallel no longer intersects terminating
            // lines (a message is issued to that effect).
            IPosition spar, epar;

            try
            {
                if (!Calculate(m_RefLine, m_Offset, m_Term1, m_Term2, out spar, out epar))
                    return false;
            }

            catch (Exception ex)
            {
                throw new RollforwardException(this, ex.Message);
            }

            // May need the map.
            CadastralMapModel map = CadastralMapModel.Current;

            // Move the end points (in a situation where the end point
            // was not previously created by this operation, and the
            // end does not agree with what we want, add a new point!).
            PointFeature ps = CreatedStartPoint;
            PointFeature pe = CreatedEndPoint;

            if (ps!=null)
                ps.MovePoint(uc, spar);
            else
            {
                // Get the location at the start of the parallel and
                // see if it's already been moved to the position we
                // want. If not, add a point at the desired location,
                // and alter the parallel line so that it starts there
                // instead.

                IPointGeometry loc = m_ParLine.StartPoint;
                IPointGeometry wantLoc = PointGeometry.Create(spar);

                if (!loc.IsCoincident(wantLoc))
                {
                    ps = AddPoint(wantLoc);
                    m_ParLine.IsMoved = true;
                }
            }

            // Same for the end of the parallel
            if (pe!=null)
                pe.MovePoint(uc, epar);
            else
            {
                IPointGeometry loc = m_ParLine.EndPoint;
                IPointGeometry wantLoc = PointGeometry.Create(epar);

                if (!loc.IsCoincident(wantLoc))
                {
                    pe = AddPoint(wantLoc);
                    m_ParLine.IsMoved = true;
                }
            }

            // If the parallel is a circular arc, check if the arc's
            // circle is still suitable. If not, either adjust the circle,
            // or create a new one. We'll adjust the circle if this
            // operation created the circle, and create a new one if it
            // was created by some earlier operation.

            Circle circle = m_ParLine.Circle;
            if (circle!=null)
            {
                // Define geometry based on the circle's centre and the position
                // of the start of the parallel. If the circles don't match, either
                // adjust the circle, or add a replacement one.
                double parrad = Geom.Distance(circle.Center, spar);
                if (Math.Abs(circle.Radius - parrad) > Constants.XYRES)
                {
                    // Ensure the parallel is known to have moved.
                    m_ParLine.IsMoved = true;

                    if (circle.Creator == this)
                    {
                        // Changing the radius of the circle doesn't actually
                        // flag any dependent ops for update. That should
                        // have happened as a consequence of moving the
                        // end points of the parallel (done above). That's
                        // because the curve incident on the end locations
                        // would have been told, which would tell attached
                        // lines, which would tell the dependent ops.
                        circle.ChangeRadius((ArcFeature)m_ParLine, parrad);
                    }
                    else
                    {
                        // Create a new circle.
                        Circle newCircle = map.AddCircle(circle.CenterPoint, parrad);

                        // Transfer the arc to the new circle.
                        ArcFeature arc = (m_ParLine as ArcFeature);
                        arc.Move(newCircle, arc.IsClockwise);
                    }
		        }
            }

            // Ensure that the parallel really does end at the locations where it's expected to be.
            // SS:25-JUN-07... is this still needed?
            // if (m_ParLine.ChangeEnds(ps, pe))
            //     m_ParLine.IsMoved = true;

	        // Rollforward the base class.
	        return base.OnRollforward();
             */
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.Parallel; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();

            if (m_RefLine != null)
                result.Add(m_RefLine);

            if (m_Term1 != null)
                result.Add(m_Term1);

            if (m_Term2 != null && m_Term2 != m_Term1)
                result.Add(m_Term2);

            result.AddRange(m_Offset.GetReferences());
            return result.ToArray();
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="refLine">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c> or
        /// an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on.</param>
        /// <param name="term2">A line that the parallel should end on.</param>
        /// <param name="isArcReversed">Should circular arc be reversed?</param>
        internal void Execute()
        {
            // Calculate the mathematical position of the parallel points.
            IPosition spar, epar;
            if (!Calculate(m_RefLine, m_Offset, m_Term1, m_Term2, out spar, out epar))
                throw new ApplicationException("Cannot calculate position of parallel line");

            // Don't attempt to add a parallel that starts and stops at the same place.
            if (spar.IsAt(epar, Double.Epsilon))
                throw new ApplicationException("Parallel line has the same start and end points.");

            FeatureFactory ff = new FeatureFactory(this);

            // If a point will be needed at the start of the parallel line, remember that
            // fact so that ProcessFeatures will know what to do.

            IEntity pointType = MapModel.DefaultPointType;

            if (!IsPositionAtOffsetPoint(spar))
            {
                IFeature f = CreateFeatureDescription(pointType);
                ff.AddFeatureDescription(DataField.From, f);
            }

            if (!IsPositionAtOffsetPoint(epar))
            {
                IFeature f = CreateFeatureDescription(pointType);
                ff.AddFeatureDescription(DataField.To, f);
            }

            base.Execute(ff);
        }

        /// <summary>
        /// Creates basic information for a new feature that will be created by this edit.
        /// </summary>
        /// <param name="e">The entity type for the feature</param>
        /// <returns>Information for the new feature</returns>
        IFeature CreateFeatureDescription(IEntity e)
        {
            FeatureId fid = null;
            IdHandle h = new IdHandle();
            if (h.ReserveId(e, 0))
                fid = h.CreateId();

            return new FeatureStub(this, e, fid);
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            // If this method is being called during deserialization from the database, you
            // can't easily determine whether the parallel ends at an offset point (since we
            // haven't yet calculated where the parallel line is located). In that case, we
            // can utilize the IDs of the "From" and "To" items that should be there on
            // deserialization.

            // The problem is that during the initial execution, we don't have a way to
            // pass through the positions that were determined in the Execute method. To
            // cover that, the Execute method is expected to add an item description if
            // a new feature needs to be created.

            PointFeature from, to;

            if (ff.HasFeatureDescription(DataField.From))
                from = ff.CreatePointFeature(DataField.From);
            else
                from = this.OffsetPoint;

            if (ff.HasFeatureDescription(DataField.To))
                to = ff.CreatePointFeature(DataField.To);
            else
                to = this.OffsetPoint;

            Debug.Assert(from != null);
            Debug.Assert(to != null);

            // Create a circular arc (without any circle) if the reference line is a circular
            // arc, or a section that's based on a circular arc.

            if (m_RefLine.GetArcBase() == null)
                m_ParLine = ff.CreateSegmentLineFeature(DataField.NewLine, from, to);
            else
                m_ParLine = ff.CreateArcFeature(DataField.NewLine, from, to);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            // Calculate the end positions
            IPosition spos, epos;
            if (!Calculate(out spos, out epos))
                throw new Exception("Failed to calculate parallel line positions");

            // Apply the calculated positions so long as the end points of the parallel line
            // were created by this edit
            if (m_ParLine.StartPoint.Creator == this)
                m_ParLine.StartPoint.ApplyPointGeometry(ctx, PointGeometry.Create(spos));

            if (m_ParLine.EndPoint.Creator == this)
                m_ParLine.EndPoint.ApplyPointGeometry(ctx, PointGeometry.Create(epos));

            // If the parallel is an arc, define the geometry
            if (m_ParLine is ArcFeature)
            {
                // Get the center of the reference line
                ArcFeature refArc = m_RefLine.GetArcBase();
                PointFeature center = refArc.Circle.CenterPoint;

                // Obtain a circle for the parallel
                double radius = Geom.Distance(center, m_ParLine.StartPoint);
                Circle circle = MapModel.AddCircle(center, radius);

                // Define arc direction
                bool iscw = refArc.IsClockwise;
                if (IsArcReversed)
                    iscw = !iscw;

                ArcGeometry geom = new ArcGeometry(circle, m_ParLine.StartPoint, m_ParLine.EndPoint, iscw);
                (m_ParLine as ArcFeature).Geometry = geom;
            }
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="refline">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c> or an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on.</param>
        /// <param name="term2">A line that the parallel should end on.</param>
        /// <param name="isArcReversed">Should circular arc be reversed?</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(LineFeature refline, Observation offset,
            LineFeature term1, LineFeature term2, bool isArcReversed)
        {
            UpdateItemCollection result = new UpdateItemCollection();

            result.AddFeature<LineFeature>(DataField.RefLine, m_RefLine, refline);
            result.AddObservation<Observation>(DataField.Offset, m_Offset, offset);
            result.AddFeature<LineFeature>(DataField.Term1, m_Term1, term1);
            result.AddFeature<LineFeature>(DataField.Term2, m_Term2, term2);
            result.AddItem<bool>(DataField.ReverseArc, this.IsArcReversed, isArcReversed);

            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteFeature<LineFeature>(editSerializer, DataField.RefLine);
            data.WriteObservation<Observation>(editSerializer, DataField.Offset);
            data.WriteFeature<LineFeature>(editSerializer, DataField.Term1);
            data.WriteFeature<LineFeature>(editSerializer, DataField.Term2);
            data.WriteItem<bool>(editSerializer, DataField.ReverseArc);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadFeature<LineFeature>(editDeserializer, DataField.RefLine);
            result.ReadObservation<Observation>(editDeserializer, DataField.Offset);
            result.ReadFeature<LineFeature>(editDeserializer, DataField.Term1);
            result.ReadFeature<LineFeature>(editDeserializer, DataField.Term2);
            result.ReadItem<bool>(editDeserializer, DataField.ReverseArc);
            return result;
        }

        /// <summary>
        /// Modifies this edit by applying the values in the supplied update items
        /// (as produced via a prior call to <see cref="GetUpdateItems"/>).
        /// </summary>
        /// <param name="data">The update items to apply to this edit.</param>
        /// <returns>The original values for the update items.</returns>
        public override void ExchangeData(UpdateItemCollection data)
        {
            m_RefLine = data.ExchangeFeature<LineFeature>(this, DataField.RefLine, m_RefLine);
            m_Offset = data.ExchangeObservation<Observation>(this, DataField.Offset, m_Offset);
            m_Term1 = data.ExchangeFeature<LineFeature>(this, DataField.Term1, m_Term1);
            m_Term2 = data.ExchangeFeature<LineFeature>(this, DataField.Term2, m_Term2);

            // Alter arc direction if necessary.
            bool isArcReversed = data.ExchangeValue<bool>(DataField.ReverseArc, this.IsArcReversed);

            if (isArcReversed)
                m_Flags = 1;
            else
                m_Flags = 0;
        }

        /// <summary>
        /// Does a position coincide with a point feature that was used to define the
        /// offset to the parallel line?
        /// </summary>
        /// <param name="loc">The test position</param>
        /// <returns>True if the offset was defined using a point feature (as opposed to an
        /// explicit distance), and the supplied position is exactly coincident.</returns>
        bool IsPositionAtOffsetPoint(IPosition loc)
        {
            PointFeature p = this.OffsetPoint;
            if (p == null)
                return false;

            IPointGeometry pg = PointGeometry.Create(loc);
            return p.IsCoincident(pg);
        }

        /// <summary>
        /// Adds a termination point (or re-use a point if it happens to be the offset point).
        /// </summary>
        /// <param name="loc">The location for the termination point.</param>
        /// <returns>The point feature at the termination point (may be the position that was
        /// used to define the offset to the parallel line).</returns>
        PointFeature AddPoint(IPosition loc)
        {
            CadastralMapModel map = CadastralMapModel.Current;

            // Add the split point (with default entity type). If a
	        // point already exists at the location, you'll get back
	        // that point instead.

	        // We do this in case the user has decided to
	        // terminate on a line connected to the offset point
	        // that defines the offset to the parallel (which the
	        // UI lets the user do).

            PointFeature p = this.OffsetPoint;
            if (p != null)
            {
                IPointGeometry pg = PointGeometry.Create(loc);
                if (p.IsCoincident(pg))
                    return p;
            }

            p = map.AddPoint(p, map.DefaultPointType, this);
            p.SetNextId();

            return p;
        }

        /// <summary>
        /// The point created at the start of the parallel (null if the point
        /// previously existed).
        /// </summary>
        PointFeature CreatedStartPoint
        {
            get
            {
                if (m_ParLine==null)
                    return null;

                PointFeature p = m_ParLine.StartPoint;
                Debug.Assert(p!=null);
                return (p.Creator==this ? p : null);
            }
        }

        /// <summary>
        /// The point created at the end of the parallel (null if the point
        /// previously existed).
        /// </summary>
        PointFeature CreatedEndPoint
        {
            get
            {
                if (m_ParLine==null)
                    return null;

                PointFeature p = m_ParLine.EndPoint;
                Debug.Assert(p!=null);
                return (p.Creator==this ? p : null);
            }
        }

        /// <summary>
        /// Calculates the position of the parallel points
        /// </summary>
        /// <param name="spar">The start of the parallel.</param>
        /// <param name="epar">The end of the parallel.</param>
        /// <returns>True if calculated ok.</returns>
        bool Calculate(out IPosition spar, out IPosition epar)
        {
            return Calculate(m_RefLine, m_Offset, m_Term1, m_Term2, out spar, out epar);
        }

        /// <summary>
        /// Calculates the terminal positions for the parallel.
        /// </summary>
        /// <param name="refLine">The reference line.</param>
        /// <param name="offset">The observed offset (either a <c>Distance</c>
        /// or an <c>OffsetPoint</c>).</param>
        /// <param name="term1">A line that the parallel should start on (may be null).</param>
        /// <param name="term2">A line that the parallel should end on (may be null).</param>
        /// <param name="spar">The start of the parallel.</param>
        /// <param name="epar">The end of the parallel.</param>
        /// <returns>True if calculated ok.</returns>
        bool Calculate( LineFeature refLine
                      , Observation offset
                      , LineFeature term1
                      , LineFeature term2
                      , out IPosition spar
                      , out IPosition epar )
        {
            spar = epar = null;

	        if (!ParallelLineUI.Calculate(refLine, offset, out spar, out epar))
                return false;

	        // If the start of the parallel should begin on a specific
	        // line, get the closest intersection.

        	if (term1!=null)
            {
                spar = ParallelLineUI.GetIntersect(refLine, spar, term1);
		        if (spar == null)
                    throw new Exception("Parallel does not intersect terminal line.");
    		}

        	// And similarly for the end of the parallel.

	        if (term2!=null)
            {
                epar = ParallelLineUI.GetIntersect(refLine, epar, term2);
                if (epar == null)
                    throw new Exception("Parallel does not intersect terminal line.");
    		}

            return true;
	    }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>Null (always), since this edit doesn't supersede any lines.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);
            editSerializer.WriteFeatureRef<LineFeature>(DataField.RefLine, m_RefLine);

            if (m_Term1 != null)
                editSerializer.WriteFeatureRef<LineFeature>(DataField.Term1, m_Term1);

            if (m_Term2 != null)
                editSerializer.WriteFeatureRef<LineFeature>(DataField.Term2, m_Term2);

            if (IsArcReversed)
                editSerializer.WriteBool(DataField.ReverseArc, true);

            editSerializer.WritePersistent<Observation>(DataField.Offset, m_Offset);

            if (m_ParLine.StartPoint != OffsetPoint)
                editSerializer.WritePersistent<FeatureStub>(DataField.From, new FeatureStub(m_ParLine.StartPoint));

            if (m_ParLine.EndPoint != OffsetPoint)
                editSerializer.WritePersistent<FeatureStub>(DataField.To, new FeatureStub(m_ParLine.EndPoint));

            editSerializer.WritePersistent<FeatureStub>(DataField.NewLine, new FeatureStub(m_ParLine));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParallelLineOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal ParallelLineOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_RefLine = editDeserializer.ReadFeatureRef<LineFeature>(DataField.RefLine);

            if (editDeserializer.IsNextField(DataField.Term1))
                m_Term1 = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Term1);

            if (editDeserializer.IsNextField(DataField.Term2))
                m_Term2 = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Term2);

            if (editDeserializer.IsNextField(DataField.ReverseArc) && editDeserializer.ReadBool(DataField.ReverseArc) == true)
                m_Flags = 1;

            m_Offset = editDeserializer.ReadPersistent<Observation>(DataField.Offset);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub(DataField.From, editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.From));
            dff.AddFeatureStub(DataField.To, editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.To));
            dff.AddFeatureStub(DataField.NewLine, editDeserializer.ReadPersistent<FeatureStub>(DataField.NewLine));
            ProcessFeatures(dff);
        }
    }
}
