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

using Backsight.Editor.Observations;
using System.Diagnostics;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="10-JUN-1999" was="CePointOnLine"/>
    /// <summary>
    /// Add a point at a specific distance from the start or end of an existing line,
    /// splitting the original line at the point.
    /// </summary>
    class SimpleLineSubdivisionOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        // Input ...

        /// <summary>
        /// The line the point sits on. This line gets de-activated.
        /// </summary>
        readonly LineFeature m_Line;

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
        /// Initializes a new instance of the <see cref="SimpleLineSubdivisionOperation"/> class
        /// </summary>
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="splitLine">The line to split.</param>
        /// <param name="dist">The distance to the split point (specify a negated distance
        /// if it's from the end of the line).</param>
        internal SimpleLineSubdivisionOperation(Session session, uint sequence, LineFeature splitLine, Distance dist)
            : base(session, sequence)
        {
            m_Line = splitLine;
            m_Distance = dist;
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
        /// The line created prior to the new point.
        /// </summary>
        internal LineFeature NewLine1
        {
            get { return m_NewLine1; }
            set { m_NewLine1 = value; }
        }

        /// <summary>
        /// The added point (between the 2 new lines)
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_NewPoint; }
            set { m_NewPoint = value; }
        }

        /// <summary>
        /// The line created after the point.
        /// </summary>
        internal LineFeature NewLine2
        {
            get { return m_NewLine2; }
            set { m_NewLine2 = value; }
        }

        /// <summary>
        /// The distance to the point. A negated distance refers to the distance from the end of the line.
        /// </summary>
        internal Distance Distance
        {
            get { return m_Distance; }
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
            return Calculate(m_Line, d, isFromEnd);
        }

        /// <summary>
        /// Calculates the positions of the split point.
        /// </summary>
        /// <param name="line">The line being subdivided.</param>
        /// <param name="dist">The distance to the split point.</param>
        /// <param name="isFromEnd">Is the distance from the end of the line?</param>
        /// <returns>The calculated position (null if the distance is longer than the line being subdivided,
        /// or supplied information is incomplete)</returns>
        internal static IPosition Calculate(LineFeature line, Distance dist, bool isFromEnd)
        {
            // Can't calculate if there is insufficient data.
            if (line == null || dist == null)
                return null;

            // The length must be defined.
            if (!dist.IsDefined)
                return null;

            // Return if the observed distance is longer than the total
            // length of the line.
            double maxlen = line.Length.Meters;
            double obsvlen = dist.Meters;
            if (obsvlen > maxlen)
                return null;

            // Get the approximate position of the split point.
            IPosition start, approx;
            LineGeometry g = line.LineGeometry;
            if (isFromEnd)
            {
                start = line.EndPoint;
                g.GetPosition(new Length(maxlen - obsvlen), out approx);
            }
            else
            {
                start = line.StartPoint;
                g.GetPosition(new Length(obsvlen), out approx);
            }

            // Get the distance to the approximate position on the mapping plane.
            ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;
            double planlen = dist.GetPlanarMetric(start, approx, sys);

            // Figure out the true position on the line.
            IPosition splitpos;
            if (isFromEnd)
                g.GetPosition(new Length(maxlen - planlen), out splitpos);
            else
                g.GetPosition(new Length(planlen), out splitpos);

            return splitpos;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        internal void Execute()
        {
            // Calculate the position of the point.
            IPosition splitpos = Calculate();
            if (splitpos==null)
                throw new Exception("Cannot calculate split position");

            FeatureFactory ff = new FeatureFactory(this);

            // See FeatureFactory.MakeSection - the only thing that really matters is the
            // session sequence number that will get picked up by the FeatureStub constructor.
            ff.AddFeatureDescription("NewLine1", new FeatureStub(this, m_Line.EntityType, null));
            ff.AddFeatureDescription("NewLine2", new FeatureStub(this, m_Line.EntityType, null));

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
            m_NewPoint = ff.CreatePointFeature("NewPoint");
            SectionLineFeature line1, line2;
            ff.MakeSections(m_Line, "NewLine1", m_NewPoint, "NewLine2", out line1, out line2);
            m_NewLine1 = line1;
            m_NewLine2 = line2;
        }

        /// <summary>
        /// Calculates the geometry for any spatial features that were created by
        /// this editing operation.
        /// </summary>
        internal override void CalculateGeometry()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_NewPoint.SetPointGeometry(pg);
        }

        /// <summary>
        /// Creates a section for this subdivision op.
        /// </summary>
        /// <param name="sessionSequence">The 1-based creation sequence of this feature within the
        /// session that created it.</param>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="end">The point at the end of the section</param>
        /// <returns>The created section</returns>
        internal LineFeature MakeSection(uint sessionSequence, PointFeature start, PointFeature end)
        {
            SectionGeometry section = new SectionGeometry(m_Line, start, end);
            LineFeature newLine = m_Line.MakeSubSection(this, sessionSequence, section);
            //MapModel.EditingIndex.Add(newLine);
            return newLine;
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit.
        /// </summary>
        /// <param name="dist">The new observed distance.</param>
        /// <param name="isFromEnd">Is the distance observed from the end of the line?</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItem[] GetUpdateItems(Distance dist, bool isFromEnd)
        {
            Distance d = new Distance(dist);

            // And make sure the sign is correct.
            if (isFromEnd)
                d.SetNegative();
            else
                d.SetPositive();

            return new UpdateItem[]
            {
                new UpdateItem("Distance", d)
            };
        }

        /// <summary>
        /// Modifies this edit by applying the values in the supplied update items.
        /// </summary>
        /// <param name="changes">The changes to apply (obtained via a prior call to
        /// <see cref="GetUpdateItems"/>)</param>
        /// <returns>The old values for each update item</returns>
        internal UpdateItem[] ExchangeUpdateItems(UpdateItem[] changes)
        {
            Debug.Assert(changes.Length == 1);
            Debug.Assert(changes[0].Name == "Distance");

            // Note the original values
            UpdateItem[] originalData = new UpdateItem[] { new UpdateItem("Distance", m_Distance) };

            // Apply the revised values
            m_Distance = (Distance)changes[0].Value;

            return originalData;
        }

        /*
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
        */

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
            get { return EditingActionId.SimpleLineSubdivision; }
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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the split point.
            Distance dist = new Distance(m_Distance);
            bool isFromEnd = dist.SetPositive();
            IPosition splitpos = Calculate(m_Line, dist, isFromEnd);
            if (splitpos==null)
                throw new RollforwardException(this, "Cannot re-calculate position of point on line.");

            // Move the split point.
            m_NewPoint.MovePoint(uc, splitpos);

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
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The line that was subdivided (if it resulted in the supplied line
        /// of interest), otherwise null.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            if (Object.ReferenceEquals(line, m_NewLine1) || Object.ReferenceEquals(line, m_NewLine2))
                return m_Line;
            else
                return null;
        }
    }
}
