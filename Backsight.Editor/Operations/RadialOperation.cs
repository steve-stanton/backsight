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

using Backsight.Editor.Observations;
using Backsight.Environment;

namespace Backsight.Editor.Operations
{
	/// <written by="Steve Stanton" on="08-DEC-1998" />
    /// <summary>
    /// Operation to add a single sideshot.
    /// </summary>
    /// <remarks>It was originally planned to also provide a RadialStakeout
    /// operation, that would add a whole bunch of sideshots, but there has
    /// been no need for that so far.</remarks>
    class RadialOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        // Observations ...

        /// <summary>
        /// The direction (could contain an offset).
        /// </summary>
        Direction m_Direction;

        /// <summary>
        /// The length of the sideshot arm (either a <see cref="Distance"/> or
        /// an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Length;

        // Creations ...

        /// <summary>
        /// The point at the end of the sideshot arm.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The line (if any) that was added to correspond to the sideshot arm.
        /// </summary>
        LineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialOperation"/> class.
        /// </summary>
        /// <param name="dir">The direction (could contain an offset).</param>
        /// <param name="length">The length of the sideshot arm (either a <see cref="Distance"/> or
        /// an <see cref="OffsetPoint"/>).</param>
        internal RadialOperation(Direction dir, Observation length)
            : base()
        {
            m_Direction = dir;
            m_Length = length;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RadialOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal RadialOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            try
            {
                m_Direction = editDeserializer.ReadPersistent<Direction>(DataField.Direction);
                m_Length = editDeserializer.ReadPersistent<Observation>(DataField.Length);

                DeserializationFactory dff = new DeserializationFactory(this);
                dff.AddFeatureStub(DataField.To, editDeserializer.ReadPersistent<FeatureStub>(DataField.To));
                dff.AddFeatureStub(DataField.Line, editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.Line));
                ProcessFeatures(dff);
            }

            catch
            {
                int junk = 0;
                throw;
            }
        }

        #endregion

        /// <summary>
        /// The point that the sideshot was observed from (the origin of
        /// the observed direction).
        /// </summary>
        internal PointFeature From
        {
            get { return (m_Direction==null ? null : (PointFeature)m_Direction.From); }
        }

        /// <summary>
        /// The point created at the end of the sideshot arm.
        /// </summary>
        internal PointFeature Point
        {
            get { return m_To; }
            set { m_To = value; }
        }

        /// <summary>
        /// The line (if any) that was added to correspond to the sideshot arm.
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
            set { m_Line = value; }
        }

        /// <summary>
        /// The length of the sideshot arm (either a <c>Distance</c> or
        /// an <c>OffsetPoint</c>).
        /// </summary>
        internal Observation Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// The direction (could contain an offset).
        /// </summary>
        internal Direction Direction
        {
            get { return m_Direction; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Sideshot"; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.Radial; }
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="pointId"></param>
        /// <param name="lineType"></param>
        internal void Execute(IdHandle pointId, IEntity lineType)
        {
            // Calculate the position of the sideshot point.
            IPosition to = Calculate(m_Direction, m_Length);
            if (to==null)
                throw new Exception("Cannot calculate position of sideshot point.");

            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature x = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription(DataField.To, x);

            if (lineType != null)
            {
                IFeature f = new FeatureStub(this, lineType, null);
                ff.AddFeatureDescription(DataField.Line, f);
            }

            base.Execute(ff);
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        /// <remarks>This implementation does nothing. Derived classes that need to are
        /// expected to provide a suitable override.</remarks>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            m_To = ff.CreatePointFeature(DataField.To);

            if (ff.HasFeatureDescription(DataField.Line))
            {
                m_Line = ff.CreateSegmentLineFeature(DataField.Line, m_Direction.From, m_To);
                m_Line.ObservedLength = (m_Length as Distance);
            }
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            if (this.EditSequence == 367)
            {
                int junk = 0;
            }

            IPosition p = Calculate(m_Direction, m_Length);
            PointGeometry pg = PointGeometry.Create(p);
            m_To.ApplyPointGeometry(ctx, pg);
        }

        /// <summary>
        /// Calculates the position of the sideshot point.
        /// </summary>
        /// <param name="dir">The direction observation (if any).</param>
        /// <param name="len">The length observation (if any). Could be a <c>Distance</c> or an
        /// <c>OffsetPoint</c>.</param>
        /// <returns>The position of the sideshot point (null if there is insufficient data
        /// to calculate a position)</returns>
        internal static IPosition Calculate(Direction dir, Observation len)
        {
            // Return if there is insufficient data.
            if (dir == null || len == null)
                return null;

            // Get the position of the point the sideshot should radiate from.
            PointFeature from = dir.From;

            // Get the position of the start of the direction line (which may be offset).
            IPosition start = dir.StartPosition;

            // Get the bearing of the direction.
            double bearing = dir.Bearing.Radians;

            // Get the length of the sideshot arm.
            double length = len.GetDistance(from).Meters;

            // Calculate the resultant position. Note that the length is the length along the
            // bearing -- if an offset was specified, the actual length of the line from-to =
            // sqrt(offset*offset + length*length)
            IPosition to = Geom.Polar(start, bearing, length);

            // Return if the length is an offset point. In that case, the length we have obtained
            // is already a length on the mapping plane, so no further reduction should be done
            // (although it's debateable).
            if (len is OffsetPoint)
                return to;

            // Using the position we've just got, reduce the length we used to a length on the
            // mapping plane (it's actually a length on the ground).
            ISpatialSystem sys = CadastralMapModel.Current.SpatialSystem;
            double sfac = sys.GetLineScaleFactor(start, to);
            return Geom.Polar(start, bearing, length * sfac);
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="dir">The direction (could contain an offset).</param>
        /// <param name="length">The length of the sideshot arm (either a <see cref="Distance"/>
        /// or an <see cref="OffsetPoint"/>).</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(Direction dir, Observation length)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddObservation<Direction>(DataField.Direction, m_Direction, dir);
            result.AddObservation<Observation>(DataField.Length, m_Length, length);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteObservation<Direction>(editSerializer, DataField.Direction);
            data.WriteObservation<Observation>(editSerializer, DataField.Length);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadObservation<Direction>(editDeserializer, DataField.Direction);
            result.ReadObservation<Observation>(editDeserializer, DataField.Length);
            return result;
        }

        /// <summary>
        /// Exchanges update items that were previously generated via
        /// a call to <see cref="GetUpdateItems"/>.
        /// </summary>
        /// <param name="data">The update data to apply to the edit (modified to
        /// hold the values that were previously defined for the edit)</param>
        public override void ExchangeData(UpdateItemCollection data)
        {
            m_Direction = data.ExchangeObservation<Direction>(this, DataField.Direction, m_Direction);
            m_Length = data.ExchangeObservation<Observation>(this, DataField.Length, m_Length);

            if (m_Line != null)
                m_Line.ObservedLength = (m_Length as Distance);
        }

        /// <summary>
        /// The features that were created by this operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(2);

                if (m_To!=null)
                    result.Add(m_To);

                if (m_Line!=null)
                    result.Add(m_Line);

                return result.ToArray();
            }
        }

