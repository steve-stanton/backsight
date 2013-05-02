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
        /// The distance to the point.
        /// </summary>
        Distance m_Distance;

        /// <summary>
        /// Is the distance observed from the end of the line?
        /// </summary>
        bool m_IsFromEnd;

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
        /// <param name="splitLine">The line to split.</param>
        /// <param name="dist">The distance to the split point.</param>
        /// <param name="isFromEnd">Is the distance observed from the end of the line?</param>
        internal SimpleLineSubdivisionOperation(LineFeature splitLine, Distance dist, bool isFromEnd)
            : base()
        {
            m_Line = splitLine;
            m_Distance = dist;
            m_IsFromEnd = isFromEnd;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleLineSubdivisionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal SimpleLineSubdivisionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_Line = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line);
            m_Distance = editDeserializer.ReadPersistent<Distance>(DataField.Distance);
            m_IsFromEnd = editDeserializer.ReadBool(DataField.EntryFromEnd);
            FeatureStub newPoint = editDeserializer.ReadPersistent<FeatureStub>(DataField.NewPoint);
            string dataId1 = editDeserializer.ReadString(DataField.NewLine1);
            string dataId2 = editDeserializer.ReadString(DataField.NewLine2);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub(DataField.NewPoint, newPoint);
            dff.AddLineSplit(m_Line, DataField.NewLine1, dataId1);
            dff.AddLineSplit(m_Line, DataField.NewLine2, dataId2);
            ProcessFeatures(dff);
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
        /// The distance to the point.
        /// </summary>
        internal Distance Distance
        {
            get { return m_Distance; }
        }

        /// <summary>
        /// Is the distance observed from the end of the line?
        /// </summary>
        internal bool IsFromEnd
        {
            get { return m_IsFromEnd; }
        }

        /// <summary>
        /// Calculates the position of the point
        /// </summary>
        /// <returns>The calculated position (null if the distance is longer than the line being subdivided,
        /// or supplied information is incomplete)</returns>
        IPosition Calculate()
        {
            return Calculate(m_Line, m_Distance, m_IsFromEnd);
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
            ISpatialSystem sys = CadastralMapModel.Current.SpatialSystem;
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
            ff.AddFeatureDescription(DataField.NewLine1, new FeatureStub(this, m_Line.EntityType, null));
            ff.AddFeatureDescription(DataField.NewLine2, new FeatureStub(this, m_Line.EntityType, null));

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
            m_NewPoint = ff.CreatePointFeature(DataField.NewPoint);
            LineFeature line1, line2;
            ff.MakeSections(m_Line, DataField.NewLine1, m_NewPoint, DataField.NewLine2, out line1, out line2);
            m_NewLine1 = line1;
            m_NewLine2 = line2;

            if (m_IsFromEnd)
                m_NewLine2.ObservedLength = m_Distance;
            else
                m_NewLine1.ObservedLength = m_Distance;
        }

        /// <summary>
        /// Calculates the geometry for any spatial features that were created by
        /// this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_NewPoint.ApplyPointGeometry(ctx, pg);
        }

        /// <summary>
        /// Creates a section for this subdivision op.
        /// </summary>
        /// <param name="id">The internal of this feature within the project that created it.</param>
        /// <param name="start">The point at the start of the section</param>
        /// <param name="end">The point at the end of the section</param>
        /// <returns>The created section</returns>
        internal LineFeature MakeSection(InternalIdValue id, PointFeature start, PointFeature end)
        {
            SectionGeometry section = new SectionGeometry(m_Line, start, end);
            LineFeature newLine = m_Line.MakeSubSection(this, id, section);
            //MapModel.EditingIndex.Add(newLine);
            return newLine;
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="dist">The new observed distance.</param>
        /// <param name="isFromEnd">Is the distance observed from the end of the line?</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(Distance dist, bool isFromEnd)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddObservation<Distance>(DataField.Distance, m_Distance, dist);
            result.AddItem<bool>(DataField.EntryFromEnd, m_IsFromEnd, isFromEnd);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteObservation<Distance>(editSerializer, DataField.Distance);
            data.WriteItem<bool>(editSerializer, DataField.EntryFromEnd);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadObservation<Distance>(editDeserializer, DataField.Distance);
            result.ReadItem<bool>(editDeserializer, DataField.EntryFromEnd);
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
            m_Distance = data.ExchangeObservation<Distance>(this, DataField.Distance, m_Distance);
            m_IsFromEnd = data.ExchangeValue<bool>(DataField.EntryFromEnd, m_IsFromEnd);

            if (m_IsFromEnd)
                m_NewLine2.ObservedLength = m_Distance;
            else
                m_NewLine1.ObservedLength = m_Distance;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Line subdivide (one distance)"; }
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
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();
            result.Add(m_Line);
            result.AddRange(m_Distance.GetReferences());
            return result.ToArray();
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
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

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line, m_Line);
            editSerializer.WritePersistent<Distance>(DataField.Distance, m_Distance);
            editSerializer.WriteBool(DataField.EntryFromEnd, m_IsFromEnd);
            editSerializer.WritePersistent<FeatureStub>(DataField.NewPoint, new FeatureStub(m_NewPoint));
            editSerializer.WriteInternalId(DataField.NewLine1, m_NewLine1.InternalId);
            editSerializer.WriteInternalId(DataField.NewLine2, m_NewLine2.InternalId);
        }
    }
}
