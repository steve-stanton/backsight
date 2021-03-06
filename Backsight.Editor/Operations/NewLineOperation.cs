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
        /// Initializes a new instance of the <see cref="NewLineOperation"/> class.
        /// </summary>
        protected NewLineOperation()
            : base()
        {
            m_NewLine = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewLineOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        protected NewLineOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            // Bit of a hack - does NewCircleOperation actually need to extend NewLineOperation?
            if (!(this is NewCircleOperation) && !(this is NewArcOperation))
                m_NewLine = editDeserializer.ReadPersistent<LineFeature>(DataField.Line);
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
        /// Records the new line for this operation.
        /// </summary>
        /// <param name="line">The line created by this operation.</param>
        internal void SetNewLine(LineFeature line)
        {
            m_NewLine = line;
        }

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

        internal override Feature[] Features
        {
            get { return new Feature[] { m_NewLine }; }
        }

        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewLine; }
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            if (m_NewLine==null)
                throw new Exception("NewLineOperation.Rollback - No line to rollback.");

            // Rollback any sub-operations.
            base.OnRollback();

            // Mark the line as deleted.
            Rollback(m_NewLine);
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
            */
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
            //return base.OnRollforward();
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
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>
        /// The referenced features (never null, but may be an empty array).
        /// </returns>
        public override Feature[] GetRequiredFeatures()
        {
            return new Feature[] { m_NewLine.StartPoint, m_NewLine.EndPoint };
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            // Bit of a hack - does NewCircleOperation actually need to extend NewLineOperation?
            if (!(this is NewCircleOperation))
                editSerializer.WritePersistent<LineFeature>(DataField.Line, m_NewLine);
        }
    }
}
