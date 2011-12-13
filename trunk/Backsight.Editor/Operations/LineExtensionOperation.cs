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

using Backsight.Editor.Observations;
using Backsight.Editor.UI;
using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="06-DEC-1998" was="CeArcExtension" />
    /// <summary>
    /// Operation to extend a line.
    /// </summary>
    class LineExtensionOperation : Operation, IRecallable, IRevisable
    {
        #region Class data

        /// <summary>
        /// The line being extended.
        /// </summary>
        readonly LineFeature m_ExtendLine;

        /// <summary>
        /// True if extending from the end of <c>m_ExtendLine</c>.
        /// False if extending from the start.
        /// </summary>
        bool m_IsExtendFromEnd;

        /// <summary>
        /// The observed length of the extension.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// The actual extension line (if any).
        /// </summary>
        LineFeature m_NewLine;

        /// <summary>
        /// The point at the end of the extension.
        /// </summary>
        PointFeature m_NewPoint;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineExtensionOperation"/> class
        /// </summary>
        /// <param name="extendLine">The line that's being extended.</param>
        /// <param name="isFromEnd">True if extending from the end | False from the start.</param>
        /// <param name="length">The length of the extension.</param>
        internal LineExtensionOperation(LineFeature extendLine, bool isFromEnd, Distance length)
            : base()
        {
            m_ExtendLine = extendLine;
            m_IsExtendFromEnd = isFromEnd;
            m_Length = length;

            m_NewLine = null;
            m_NewPoint = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineExtensionOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal LineExtensionOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            FeatureStub newPoint, newLine;
            ReadData(editDeserializer, out m_ExtendLine, out m_IsExtendFromEnd, out m_Length, out newPoint, out newLine);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub(DataField.NewPoint, newPoint);
            dff.AddFeatureStub(DataField.NewLine, newLine);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// The line that was extended.
        /// </summary>
        internal LineFeature ExtendedLine
        {
            get { return m_ExtendLine; }
        }

        /// <summary>
        /// The actual extension line (if any).
        /// </summary>
        internal LineFeature NewLine
        {
            get { return m_NewLine; }
            set { m_NewLine = value; }
        }

        /// <summary>
        /// The point at the end of the extension.
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_NewPoint; }
            set { m_NewPoint = value; }
        }

        /// <summary>
        /// The observed length of the extension.
        /// </summary>
        internal Distance Length
        {
            get { return m_Length; }
        }

        /// <summary>
        /// Is the extension from the end of <see cref="ExtendLine"/>
        /// </summary>
        internal bool IsExtendFromEnd
        {
            get { return m_IsExtendFromEnd; }
        }

        /// <summary>
        /// Attempts to locate a superseded (inactive) line that was the parent of
        /// a specific line.
        /// </summary>
        /// <param name="line">The line of interest</param>
        /// <returns>The superseded line that the line of interest was derived from. Null if
        /// this edit did not create the line of interest.</returns>
        internal override LineFeature GetPredecessor(LineFeature line)
        {
            // This edit doesn't supersede anything
            return null;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="pointId">The ID (and entity type) for the extension point.</param>
        /// <param name="lineEnt">The entity type for the extension line (null for no line).</param>
        internal void Execute(IdHandle pointId, IEntity lineEnt)
        {
            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature xp = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription(DataField.NewPoint, xp);

            if (lineEnt != null)
            {
                IFeature f = new FeatureStub(this, lineEnt, null);
                ff.AddFeatureDescription(DataField.NewLine, f);
            }

            base.Execute(ff);

            /*
            IPosition start;    // Start of the extension
            IPosition end;      // End of the extension

            // See if the extension is a straight line.
            bool isStraight = LineExtensionUI.Calculate(m_ExtendLine, m_IsExtendFromEnd, m_Length, out start, out end);

            // If it's not straight, it should be a circular arc.
            bool isCurve = false;
            IPosition center;   // The centre of the circle
            bool iscw = true;   // Is the curve clockwise?

            if (!isStraight)
                isCurve = LineExtensionUI.Calculate(m_ExtendLine, m_IsExtendFromEnd, m_Length, out start, out end, out center, out iscw);

            // Return if it's neither straight or a circular arc.
        	if ( !(isStraight || isCurve) )
                throw new Exception("Cannot calculate line extension point.");

            // Add the extension point to the map.
            CadastralMapModel map = MapModel;
            m_NewPoint = map.AddPoint(end, pointId.Entity, this);

            // Associate the new point with the specified ID (if any).
            pointId.CreateId(m_NewPoint);

            // If a line entity has been supplied, add a line too.
            if (lineEnt==null)
                m_NewLine = null;
            else
            {
                // Get the point at the end of the extension line
                PointFeature s = (m_IsExtendFromEnd ? m_ExtendLine.EndPoint : m_ExtendLine.StartPoint);

                if (isStraight)
                    m_NewLine = map.AddLine(s, m_NewPoint, lineEnt, this);
                else
                {
                    // We need the circle that the arc lies on.
                    Circle circle = m_ExtendLine.Circle;
                    Debug.Assert(circle!=null);

                    // Add the arc to the map.
                    m_NewLine = map.AddCircularArc(circle, s, m_NewPoint, iscw, lineEnt, this);
                }
            }

            // Peform standard completion steps
            Complete();
             */
        }

        /// <summary>
        /// Creates any new spatial features (without any geometry)
        /// </summary>
        /// <param name="ff">The factory class for generating spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            m_NewPoint = ff.CreatePointFeature(DataField.NewPoint);

            if (ff.HasFeatureDescription(DataField.NewLine))
            {
                PointFeature from = (m_IsExtendFromEnd ? m_ExtendLine.EndPoint : m_ExtendLine.StartPoint);
                ArcFeature arc = m_ExtendLine.GetArcBase();

                if (arc == null)
                    m_NewLine = ff.CreateSegmentLineFeature(DataField.NewLine, from, m_NewPoint);
                else
                    m_NewLine = ff.CreateArcFeature(DataField.NewLine, from, m_NewPoint);

                m_NewLine.ObservedLength = m_Length;
            }
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_NewPoint.ApplyPointGeometry(ctx, pg);

            // If the extension line was a circular arc, we also need to define it's geometry.
            // This COULD have been defined at an earlier stage (e.g. as part of CreateFeature),
            // but it's more consistent to do it as part of this method.

            if (m_NewLine is ArcFeature)
            {
                ArcFeature arc = m_ExtendLine.GetArcBase();
                Circle circle = arc.Circle;
                Debug.Assert(circle != null);

                bool iscw = arc.IsClockwise;
                if (!m_IsExtendFromEnd)
                    iscw = !iscw;

                ArcGeometry geom = new ArcGeometry(circle, m_NewLine.StartPoint, m_NewLine.EndPoint, iscw);
                (m_NewLine as ArcFeature).Geometry = geom;
            }
        }

        /// <summary>
        /// Calculates the position of the extension point.
        /// </summary>
        /// <returns>The calculated position</returns>
        IPosition Calculate()
        {
            // Figure out the new position for the extension point, depending
            // on whether the line we extended is a circular arc or a straight.

            IPosition start;		// Start of the extension
            IPosition end;			// End of the extension
            bool ok;				// Did calculation work ok?

            if (m_ExtendLine is ArcFeature)
            {
                IPosition center;	// The center of the circle
                bool iscw;			// Is the curve clockwise?

                ok = LineExtensionUI.Calculate(m_ExtendLine, m_IsExtendFromEnd, m_Length,
                            out start, out end, out center, out iscw);
            }
            else
            {
                ok = LineExtensionUI.Calculate(m_ExtendLine, m_IsExtendFromEnd, m_Length,
                            out start, out end);
            }

            return (ok ? end : null);
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="isFromEnd">True if extending from the end of the line</param>
        /// <param name="length">The observed length of the extension</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(bool isFromEnd, Distance length)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddItem<bool>(DataField.ExtendFromEnd, m_IsExtendFromEnd, isFromEnd);
            result.AddObservation<Distance>(DataField.Distance, m_Length, length);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteItem<bool>(editSerializer, DataField.ExtendFromEnd);
            data.WriteObservation<Distance>(editSerializer, DataField.Distance);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadItem<bool>(editDeserializer, DataField.ExtendFromEnd);
            result.ReadObservation<Distance>(editDeserializer, DataField.Distance);
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
            m_IsExtendFromEnd = data.ExchangeValue<bool>(DataField.ExtendFromEnd, m_IsExtendFromEnd);
            m_Length = data.ExchangeObservation<Distance>(this, DataField.Distance, m_Length);
            m_NewLine.ObservedLength = m_Length;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Line extension"; }
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(2);

                if (m_NewPoint!=null)
                    result.Add(m_NewPoint);

                if (m_NewLine!=null)
                    result.Add(m_NewLine);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.LineExtend; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();
            result.Add(m_ExtendLine);
            result.AddRange(m_Length.GetReferences());
            return result.ToArray();
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

	        // Cut the reference to this op from the line that we extended.
            m_ExtendLine.CutOp(this);

	        // Undo the extension point and any extension line
            Rollback(m_NewPoint);
            Rollback(m_NewLine);

        	return true;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            if (Object.ReferenceEquals(m_ExtendLine, feat))
                return true;

            if (m_Length.HasReference(feat))
                return true;

            return false;
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line, m_ExtendLine);
            editSerializer.WriteBool(DataField.ExtendFromEnd, m_IsExtendFromEnd);
            editSerializer.WritePersistent<Distance>(DataField.Distance, m_Length);
            editSerializer.WritePersistent<FeatureStub>(DataField.NewPoint, new FeatureStub(m_NewPoint));

            if (m_NewLine != null)
                editSerializer.WritePersistent<FeatureStub>(DataField.NewLine, new FeatureStub(m_NewLine));
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="extendLine">The line being extended.</param>
        /// <param name="isExtendFromEnd">True if extending from the end of the extension line. False if extending from the start.</param>
        /// <param name="length">The observed length of the extension.</param>
        /// <param name="newPoint">The point at the end of the extension.</param>
        /// <param name="newLine">The actual extension line (if any).</param>
        static void ReadData(EditDeserializer editDeserializer, out LineFeature extendLine, out bool isExtendFromEnd,
                                out Distance length, out FeatureStub newPoint, out FeatureStub newLine)
        {
            extendLine = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line);
            isExtendFromEnd = editDeserializer.ReadBool(DataField.ExtendFromEnd);
            length = editDeserializer.ReadPersistent<Distance>(DataField.Distance);
            newPoint = editDeserializer.ReadPersistent<FeatureStub>(DataField.NewPoint);
            newLine = editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.NewLine);
        }
    }
}
