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

using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersectDir" />
    /// <summary>
    /// Create point (and optional lines) based on two direction observations.
    /// </summary>
    class IntersectTwoDirectionsOperation : IntersectOperation, IRecallable
    {
        #region Class data

        /// <summary>
        /// The first observed direction
        /// </summary>
        Direction m_Direction1;

        /// <summary>
        /// The second observed direction
        /// </summary>
        Direction m_Direction2;

        // Creations ...

        /// <summary>
        /// The created intersection point (if any).
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The first line created (if any).
        /// Should always be null if the first direction has an offset.
        /// </summary>
        LineFeature m_Line1; // was m_pArc1

        /// <summary>
        /// The second line created (if any).
        /// Should always be null if the second direction has an offset.
        /// </summary>
        LineFeature m_Line2; // was m_pArc2

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization. The point created by this edit
        /// is defined without any geometry. A subsequent call to <see cref="CalculateGeometry"/>
        /// is needed to define the geometry.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal IntersectTwoDirectionsOperation(Session s, IntersectTwoDirectionsData t)
            : base(s, t)
        {
            m_Direction1 = (Direction)t.Direction1.LoadObservation(this);
            m_Direction2 = (Direction)t.Direction2.LoadObservation(this);
            m_To = new PointFeature(this, t.To);

            if (t.Line1 == null)
                m_Line1 = null;
            else
                m_Line1 = new LineFeature(this, m_Direction1.From, m_To, t.Line1);

            if (t.Line2 == null)
                m_Line2 = null;
            else
                m_Line2 = new LineFeature(this, m_Direction2.From, m_To, t.Line2);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoDirectionsOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal IntersectTwoDirectionsOperation(Session s)
            : base(s)
        {
            m_Direction1 = null;
            m_Direction2 = null;
            m_To = null;
            m_Line1 = null;
            m_Line2 = null;
        }

        #endregion

        /// <summary>
        /// The point feature at the intersection created by this edit.
        /// </summary>
        internal override PointFeature IntersectionPoint // was GetpIntersect
        {
            get { return m_To; }
        }

        /// <summary>
        /// The first line created (if any).
        /// </summary>
        internal LineFeature CreatedLine1 // was GetpArc1
        {
            get { return m_Line1; }
        }

        /// <summary>
        /// The second line created (if any).
        /// </summary>
        internal LineFeature CreatedLine2 // was GetpArc2
        {
            get { return m_Line2; }
        }

        /// <summary>
        /// Returns true (always), indicating that the intersection was created at
        /// it's default position.
        /// </summary>
        internal override bool IsDefault
        {
            get { return true; }
        }

        /// <summary>
        /// The first observed direction
        /// </summary>
        internal Direction Direction1
        {
            get { return m_Direction1; }
        }

        /// <summary>
        /// The second observed direction
        /// </summary>
        internal Direction Direction2
        {
            get { return m_Direction2; }
        }

        /// <summary>
        /// A point feature that is close to the intersection (for use when relocating
        /// the intersection as part of rollforward processing). This implementation
        /// returns null.
        /// </summary>
        internal override PointFeature ClosePoint
        {
            get { return null; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Direction - direction intersection"; }
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
            get
            {
                List<Feature> result = new List<Feature>(3);

                if (m_To!=null)
                    result.Add(m_To);

                AddCreatedFeatures(m_Line1, result);
                AddCreatedFeatures(m_Line2, result);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.DirIntersect; }
        }

        /// <summary>
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_Direction1.AddReferences(this);
            m_Direction2.AddReferences(this);
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Get rid of the observations.
            m_Direction1.OnRollback(this);
            m_Direction2.OnRollback(this);

            // Undo the intersect point and any connecting lines
            Rollback(m_To);
            Rollback(m_Line1);
            Rollback(m_Line2);

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

            // Re-calculate the position of the point of intersection.
            IPosition xsect = m_Direction1.Intersect(m_Direction2);

            if (xsect==null)
                throw new RollforwardException(this, "Cannot re-calculate intersection point.");

            // Update the intersection point to the new position.
            m_To.MovePoint(uc, xsect);

            // Rollforward the base class.
            return base.OnRollforward();
        }

        /// <summary>
        /// Executes this operation. 
        /// </summary>
        /// <param name="dir1">First direction observation.</param>
        /// <param name="dir2">Second direction observation.</param>
        /// <param name="pointId">The ID and entity type for the intersect point
        /// If null, the default entity type for point features will be used.</param>
        /// <param name="lineEnt1">The entity type for a line connecting the 1st direction to the
        /// intersection (null for no line)</param>
        /// <param name="lineEnt2">The entity type for a line connecting the 2nd direction to the
        /// intersection (null for no line)</param>
        internal void Execute(Direction dir1, Direction dir2, IdHandle pointId, IEntity lineEnt1, IEntity lineEnt2)
        {
            // Calculate the position of the point of intersection.
            IPosition xsect = dir1.Intersect(dir2);
            if (xsect==null)
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_To = AddIntersection(xsect, pointId);

            // Remember input
            m_Direction1 = dir1;
            m_Direction2 = dir2;

            // If we have a defined entity types for lines, add them too.
            CadastralMapModel map = MapModel;

            if (lineEnt1!=null)
            {
                IPosition start = m_Direction1.StartPosition;
                PointFeature ps = map.EnsurePointExists(start, this);
                m_Line1 = map.AddLine(ps, m_To, lineEnt1, this);
            }

            if (lineEnt2!=null)
            {
                IPosition start = m_Direction2.StartPosition;
                PointFeature ps = map.EnsurePointExists(start, this);
                m_Line2 = map.AddLine(ps, m_To, lineEnt2, this);
            }

            // Peform standard completion steps
            Complete();
        }

        /// <summary>
        /// Calculates the position of the intersection (if any).
        /// </summary>
        /// <returns>The position of the intersection (null if it cannot be calculated).</returns>
        IPosition Calculate()
        {
            if (m_Direction1!=null && m_Direction2!=null)
                return m_Direction1.Intersect(m_Direction2);
            else
                return null;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            if (m_Direction1.HasReference(feat))
                return true;

            if (m_Direction2.HasReference(feat))
                return true;

            return false;
        }

        /// <summary>
        /// Updates this direction-direction intersection operation.
        /// </summary>
        /// <param name="dir1">First direction.</param>
        /// <param name="dir2">Second direction.</param>
        /// <param name="lineEnt1">The entity type for a line connecting the 1st direction to the intersection</param>
        /// <param name="lineEnt2">The entity type for a line connecting the 2nd direction to the intersection</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct(Direction dir1, Direction dir2, IEntity lineEnt1, IEntity lineEnt2)
        {
            if ((lineEnt1==null && m_Line1!=null) || (lineEnt2==null && m_Line2!=null))
                throw new Exception("You cannot delete lines via update. Use Line Delete.");

            // Calculate the position of the point of intersection.
            IPosition xsect = dir1.Intersect(dir2);
            if (xsect==null)
                return false;

            // Cut the references made by the direction objects. If nothing
            // has changed, the references will be re-inserted when the
            // direction is re-saved below.
            m_Direction1.CutRef(this);
            m_Direction2.CutRef(this);

            // Get rid of the previously defined observations, and replace
            // with the new ones (we can't necessarily change the old ones
            // because we may have changed the type of observation).

            m_Direction1.OnRollback(this);
            m_Direction2.OnRollback(this);

            m_Direction1 = dir1;
            m_Direction1.AddReferences(this);

            m_Direction2 = dir2;
            m_Direction2.AddReferences(this);

            // If we have defined entity types for lines, and we did not
            // have a line before, add a new line now.

            if (lineEnt1!=null)
            {
                if (m_Line1==null)
                {
                    CadastralMapModel map = MapModel;
                    IPosition start = m_Direction1.StartPosition;
                    PointFeature ps = map.EnsurePointExists(start, this);
                    m_Line1 = map.AddLine(ps, m_To, lineEnt1, this);
                }
                else if (m_Line1.EntityType.Id != lineEnt1.Id)
                    throw new NotImplementedException("IntersectTwoDirectionsOperation.Correct");
                    //m_Line1.EntityType = lineEnt1;
            }

            if (lineEnt2!=null)
            {
                if (m_Line2==null)
                {
                    CadastralMapModel map = MapModel;
                    IPosition start = m_Direction2.StartPosition;
                    PointFeature ps = map.EnsurePointExists(start, this);
                    m_Line2 = map.AddLine(ps, m_To, lineEnt2, this);
                }
                else if (m_Line2.EntityType.Id != lineEnt2.Id)
                    throw new NotImplementedException("IntersectTwoDirectionsOperation.Correct");
                    //m_Line2.EntityType = lineEnt2;
            }

            return true;
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationData GetSerializableEdit()
        {
            IntersectTwoDirectionsData t = new IntersectTwoDirectionsData();
            base.SetSerializableEdit(t);

            t.Direction1 = (DirectionData)m_Direction1.GetSerializableObservation();
            t.Direction2 = (DirectionData)m_Direction2.GetSerializableObservation();
            t.To = new CalculatedFeatureData(m_To);

            if (m_Line1 != null)
                t.Line1 = new CalculatedFeatureData(m_Line1);

            if (m_Line2 != null)
                t.Line2 = new CalculatedFeatureData(m_Line2);

            return t;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_To.PointGeometry = pg;
        }
    }
}
