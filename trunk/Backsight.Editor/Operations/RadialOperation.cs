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

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

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
        readonly Direction m_Direction;

        /// <summary>
        /// The length of the sideshot arm (either a <see cref="Distance"/> or
        /// an <see cref="OffsetPoint"/>).
        /// </summary>
        readonly Observation m_Length;

        // Creations ...

        /// <summary>
        /// The point at the end of the sideshot arm.
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The line (if any) that was added to correspond to the sideshot arm.
        /// </summary>
        SegmentLineFeature m_Line;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization.
        /// </summary>
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="dir">The direction (could contain an offset).</param>
        /// <param name="length">The length of the sideshot arm (either a <see cref="Distance"/> or
        /// an <see cref="OffsetPoint"/>).</param>
        internal RadialOperation(Session session, uint sequence, Direction dir, Observation length)
            : base(session, sequence)
        {
            m_Direction = dir;
            m_Length = length;
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
        internal SegmentLineFeature Line
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
            ff.AddFeatureDescription("To", x);

            if (lineType != null)
            {
                IFeature f = new FeatureStub(this, lineType, null);
                ff.AddFeatureDescription("Line", f);
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
            m_To = ff.CreatePointFeature("To");

            if (ff.HasFeatureDescription("Line"))
                m_Line = ff.CreateSegmentLineFeature("Line", m_Direction.From, m_To);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void CalculateGeometry()
        {
            IPosition p = Calculate(m_Direction, m_Length);
            PointGeometry pg = PointGeometry.Create(p);
            m_To.PointGeometry = pg;
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
            ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;
            double sfac = sys.GetLineScaleFactor(start, to);
            return Geom.Polar(start, bearing, length * sfac);
        }

        /// <summary>
        /// Corrects this operation. This just changes the info defining the op, and
        /// marks the operation as changed.
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="length"></param>
        /// <returns>True if changes are ok.</returns>
        internal bool Correct(Direction dir, Observation length)
        {
            throw new NotImplementedException();

            //// Confirm that the sideshot point can be re-calculated.
            //IPosition to = RadialUI.Calculate(dir, length);
            //if (to==null)
            //    throw new ArgumentException("Cannot update position of sideshot point.");

            //// Cut the references made by the direction object. If nothing
            //// has changed, the references will be re-inserted when the
            //// direction is re-saved below.
            //if (m_Direction!=null)
            //    m_Direction.OnRollback(this);

            //if (m_Length!=null)
            //    m_Length.OnRollback(this);

            //m_Direction = dir;
            //m_Direction.AddReferences(this);
            //m_Length = length;
            //m_Length.AddReferences(this);

            //return true;
        }

        /// <summary>
        /// Find the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            // If the length of the sideshot arm was specified as an
            // entered distance (as opposed to an offset point), return
            // a reference to it.

            if (Object.ReferenceEquals(line, m_Line))
                return (m_Length as Distance);
            else
                return null;
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

        public override void AddReferences()
        {
            m_Direction.AddReferences(this);
            m_Length.AddReferences(this);
        }

        internal override bool Undo()
        {
            base.OnRollback();

            // Delete observations.
            m_Direction.OnRollback(this);
            m_Length.OnRollback(this);

            // Mark sideshot point for deletion
            Rollback(m_To);

            // If we created a line, mark it as well
            Rollback(m_Line);

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

        	// Re-calculate the position of the sideshot point.
            IPosition to = Calculate(m_Direction, m_Length);
            if (to==null)
                throw new RollforwardException(this, "Cannot re-calculate position of sideshot point.");

        	// Move the sideshot point.
            m_To.MovePoint(uc, to);

            // Rollforward the base class.
            return base.OnRollforward();
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
    }
}
