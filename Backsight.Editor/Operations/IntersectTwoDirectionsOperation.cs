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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersectDir" />
    /// <summary>
    /// Create point (and optional lines) based on two direction observations.
    /// </summary>
    class IntersectTwoDirectionsOperation : IntersectOperation, IRecallable, IRevisable
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
        /// Initializes a new instance of the <see cref="IntersectTwoDirectionsOperation"/> class.
        /// </summary>
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="dir1">The first observed direction</param>
        /// <param name="dir2">The second observed direction</param>
        internal IntersectTwoDirectionsOperation(Session session, uint sequence, Direction dir1, Direction dir2)
            : base(session, sequence)
        {
            m_Direction1 = dir1;
            m_Direction2 = dir2;
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
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();
            result.AddRange(m_Direction1.GetReferences());
            result.AddRange(m_Direction2.GetReferences());
            return result.ToArray();
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
        /// Executes this operation. 
        /// </summary>
        /// <param name="pointId">The ID and entity type for the intersect point
        /// If null, the default entity type for point features will be used.</param>
        /// <param name="lineEnt1">The entity type for a line connecting the 1st direction to the
        /// intersection (null for no line)</param>
        /// <param name="lineEnt2">The entity type for a line connecting the 2nd direction to the
        /// intersection (null for no line)</param>
        internal void Execute(IdHandle pointId, IEntity lineEnt1, IEntity lineEnt2)
        {
            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature x = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription("To", x);

            if (lineEnt1 != null)
            {
                // Lines are not allowed if the direction line is associated with an offset
                // distance (since we would then need to add a point at the start of the
                // direction line). This should have been trapped by the UI. Note that an
                // offset specified using an OffsetPoint is valid.

                if (m_Direction1.Offset is OffsetDistance)
                    throw new ApplicationException("Cannot add direction line because a distance offset is involved");

                IFeature f = new FeatureStub(this, lineEnt1, null);
                ff.AddFeatureDescription("Line1", f);
            }

            if (lineEnt2 != null)
            {
                if (m_Direction2.Offset is OffsetDistance)
                    throw new ApplicationException("Cannot add direction line because a distance offset is involved");

                IFeature f = new FeatureStub(this, lineEnt2, null);
                ff.AddFeatureDescription("Line2", f);
            }

            base.Execute(ff);

/*
            // Calculate the position of the point of intersection.
            IPosition xsect = m_Direction1.Intersect(m_Direction2);
            if (xsect==null)
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_To = AddIntersection(xsect, pointId);

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
 */
        }

        /// <summary>
        /// Creates any new spatial features (without any geometry)
        /// </summary>
        /// <param name="ff">The factory class for generating spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            m_To = ff.CreatePointFeature("To");

            if (ff.HasFeatureDescription("Line1"))
            {
                OffsetPoint op1 = m_Direction1.Offset as OffsetPoint;
                PointFeature from1 = (op1 == null ? m_Direction1.From : op1.Point);
                m_Line1 = ff.CreateSegmentLineFeature("Line1", from1, m_To);
            }

            if (ff.HasFeatureDescription("Line2"))
            {
                OffsetPoint op2 = m_Direction2.Offset as OffsetPoint;
                PointFeature from2 = (op2 == null ? m_Direction2.From : op2.Point);
                m_Line2 = ff.CreateSegmentLineFeature("Line2", from2, m_To);
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
            m_To.ApplyPointGeometry(ctx, pg);
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
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="dir1">1st direction observation.</param>
        /// <param name="dir2">2nd direction observation.</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(Direction dir1, Direction dir2)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddObservation<Direction>("Direction1", m_Direction1, dir1);
            result.AddObservation<Direction>("Direction2", m_Direction2, dir2);
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
            m_Direction1 = data.ExchangeObservation<Direction>(this, "Direction1", m_Direction1);
            m_Direction2 = data.ExchangeObservation<Direction>(this, "Direction2", m_Direction2);
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoDirectionsOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// </summary>
        internal IntersectTwoDirectionsOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            throw new NotImplementedException();
        }
    }
}
