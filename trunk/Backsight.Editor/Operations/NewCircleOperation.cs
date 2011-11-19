// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using Backsight.Environment;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="01-DEC-1998" was="CeNewCircle" />
    /// <summary>
    /// Operation to add a new circle
    /// </summary>
    class NewCircleOperation : NewLineOperation, IRecallable //, IRevisable
    {
        #region Class data

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        readonly PointFeature m_Center;

        /// <summary>
        /// The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of
        /// the circle.
        /// </summary>
        readonly Observation m_Radius;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="NewCircleOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of the circle.</param>
        internal NewCircleOperation(Session s, uint sequence, PointFeature center, Observation radius)
            : base(s, sequence)
        {
            m_Center = center;
            m_Radius = radius;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NewCircleOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal NewCircleOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            FeatureStub closingPoint, arc;
            ReadData(editDeserializer, out m_Center, out m_Radius, out closingPoint, out arc);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub("ClosingPoint", closingPoint);
            dff.AddFeatureStub("Arc", arc);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// Point at the center of the circle.
        /// </summary>
        internal PointFeature Center
        {
            get { return m_Center; }
        }

        /// <summary>
        /// The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of
        /// the circle.
        /// </summary>
        internal Observation Radius
        {
            get { return m_Radius; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Add new circle"; }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.NewCircle; }
        }

        /// <summary>
        /// Rollback this operation.
        /// </summary>
        /// <returns></returns>
        internal override bool Undo()
        {
            // Delete observed radius.
            m_Radius.OnRollback(this);

            // Cut direct refs made by this operation.
            if (m_Center != null)
                m_Center.CutOp(this);

            // Rollback the base class (mark the circle for undo).
            return base.Undo();
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            throw new NotImplementedException();
            /*
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Get the circle that has changed.
            ArcFeature line = (this.Line as ArcFeature);
            if (line==null)
                throw new RollforwardException(this, "NewCircleOperation.Rollforward - Unexpected line type.");
            ArcGeometry curve = (ArcGeometry)line.LineGeometry;

            Circle circle = (Circle)curve.Circle;

            // Get the new radius (on the ground).
            double rad = m_Radius.GetDistance(m_Center).Meters;
            if (rad < Constants.TINY)
                throw new RollforwardException(this, "NewCircleOperation.Rollforward - New radius is too small.");

            // Mark the circle as moved (force CleanEdit to re-intersect).
            // Actually, I think this should do nothing, seeing how circle
            // construction lines are supposed to always be non-topolgical.
            line.IsMoved = true;

	        // If the underlying circle was created by this op, update
	        // it so that it has the correct center and radius.
	        // If the circle previously existed, make a new one (if there
	        // isn't one there already).

            if (Object.ReferenceEquals(circle.Creator, this))
                circle.MoveCircle(m_Center, rad);
            else
            {
                // Is there a suitable circle where we're going to? If not, create a new one.
                // If a new circle gets created, this will reference the center point to the circle.
                circle = MapModel.AddCircle(m_Center, rad);
            }

            // Update the arc geometry. The BC=EC must move to
            // fall on the updated circle. For circles where the
            // radius was defined using an offset point, we move
            // to there. Otherwise we want a point at the top of
            // the circle.

            if (m_Radius is OffsetPoint)
            {
                OffsetPoint offset = (m_Radius as OffsetPoint);
                PointFeature start = offset.Point;

                // Alter the arc (the complete circle) so it starts at
                // (and ends) at the offset position.
                // 09-JAN-08: is this really needed? - the line ends where the point is!
                //line.ChangeEnds(start, start);
            }
            else
            {
                // Get the new start location for the curve and shift the
                // BC/EC point
                IPosition newstart = new Position(m_Center.X, m_Center.Y + rad);
                line.StartPoint.MovePoint(uc, newstart);
            }

            // Rollforward the base class (does nothing)
            return base.Rollforward(uc);
             */
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();

            result.Add(m_Center);
            result.AddRange(m_Radius.GetReferences());

            return result.ToArray();
        }

        /// <summary>
        /// Executes this new circle operation.
        /// </summary>
        internal void Execute()
        {
            FeatureFactory ff = new FeatureFactory(this);

            // The circle (and any closing point) will have the blank entity type (with ID=0).
            // If you don't do this, the factory will create stuff with default entity types,
            // and possibly a user-perceived ID.
            IEntity blank = EnvironmentContainer.FindBlankEntity();
            ff.PointType = blank;
            ff.LineType = blank;

            base.Execute(ff);
            /*
            // Get the radius, in meters on the ground.
            double rad = m_Radius.GetDistance(m_Center).Meters;
            if (rad < Constants.TINY)
                throw new Exception("NewCircleOperation.Execute - Radius is too close to zero.");

            // If the radius was specified as an offset point, make the circle
            // start at that point.
            OffsetPoint offset = (m_Radius as OffsetPoint);
            PointFeature start = (offset == null ? null : offset.Point);

            // Add a circle to the map.
            CadastralMapModel map = MapModel;
            ArcFeature arc = map.AddCompleteCircularArc(m_Center, rad, start, this);

            // Record the new arc in the base class.
            SetNewLine(arc);

            // Peform standard completion steps
            Complete();
             */
        }

        /// <summary>
        /// Performs data processing that involves creating or retiring spatial features.
        /// Newly created features will not have any definition for their geometry - a
        /// subsequent call to <see cref="CalculateGeometry"/> is needed to to that.
        /// </summary>
        /// <param name="ff">The factory class for generating any spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            // If the closing point does not already exist, create one at some unspecified position
            OffsetPoint offset = (m_Radius as OffsetPoint);
            PointFeature p = (offset == null ? null : offset.Point);
            if (p == null)
                p = ff.CreatePointFeature("ClosingPoint");

            // Form the construction line (there is no associated circle at this stage)
            ArcFeature arc = ff.CreateArcFeature("Arc", p, p);

            base.SetNewLine(arc);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            // Get the radius, in meters on the ground.
            double rad = m_Radius.GetDistance(m_Center).Meters;
            if (rad < Constants.TINY)
                throw new Exception("NewCircleOperation.CalculateGeometry - Radius is too close to zero.");

            // If the closing point was created by this edit, define it's position
            ArcFeature arc = (ArcFeature)this.Line;
            PointFeature p = arc.StartPoint;
            if (p.Creator == this)
            {
                PointGeometry pg = new PointGeometry(m_Center.X, m_Center.Y+rad);
                p.ApplyPointGeometry(ctx, pg);
            }

            // Try to find an existing circle. If we don't find one, create one (attaching it to
            // the center point);
            Circle circle = MapModel.AddCircle(m_Center, rad);

            // Define the geometry for the feature (and attach the arc to the circle)
            arc.Geometry = new ArcGeometry(circle, arc.StartPoint, arc.StartPoint, true);
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            if (Object.ReferenceEquals(m_Center, feat))
                return true;

            if (m_Radius.HasReference(feat))
                return true;

            return false;
        }

        /// <summary>
        /// Corrects this operation.
        /// </summary>
        /// <param name="center">The point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle (either a <see cref="Distance"/> object,
        /// or an <see cref="OffsetPoint"/> that sits on the circumference of the circle.</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct(PointFeature center, Observation radius)
        {
            throw new NotImplementedException();

            /*
            // If the center point has changed, cut the reference to this
            // operation from the old point, and change it so the
            // operation is referenced from the new center.
            if (!Object.ReferenceEquals(m_Center, center))
            {
                m_Center.CutOp(this);
                m_Center = center;
                m_Center.AddOp(this);
            }

            // If the old radius observation refers to an offset point, cut
            // the reference that the point has to this op. If nothing has
            // changed, the reference will be re-inserted when the
            // observation is re-saved below.
            CutOffsetRef(m_Radius);

            // Get rid of the previously defined observation, and replace
            // with the new one (we can't necessarily change the old one
            // because we may have changed the type of observation).

            m_Radius.OnRollback(this);
            m_Radius = radius;
            m_Radius.AddReferences(this);

            return true;
             */
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                // The point at the BC (and EC) of the arc may have been created by this
                // operation, or may be a previously existing point used to define the radius.

                PointFeature p = this.Line.StartPoint;

                if (Object.ReferenceEquals(p.Creator, this))
                    return new Feature[] { this.Line, p };
                else                
                    return new Feature[] { this.Line };
            }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WriteFeatureRef<PointFeature>(DataField.Center, m_Center);
            editSerializer.WritePersistent<Observation>(DataField.Radius, m_Radius);

            // Record a closing point only if it was created by this edit -- in that case, the radius must
            // be a plain Distance (if the radius is defined using an OffsetPoint observation, the closing
            // point will coincide with the offset point itself -- we don't have to write anything further,
            // because the reference to the offset point is part of the observation object written above).

            if (Line.StartPoint.Creator == this)
            {
                Debug.Assert(m_Radius is Distance);
                editSerializer.WritePersistent<FeatureStub>(DataField.ClosingPoint, new FeatureStub(Line.StartPoint));
            }
            else
            {
                Debug.Assert(m_Radius is OffsetPoint);
            }

            editSerializer.WritePersistent<FeatureStub>(DataField.Arc, new FeatureStub(Line));
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="center">Point at the center of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <param name="arc">The arc running around the circumference of the circle</param>
        /// <param name="closingPoint">The closing point of the circle (if it was created by this edit). Null
        /// if the radius was specified using an offset point.</param>
        static void ReadData(EditDeserializer editDeserializer, out PointFeature center, out Observation radius,
                                out FeatureStub closingPoint, out FeatureStub arc)
        {
            center = editDeserializer.ReadFeatureRef<PointFeature>(DataField.Center);
            radius = editDeserializer.ReadPersistent<Observation>(DataField.Radius);
            closingPoint = editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.ClosingPoint);
            arc = editDeserializer.ReadPersistent<FeatureStub>(DataField.Arc);
        }
    }
}
