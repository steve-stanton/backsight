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
using System.Windows.Forms;
using System.Diagnostics;

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.UI;

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
        readonly bool m_IsExtendFromEnd;

        /// <summary>
        /// The observed length of the extension.
        /// </summary>
        readonly Distance m_Length;

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
        /// <param name="s">The session the operation should be referred to (the session itself
        /// is not modified until the editing operation is saved to the database).</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="extendLine">The line that's being extended.</param>
        /// <param name="isFromEnd">True if extending from the end | False from the start.</param>
        /// <param name="length">The length of the extension.</param>
        internal LineExtensionOperation(Session session, uint sequence,
                                        LineFeature extendLine, bool isFromEnd, Distance length)
            : base(session, sequence)
        {
            m_ExtendLine = extendLine;
            m_IsExtendFromEnd = isFromEnd;
            m_Length = length;

            m_NewLine = null;
            m_NewPoint = null;
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
            ff.AddFeatureDescription("NewPoint", xp);

            if (lineEnt != null)
            {
                IFeature f = new FeatureStub(this, lineEnt, null);
                ff.AddFeatureDescription("NewLine", f);
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
            m_NewPoint = ff.CreatePointFeature("NewPoint");

            if (ff.HasFeatureDescription("NewLine"))
            {
                PointFeature from = (m_IsExtendFromEnd ? m_ExtendLine.EndPoint : m_ExtendLine.StartPoint);
                ArcFeature arc = m_ExtendLine.GetArcBase();

                if (arc == null)
                    m_NewLine = ff.CreateSegmentLineFeature("NewLine", from, m_NewPoint);
                else
                    m_NewLine = ff.CreateArcFeature("NewLine", from, m_NewPoint);
            }
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void CalculateGeometry()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_NewPoint.SetPointGeometry(pg);

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
        /// Applies an update to this edit (by modifying the parameters that describe
        /// the edit). To re-execute the edit and propogate any positional changes, you
        /// must make a subsequent call to <see cref="CadastralMapModel.Rollforward"/>.
        /// </summary>
        /// <param name="ut">Information about the update (expected to have a type that is
        /// consistent with this editing operation)</param>
        /// <returns>The parameters this editing operation originally had (before the
        /// supplied information was applied). Holding on to this information makes it
        /// possible to later revert things to the way they were originally.</returns>
        //public override UpdateData ApplyUpdate(UpdateData ut)
        //{
        //    LineExtensionUpdateData current = new LineExtensionUpdateData();
        //    current.ExtendFromEnd = m_IsExtendFromEnd;
        //    current.Distance = new DistanceData(m_Length);

        //    LineExtensionUpdateData u = (LineExtensionUpdateData)ut;
        //    m_IsExtendFromEnd = u.ExtendFromEnd;
        //    m_Length = (Distance)u.Distance.LoadObservation(this);

        //    return current;
        //}

        /// <summary>
        /// Corrects this operation.
        /// </summary>
        /// <param name="isFromEnd">True if extending from the end of the line</param>
        /// <param name="length">The observed length of the extension</param>
        /// <returns>True if operation updated ok.</returns>
        /*
        bool Correct(bool isFromEnd, Distance length)
        {
            // TODO: This is a bit awkward. Should have a Calculate method that accepts
            // parameters. Should also avoid MessageBox.Show (throw an exception instead).

	        // Remember the original values.
	        bool oldend = m_IsExtendFromEnd;
	        Distance oldlen = new Distance(m_Length);
            bool isOk = false;

            try
            {
                // Assign the new values.
                m_IsExtendFromEnd = isFromEnd;
                m_Length = length;

                // Confirm that the extension point can be re-calculated
                IPosition xpos = Calculate();
                isOk = (xpos!=null);
            }

            finally
            {
                // If the new extension point could not be calculated, restore original values.
                if (!isOk)
                {
                    MessageBox.Show("Cannot re-calculate line extension point.");
                    m_IsExtendFromEnd = oldend;
                    m_Length = oldlen;
                }
            }

            return isOk;
        }
        */

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Line extension"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            if (Object.ReferenceEquals(line, m_NewLine))
                return m_Length;
            else
                return null;
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
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_ExtendLine.AddOp(this);
            m_Length.AddReferences(this);
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

        	// Re-calculate the position of the extension point.
            IPosition xpos = Calculate();

	        if (xpos==null)
                throw new RollforwardException(this, "Cannot re-calculate line extension point.");

	        // Move the extension point.
            m_NewPoint.MovePoint(uc, xpos);

            // Rollforward the base class.
            return base.OnRollforward();
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
    }
}
