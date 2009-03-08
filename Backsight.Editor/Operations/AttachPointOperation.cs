// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
//
// Backsight is free software; you can redistribute it and/or modify it under the terms
// of the GNU Lesser General Public License as published by the Free Software Foundation;
// either version 3 of the License, or (at your option) any later version.
//
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
// </remarks>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.IO;

using Backsight.Environment;
using Backsight.Data;
using Backsight.Editor.Observations;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="30-JAN-2003" was="CeAttachPoint" />
    /// <summary>
    /// Operation to attach a point to a line.
    /// </summary>
    class AttachPointOperation : Operation
    {
        /// <summary>
        /// The max value stored for <c>m_PositionRatio</c>
        /// </summary>
        const uint MAX_POSITION_RATIO = 1000000000;

        #region Class data

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        uint m_PositionRatio;

        /// <summary>
        /// The point that was created 
        /// </summary>
        PointFeature m_Point;

        #endregion

        #region Constructors

        public AttachPointOperation()
            : base()
        {
            SetInitialValues();
        }

        internal AttachPointOperation(Session s)
            : base(s)
        {
            SetInitialValues();
        }

        #endregion

        /// <summary>
        /// Initializes class data with default values
        /// </summary>
        void SetInitialValues()
        {
            m_Line = null;
            m_PositionRatio = 0;
            m_Point = null;
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Attach point to line"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get { return new Feature[] { m_Point }; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.AttachPoint; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            if (m_Line!=null)
                m_Line.AddOp(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();
            Rollback(m_Point);
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

            // Re-calculate the position of the attached point & move it.
            IPosition xpos = Calculate();
            m_Point.Move(xpos);

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Calculates the position of the attached point.
        /// </summary>
        /// <returns></returns>
        IPosition Calculate()
        {
            Debug.Assert(m_PositionRatio <= MAX_POSITION_RATIO);

            // Get the current length of the line the point is attached to
            double len = m_Line.Length.Meters;

            // Get the distance to the attached point
            double dist = len * ((double)(m_PositionRatio)/(double)MAX_POSITION_RATIO);

            // Get the position for the point
            IPosition xpos;
            if (m_Line.LineGeometry.GetPosition(new Length(dist), out xpos))
                return xpos;

            throw new Exception("Unable to calculate position of attached point");
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="line">The line to attach the point to</param>
        /// <param name="posn">The position on the line at which to attach the point</param>
        /// <param name="type">The entity type for the point.</param>
        internal void Execute(LineFeature line, IPosition posn, IEntity type)
        {
            // Get the distance to the supplied position (confirming that it does fall on the line)
            LineGeometry g = line.LineGeometry;
            double lineLen = g.Length.Meters;
            double posnLen = g.GetLength(posn).Meters;
            if (posnLen < 0.0)
                throw new Exception("Position does not appear to coincide with line.");

            // Remember the line the point is getting attached to
            m_Line = line;

            // Express the position as a position ratio in the range [0,1 billion]
            double prat = posnLen/lineLen;
            m_PositionRatio = (uint)(prat * (double)MAX_POSITION_RATIO);
            Debug.Assert(m_PositionRatio>=0);
            Debug.Assert(m_PositionRatio<=MAX_POSITION_RATIO);

            // Add the point to the map.
            m_Point = MapModel.AddPoint(posn, type, this);

            // If necessary, assign the new point the next available ID.
            m_Point.SetNextId();

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
        }

        /// <summary>
        /// The point that was created (defined on a call to <see cref="Execute"/>)
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_Point; }
        }

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        internal uint PositionRatio
        {
            get { return m_PositionRatio; }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            writer.WriteFeatureReference("Line", m_Line);
            writer.WriteUnsignedInt("PositionRatio", m_PositionRatio);
            writer.WriteCalculatedPoint("Point", m_Point);
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
            m_PositionRatio = reader.ReadUnsignedInt("PositionRatio");
            //m_Point = reader.ReadCalculatedPoint("Point", Calculate());
            m_Point = reader.ReadPoint("Point");
        }

        public override void WriteAttributes(ContentWriter writer)
        {
            base.WriteAttributes(writer);
            writer.WriteFeatureReference("Line", m_Line);
            writer.WriteUnsignedInt("PositionRatio", m_PositionRatio);
        }

        public override void WriteChildElements(ContentWriter writer)
        {
            base.WriteChildElements(writer);
            //writer.WriteElement("Point", Point);
        }

        public override void ReadAttributes(ContentReader reader)
        {
            base.ReadAttributes(reader);
            //Line = new Id(reader.ReadString("Line"));
            m_PositionRatio = reader.ReadUnsignedInt("PositionRatio");
        }

        public override void ReadChildElements(ContentReader reader)
        {
            base.ReadChildElements(reader);
            //Point = reader.ReadElement<SpatialItem>("Point");
        }

        /// <summary>
        /// Calculates the geometry for any features created by this edit.
        /// </summary>
        public override void CalculateGeometry()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_Point.PointGeometry = pg;
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
 