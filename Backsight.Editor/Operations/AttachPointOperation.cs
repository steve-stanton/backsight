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

        #region Static

        /// <summary>
        /// Obtains the position ratio for a position that is coincident with a line.
        /// </summary>
        /// <param name="line">The line the position is coincident with</param>
        /// <param name="posn">The position on the line</param>
        /// <returns>The position ratio of the position, expressed in the numeric range
        /// expected by this editing operation.</returns>
        /// <exception cref="ArgumentException">If the position does not appear to coincide
        /// with the supplied line.</exception>
        static internal uint GetPositionRatio(LineFeature line, IPosition posn)
        {
            // Get the distance to the supplied position (confirming that it does fall on the line)
            LineGeometry g = line.LineGeometry;
            double lineLen = g.Length.Meters;
            double posnLen = g.GetLength(posn).Meters;
            if (posnLen < 0.0)
                throw new ArgumentException("Position does not appear to coincide with line.");

            // Express the position as a position ratio in the range [0,1 billion]
            double prat = posnLen/lineLen;
            uint result = (uint)(prat * (double)MAX_POSITION_RATIO);
            Debug.Assert(result <= MAX_POSITION_RATIO);
            return result;
        }

        #endregion


        #region Class data

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        readonly uint m_PositionRatio;

        /// <summary>
        /// The point that was created 
        /// </summary>
        PointFeature m_Point;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachPointOperation"/> class.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="line">The line the point should appear on.</param>
        /// <param name="positionRatio">The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).</param>
        internal AttachPointOperation(Session s, uint sequence, LineFeature line, uint positionRatio)
            : base(s, sequence)
        {
            if (line == null)
                throw new ArgumentNullException();

            m_Line = line;
            m_PositionRatio = positionRatio;
            m_Point = null;
        }

        #endregion

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
        /// <param name="uc">The context in which editing revisions are being made (not null).
        /// Used to hold a record of any positional changes.</param>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward(UpdateContext uc)
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the attached point & move it.
            IPosition xpos = Calculate();
            m_Point.MovePoint(uc, xpos);

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
        /// Creates any new spatial features (without any geometry)
        /// </summary>
        /// <param name="ff">The factory class for generating spatial features</param>
        internal override void CreateFeatures(FeatureFactory ff)
        {
            m_Point = ff.CreateDirectPointFeature("Point");
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_Point.PointGeometry = pg;
        }

        /// <summary>
        /// The line the point should appear on 
        /// </summary>
        internal LineFeature Line
        {
            get { return m_Line; }
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
        /// The point that was created (defined on a call to <see cref="Execute"/>)
        /// </summary>
        internal PointFeature NewPoint
        {
            get { return m_Point; }
            set { m_Point = value; }
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
 