/*
//	@mfunc	Check whether this operation makes reference to
//			a specific feature.
//	@parm	The feature to check for.
LOGICAL CeRadial::HasReference ( const CeFeature* const pFeat ) const {

	if ( m_pDirection && m_pDirection->HasReference(pFeat) ) return TRUE;
	if ( m_pLength && m_pLength->HasReference(pFeat) ) return TRUE;

	return FALSE;

} // end of HasReference
        */

        /*
//	@mfunc	Draw observed angles recorded as part of this op.
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The view to draw in.
//	@parm	Display context for the draw.
//	@parm	The ground window for the draw (0 for no check).
void CeRadial::DrawAngles ( const CePoint* const pFrom
						  , CeView* view
						  , CDC* pDC
						  , const CeWindow* const pWin ) const {

	// Skip this op if it has an update.
	if ( GetpLatest() ) return;

	// Draw the direction.
	m_pDirection->DrawAngle(pFrom,view,pDC,pWin,m_pTo);

} // end of DrawAngles
         */

        /*
//	@mfunc	Draw observed angles recorded as part of this op.
//	@parm	The point the observation must be made from.
//			Specify 0 for all points.
//	@parm	The thing we're drawing to.
void CeRadial::DrawAngles ( const CePoint* const pFrom
						  , CeDC& gdc ) const {

	// Skip this op if it has an update.
	if ( GetpLatest() ) return;

	// Draw the direction.
	m_pDirection->DrawAngle(pFrom,gdc,m_pTo);

} // end of DrawAngles
         */

        /*
//	@mfunc	Create transient CeMiscText objects for any observed
//			angles that are part of this operation.
//
//			The caller is responsible for deleting the text
//			objects once done with them.
//
//	@parm	List of pointers to created text objects (appended to).
//	@parm	Should lines be produced too?
//	@parm	The associated point that this op must reference.
void CeRadial::CreateAngleText ( CPtrList& text
							   , const LOGICAL wantLinesToo
							   , const CePoint* const pFrom ) const {

	// Skip this op if it has an update.
	if ( GetpLatest() ) return;

	// Get the direction to do it.
	m_pDirection->CreateAngleText(text,wantLinesToo,pFrom,m_pTo);

} // end of CreateAngleText
         */

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();
            result.AddRange(m_Direction.GetReferences());
            result.AddRange(m_Length.GetReferences());
            return result.ToArray();
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            base.OnRollback();

            // Delete observations.
            m_Direction.OnRollback(this);
            m_Length.OnRollback(this);

            // Mark sideshot point for deletion
            Rollback(m_To);

            // If we created a line, mark it as well
            Rollback(m_Line);
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

            editSerializer.WritePersistent<Direction>(DataField.Direction, m_Direction);
            editSerializer.WritePersistent<Observation>(DataField.Length, m_Length);
            editSerializer.WritePersistent<FeatureStub>(DataField.To, new FeatureStub(m_To));

            if (m_Line != null)
                editSerializer.WritePersistent<FeatureStub>(DataField.Line, new FeatureStub(m_Line));
        }
    }
}
