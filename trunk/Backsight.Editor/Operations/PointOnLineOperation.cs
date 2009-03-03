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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="10-JUN-1999" />
    /// <summary>
    /// Add a point at a specific distance from the start or end of an existing line,
    /// splitting the original line at the point.
    /// </summary>
    class PointOnLineOperation : Operation
    {
        #region Class data

        // Input ...

        /// <summary>
        /// The line the point sits on. This line gets de-activated.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The distance to the point. A negated distance refers to the distance from the end of the line.
        /// </summary>
        Distance m_Distance;
        
        // Creations ...

        /// <summary>
        /// The line created prior to the new point.
        /// </summary>
        LineFeature m_NewLine1;

        /// <summary>
        /// The added point (between the 2 new lines)
        /// </summary>
        PointFeature m_NewPoint;

        /// <summary>
        /// The line created after the point.
        /// </summary>
        LineFeature m_NewLine2;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PointOnLineOperation</c> with everything set to null.
        /// </summary>
        public PointOnLineOperation()
        {
            m_Line = null;
            m_Distance = null;
            m_NewLine1 = null;
            m_NewPoint = null;
            m_NewLine2 = null;
        }

        #endregion

        /// <summary>
        /// The line the point sits on. This line gets de-activated.
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The added point (between the 2 new lines)
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_NewPoint; }
        }

        /// <summary>
        /// The distance to the point. A negated distance refers to the distance from the end of the line.
        /// </summary>
        internal Distance Distance
        {
            get { return m_Distance; }
        }

        bool CanCorrect
        {
            get { return true; }
        }

        /// <summary>
        /// Calculates the position of the point
        /// </summary>
        /// <returns>The calculated position (null if the distance is longer than the line being subdivided,
        /// or supplied information is incomplete)</returns>
        IPosition Calculate()
        {
            Distance d = new Distance(m_Distance);
            bool isFromEnd = d.SetPositive();
            return PointOnLineUI.Calculate(m_Line, d, isFromEnd);
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="splitLine">The line to split.</param>
        /// <param name="dist">The distance to the split point.</param>
        /// <param name="isFromEnd">Is the distance observed from the end of the line?</param>
        internal void Execute(LineFeature splitLine, Distance dist, bool isFromEnd)
        {
            // Calculate the position of the point.
            IPosition splitpos = PointOnLineUI.Calculate(splitLine, dist, isFromEnd);
            if (splitpos==null)
                throw new Exception("Cannot calculate split position");

            // Remember the line that got split.
            m_Line = splitLine;

            // Save the observed distance (negated if it's from the end of the line).
            m_Distance = new Distance(dist);
            if (isFromEnd)
                m_Distance.SetNegative();

            // Add the split location (with no ID and default entity type).
            CadastralMapModel map = MapModel;
            m_NewPoint = map.AddPoint(splitpos, map.DefaultPointType, this);

            // Assign the new point the next available ID.
            m_NewPoint.SetNextId();

            // Create two line sections (one of them will be associated with the distance)
            m_NewLine1 = MakeSection(m_Line.StartPoint, m_NewPoint);
            m_NewLine2 = MakeSection(m_NewPoint, m_Line.EndPoint);

            // De-activate the parent line
            m_Line.IsInactive = true;

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Creates a section for this arc subdivision op.
        /// </summary>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="end">The point at the end of the section</param>
        /// <returns>The created section</returns>
        LineFeature MakeSection(PointFeature start, PointFeature end)
        {
            SectionGeometry section = new SectionGeometry(m_Line, start, end);
            LineFeature newLine = m_Line.MakeSubSection(section, this);
            //MapModel.EditingIndex.Add(newLine);
            return newLine;
        }

        /// <summary>
        /// Corrects this operation. This just changes the info defining the op, but does not
        /// attempt to re-execute it. This is used (I think) if a problem needs to be corrected
        /// as part of rollforward processing.
        /// </summary>
        /// <param name="dist">The new observed distance.</param>
        /// <param name="isFromEnd">Is the distance observed from the end of the line?</param>
        /// <returns>True if changes made ok (always true).</returns>
        internal bool Correct(Distance dist, bool isFromEnd)
        {
            // Change the distance.
            m_Distance = new Distance(dist);

            // And make sure the sign is correct.
            if (isFromEnd)
                m_Distance.SetNegative();
            else
                m_Distance.SetPositive();

            return true;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Line subdivide (one distance)"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            // Return the distance if the specified line is the one associated
            // with the observed distance.

            if (m_Distance.Meters < 0.0)
            {
                // Distance relates to the second line that was created.
                if (Object.ReferenceEquals(line, m_NewLine2))
                    return m_Distance;
            }
            else
            {
                // Distance relates to the first line that was created.
                if (Object.ReferenceEquals(line, m_NewLine1))
                    return m_Distance;
            }

            return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[] { m_NewLine1, m_NewPoint, m_NewLine2 }; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.PointOnLine; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_Line.AddReference(this);
            m_Distance.AddReferences(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Get rid of observed distance.
            m_Distance.OnRollback(this);
            m_Distance = null;

            // Mark created features for undo
            Rollback(m_NewLine1);
            Rollback(m_NewLine2);
            Rollback(m_NewPoint);

            // Remove the reference that the subdivided line makes to this edit, and restore it
            m_Line.CutOp(this);
            m_Line.Restore();

            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the split point.
            Distance dist = new Distance(m_Distance);
            bool isFromEnd = dist.SetPositive();
            IPosition splitpos = PointOnLineUI.Calculate(m_Line, dist, isFromEnd);
            if (splitpos==null)
                throw new RollforwardException(this, "Cannot re-calculate position of point on line.");

            // Move the split point.
            m_NewPoint.Move(splitpos);

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /*
//	@mfunc	Check whether this operation makes reference to
//			a specific feature.
//
//	@parm	The feature to check for.
LOGICAL CePointOnLine::HasReference ( const CeFeature* const pFeat ) const {

	return (m_pArc==pFeat);
         */

        /*
//	@mfunc	Return the arc (if any) that is the predecessor of
//			another arc (if it was created by this op).
//
//	@parm	The arc for which we want a predecessor for. Should
//			not be the creation of a split (to avoid this, ensure
//			that a call to <mf CeArc::GetUserArc> has been made).
//
//	@rdesc	The predecessor arc (if any).

CeArc* CePointOnLine::GetpPredecessor ( const CeArc& arc ) const {

	if ( &arc == m_pNewArc1 || &arc == m_pNewArc2 )
		return m_pArc;
	else
		return 0;
         */

        /*
//	@mfunc	Get any circles that were used to establish the position
//			of a point that was created by this operation.
//
//	@parm	The list to append any circles to.
//	@parm	One of the point features created by this op (either
//			explicitly referred to, or added as a consequence of
//			creating a new line).
//
//	@rdesc	TRUE if request was handled (does not necessarily mean
//			that any circles were found). FALSE if this is a
//			do-nothing function.

#include "CeCircle.h"

LOGICAL CePointOnLine::GetCircles ( CeObjectList& clist
								  , const CePoint& point ) const {

	// If the arc we subdivided was a circular arc, append the
	// circle on which it falls.
	CeCircle* pCircle = m_pArc->GetpCircle();
	if ( pCircle ) clist.AddReference(pCircle);

	return TRUE;
         */

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteFeatureReference("Line", m_Line);

            // The lines are implied, taking the entity type of the parent line. However,
            // we need to know the item numbers that were assigned to them.
            writer.WriteUnsignedInt("NewLine1", m_NewLine1.CreatorSequence);
            writer.WriteUnsignedInt("NewLine2", m_NewLine2.CreatorSequence);

            writer.WriteElement("Distance", m_Distance);
            writer.WriteCalculatedPoint("NewPoint", m_NewPoint);
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);

            m_Line = reader.ReadFeatureByReference<LineFeature>("Line");
            uint line1 = reader.ReadUnsignedInt("NewLine1");
            uint line2 = reader.ReadUnsignedInt("NewLine2");
            m_Distance = reader.ReadElement<Distance>("Distance");

            IPosition p = Calculate();
            m_NewPoint = reader.ReadCalculatedPoint("NewPoint", p);
            m_NewLine1 = MakeSection(m_Line.StartPoint, m_NewPoint);
            m_NewLine1.CreatorSequence = line1;
            m_NewLine2 = MakeSection(m_NewPoint, m_Line.EndPoint);
            m_NewLine2.CreatorSequence = line2;

            // Register the new lines with the reader (in case subsequent edits
            // have references to them)
            reader.AddFeature(m_NewLine1);
            reader.AddFeature(m_NewLine2);

            m_Line.Deactivate();
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
