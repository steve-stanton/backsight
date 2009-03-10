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
    class LineExtensionOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The line being extended.
        /// </summary>
        LineFeature m_ExtendLine;

        /// <summary>
        /// The actual extension line (if any).
        /// </summary>
        LineFeature m_NewLine;

        /// <summary>
        /// The point at the end of the extension.
        /// </summary>
        PointFeature m_NewPoint;

        /// <summary>
        /// The observed length of the extension.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// True if extending from the end of <c>m_ExtendLine</c>.
        /// False if extending from the start.
        /// </summary>
        bool m_IsExtendFromEnd;        

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor, for use during deserialization
        /// </summary>
        public LineExtensionOperation()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LineExtensionOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal LineExtensionOperation(Session s)
            : base(s)
        {
            m_ExtendLine = null;
            m_NewLine = null;
            m_NewPoint = null;
            m_Length = null;
            m_IsExtendFromEnd = true;
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
        }

        /// <summary>
        /// The point at the end of the extension.
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_NewPoint; }
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
        /// Return true, to indicate that this edit can be corrected.
        /// </summary>
        internal bool CanCorrect
        {
            get { return true; }
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
        /// <param name="extendLine">The line that's being extended.</param>
        /// <param name="isFromEnd">True if extending from the end | False from the start.</param>
        /// <param name="length">The length of the extension.</param>
        /// <param name="pointId">The ID (and entity type) for the extension point.</param>
        /// <param name="lineEnt">The entity type for the extension line (null for no line).</param>
        internal void Execute(LineFeature extendLine, bool isFromEnd, Distance length, IdHandle pointId, IEntity lineEnt)
        {
            IPosition start;    // Start of the extension
            IPosition end;      // End of the extension

            // See if the extension is a straight line.
            bool isStraight = LineExtensionUI.Calculate(extendLine, isFromEnd, length, out start, out end);

            // If it's not straight, it should be a circular arc.
            bool isCurve = false;
            IPosition center;   // The centre of the circle
            bool iscw = true;   // Is the curve clockwise?

            if (!isStraight)
                isCurve = LineExtensionUI.Calculate(extendLine, isFromEnd, length, out start, out end, out center, out iscw);

            // Return if it's neither straight or a circular arc.
        	if ( !(isStraight || isCurve) )
                throw new Exception("Cannot calculate line extension point.");

            // Remember the line that's being extended, and which end.
            m_ExtendLine = extendLine;
            m_IsExtendFromEnd = isFromEnd;

            // Save the distance observation.
            m_Length = length;

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
                PointFeature s = (isFromEnd ? extendLine.EndPoint : extendLine.StartPoint);

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
        }

        /// <summary>
        /// Corrects this operation.
        /// </summary>
        /// <param name="isFromEnd">True if extending from the end of the line</param>
        /// <param name="length">The observed length of the extension</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct(bool isFromEnd, Distance length)
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
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

        	// Re-calculate the position of the extension point.
            IPosition xpos = Calculate();

	        if (xpos==null)
                throw new RollforwardException(this, "Cannot re-calculate line extension point.");

	        // Move the extension point.
	        m_NewPoint.Move(xpos);

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
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);

            writer.WriteFeatureReference("ExtendLine", m_ExtendLine);
            writer.WriteBool("IsExtendFromEnd", m_IsExtendFromEnd);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            writer.WriteElement("Distance", m_Length);
            writer.WriteCalculatedPoint("NewPoint", m_NewPoint);
            writer.WriteElement("NewLine", m_NewLine);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);

            m_ExtendLine = reader.ReadFeatureByReference<LineFeature>("ExtendLine");
            m_IsExtendFromEnd = reader.ReadBool("IsExtendFromEnd");
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);

            m_Length = reader.ReadElement<Distance>("Distance");
            //m_NewPoint = reader.ReadCalculatedPoint("NewPoint", Calculate());
            m_NewPoint = reader.ReadPoint("NewPoint");
            m_NewLine = reader.ReadElement<LineFeature>("NewLine");
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_NewPoint.PointGeometry = pg;
        }
    }
}
