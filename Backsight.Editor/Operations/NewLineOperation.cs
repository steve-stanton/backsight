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
    class NewLineOperation : Operation
    {
        #region Class data

        /// <summary>
        /// The created line
        /// </summary>
        LineFeature m_NewLine;

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creates a <c>NewLineOperation</c> that doesn't refer
        /// to any new line.
        /// </summary>
        public NewLineOperation()
        {
            m_NewLine = null;
        }

        #endregion

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
        /// Creates a new simple line segment.
        /// </summary>
        /// <param name="start">The point at the start of the new line.</param>
        /// <param name="end">The point at the end of the new line.</param>
        /// <returns>True if new line added ok.</returns>
        internal bool Execute(PointFeature start, PointFeature end)
        {
            // Disallow an attempt to add a null line.
            if (start.Geometry.IsCoincident(end.Geometry))
                throw new Exception("NewLineOperation.Execute - Attempt to add null line.");

            // Add the new line with default entity type.
            CadastralMapModel map = CadastralMapModel.Current;
            m_NewLine = map.AddLine(start, end, map.DefaultLineType, this);

            // Peform standard completion steps
            Complete();
            return true;
        }

        /// <summary>
        /// Creates a new circular arc.
        /// </summary>
        /// <param name="start">The point at the start of the new arc.</param>
        /// <param name="end">The point at the end of the new arc.</param>
        /// <param name="circle">The circle that the new arc should sit on.</param>
        /// <param name="isShortArc">True if the new arc refers to the short arc. False
        /// if it's a long arc (i.e. greater than half the circumference of the circle).</param>
        /// <returns>True if new arc added ok.</returns>
        internal bool Execute(PointFeature start, PointFeature end, Circle circle, bool isShortArc)
        {
            // Disallow an attempt to add a null line.
            if (start.Geometry.IsCoincident(end.Geometry))
                throw new Exception("NewLineOperation.Execute - Attempt to add null line.");

            // Figure out whether the arc should go clockwise or not.
            IPointGeometry centre = circle.Center;

            // Get the clockwise angle from the start to the end.
            Turn sturn = new Turn(centre, start);
            double angle = sturn.GetAngleInRadians(end);

            // Figure out which direction the curve should go, depending
            // on whether the user wants the short arc or the long one.
            bool iscw;
            if (angle < Constants.PI)
                iscw = isShortArc;
            else
                iscw = !isShortArc;

            // Add the new arc with default line entity type (this will
            // cross-reference the circle to the arc that gets created).
            CadastralMapModel map = CadastralMapModel.Current;
            m_NewLine = map.AddCircularArc(circle, start, end, iscw, map.DefaultLineType, this);

            // Peform standard completion steps
            Complete();
            return true;
        }

        /// <summary>
        /// Rollforward this operation.
        /// </summary>
        /// <returns>True on success.</returns>
        internal override bool Rollforward()
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
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);

            // TODO: Is this right? - should writing the geometry for the new line
            // lead to the center and radius info when dealing with a NewCircleOperation?

            /*
            if (this is NewCircleOperation)
                writer.WriteElement("NewLine", new FeatureData(m_NewLine));
            else
                writer.WriteElement("NewLine", m_NewLine);
             */
            writer.WriteElement("NewLine", m_NewLine);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);
            m_NewLine = reader.ReadElement<LineFeature>("NewLine");
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            // TODO - Nothing to do for now, since LineFeature.ReadChildElements ends up
            // reading in the geometry. This may well need to change (see comments
            // in NewCircleOperation.CalculateGeometry)
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
