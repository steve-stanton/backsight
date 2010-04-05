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
using Backsight.Editor.Xml;


namespace Backsight.Editor.Operations
{
	/// <written by="Steve Stanton" on="04-JAN-1998" />
    /// <summary>
    /// Operation to add a new line (either a simple line segment, or a circular arc).
    /// </summary>
    abstract class NewLineOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The created line
        /// </summary>
        LineFeature m_NewLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        protected NewLineOperation(Session s, NewSegmentData t)
            : base(s, t)
        {
            m_NewLine = new LineFeature(this, t.Line);
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        protected NewLineOperation(Session s, NewArcData t)
            : base(s, t)
        {
            m_NewLine = new ArcFeature(this, t.Line);
        }

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        protected NewLineOperation(Session s, NewCircleData t)
            : base(s, t)
        {
            m_NewLine = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewLineOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal NewLineOperation(Session s)
            : base(s)
        {
            m_NewLine = null;
        }

        #endregion

        /// <summary>
        /// The created line
        /// </summary>
        internal LineFeature Line
        {
            get { return m_NewLine; }
        }

        /// <summary>
        /// Records the new line for this operation (used by the <c>NewCircleOperation</c>).
        /// </summary>
        /// <param name="line">The line created by this operation.</param>
        protected void SetNewLine(LineFeature line)
        {
            m_NewLine = line;
        }

        //virtual CeArc*			GetpPredecessor	( const CeArc& arc ) const { return 0; }
        //virtual LOGICAL			HasReference	( const CeFeature* const pFeat ) const { return FALSE; }


        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get
            {
                if (m_NewLine is ArcFeature)
                    return "Add circular arc";
                else
                    return "Add straight line";
            }
        }

        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        internal override Feature[] Features
        {
            get { return new Feature[] { m_NewLine }; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewLine; }
        }

        public override void AddReferences()
        {
            // nothing to do
        }

        /// <summary>
        /// Rollback this operation.
        /// </summary>
        /// <returns></returns>
        internal override bool Undo()
        {
            if (m_NewLine==null)
                throw new Exception("NewLineOperation.Rollback - No line to rollback.");

            // Rollback any sub-operations.
            base.OnRollback();

            // Mark the line as deleted.
            Rollback(m_NewLine);
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

            // SS:03-JUL-07 -- is the following still relevant??

            // If the line is a circular arc, ensure that the circle
            // it supposedly sits on is still consistent with the
            // end points ... consider the following scenario:
            //
            // 1. Create a circle construction line, with the radius
            //	  defined by point R (centre C).
            // 2. Use Intersect - Two Lines to create a point S on
            //    the construction line.
            // 3. Add an arc from R to S
            // 4. Update the construction line so that it now has a
            //    radius defined by point T.
            //
            // At this stage, point S will shift to S' (since the
            // circle intersects the line somewhere else). But R is
            // still where it was, and has a different radius =>
            // the arc is no longer valid! The best we could hope
            // to do would be to alter the arc to go from S' to T.
            // But the original offset point never moved, and it's
            // difficult to know at this stage where it was, and
            // where it is now.

            // For now, I'll handle this in CeNewCircle::Rollforward,
            // even though it could be dangerous shifting the arcs
            // before their time.

            /*
	            CeLine* pLine = m_pNewArc->GetpLine();
	            if ( pLine->GetType() == PTY_CURVE ) {

		            CeVertex centre;
		            FLOAT8 radius;
		            LOGICAL iscw;

		            pLine->GetCurveInfo(centre,radius,iscw);

		            CeLocation* pBC = pLine->GetpStart();
		            CeLocation* pEC = pLine->GetpEnd();

		            FLOAT8 dbc = sqrt(pBC->DistanceSquared(centre)) - radius;
		            FLOAT8 dec = sqrt(pEC->DistanceSquared(centre)) - radius;

		            if ( fabs(dbc)>XYTOL || fabs(dec)>XYTOL ) {
			            CString dmsg;
			            dmsg.Format("BC=%lf  EC=%lf",dbc,dec);
			            AfxMessageBox(dmsg);
		            }
	            }
            */

            // Nothing to do for straight lines!

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Gets any circles that were used to establish the position
        /// of a feature that was created by this operation.
        /// </summary>
        /// <param name="clist">The list to append any circles to.</param>
        /// <param name="point">One of the point features created by this op (either
        /// explicitly referred to, or added as a consequence of creating a new line).
        /// NOT USED.</param>
        /// <returns>True if request was handled (does not necessarily mean
        /// that any circles were found). False if this is a do-nothing function.</returns>
        bool GetCircles(List<Circle> clist, PointFeature point)
        {
            // If the line we created is a circular arc, append the
            // circle on which it is based.
            if (m_NewLine is ArcFeature)
                clist.Add((m_NewLine as ArcFeature).Circle);

            return true;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            // Nothing to do
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
