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
using Backsight.Environment;
using Backsight.Geometry;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersectDirDist" />
    /// <summary>
    /// Create point (and optional lines) based on a direction and a distance observation.
    /// </summary>
    class IntersectDirectionAndDistanceOperation : IntersectOperation, IRecallable, IRevisable
    {
        #region Class data

        /// <summary>
        /// The observed direction
        /// </summary>
        Direction m_Direction;

        /// <summary>
        /// The observed distance (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Distance;

        /// <summary>
        /// The point the distance was measured from.
        /// </summary>
        PointFeature m_From;

        /// <summary>
        /// True if it was the default intersection (the one closest to the
        /// origin of the direction).
        /// </summary>
        bool m_Default;

        // Creations ...

        /// <summary>
        /// The created intersection point (if any).
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The first line created (if any). Should always be null if the direction
        /// has an offset.
        /// </summary>
        LineFeature m_DirLine; // was m_pDirArc

        /// <summary>
        /// The second line created (if any).
        /// </summary>
        LineFeature m_DistLine; // was m_pDistArc

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectDirectionAndDistanceOperation"/> class
        /// </summary>
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="dir">Direction observation.</param>
        /// <param name="dist">Distance observation.</param>
        /// <param name="from">The point the distance was observed from.</param>
        /// <param name="usedefault">True if the default intersection is required (the one 
        /// closer to the origin of the direction line). False for the other one (if any).</param>
        internal IntersectDirectionAndDistanceOperation(Session session, uint sequence, Direction dir,
                                                        Observation dist, PointFeature from, bool useDefault)
            : base(session, sequence)
        {
            m_Direction = dir;
            m_Distance = dist;
            m_From = from;
            m_Default = useDefault;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectDirectionAndDistanceOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal IntersectDirectionAndDistanceOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            FeatureStub to, dirLine, distLine;
            ReadData(editDeserializer, out m_Direction, out m_Distance, out m_From, out m_Default,
                            out to, out dirLine, out distLine);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub("To", to);
            dff.AddFeatureStub("DirLine", dirLine);
            dff.AddFeatureStub("DistLine", distLine);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// The observed direction
        /// </summary>
        internal Direction Direction
        {
            get { return m_Direction; }
        }

        /// <summary>
        /// The observed distance (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation Distance
        {
            get { return m_Distance; }
        }

        /// <summary>
        /// The point the distance was measured from.
        /// </summary>
        internal PointFeature DistanceFromPoint // was GetpDistFrom
        {
            get { return m_From; }
        }

        /// <summary>
        /// The first line created (if any). Should always be null if the direction
        /// has an offset.
        /// </summary>
        internal LineFeature CreatedDirectionLine // was GetpDirArc
        {
            get { return m_DirLine; }
        }

        /// <summary>
        /// The second line created (if any).
        /// </summary>
        internal LineFeature CreatedDistanceLine // was GetpDistArc
        {
            get { return m_DistLine; }
        }

        /// <summary>
        /// The point feature at the intersection created by this edit.
        /// </summary>
        internal override PointFeature IntersectionPoint
        {
            get { return m_To; }
        }

        /// <summary>
        /// Was the intersection created at it's default position?
        /// </summary>
        internal override bool IsDefault
        {
            get { return m_Default; }
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
            get { return "Direction - distance intersection"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            // If the distance-line is the one we're after, AND it was
            // defined as a distance (as opposed to an offset point),
            // return a reference to it.
            if (Object.ReferenceEquals(line, m_DistLine))
                return (m_Distance as Distance);

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

                AddCreatedFeatures(m_DirLine, result);
                AddCreatedFeatures(m_DistLine, result);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.DirDistIntersect; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();

            result.Add(m_From);
            result.AddRange(m_Direction.GetReferences());
            result.AddRange(m_Distance.GetReferences());

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
            m_Direction.OnRollback(this);
            m_Distance.OnRollback(this);

            // Cut direct refs made by this operation.
            if (m_From!=null)
                m_From.CutOp(this);

            // Undo the intersect point and any connecting lines
            Rollback(m_To);
            Rollback(m_DirLine);
            Rollback(m_DistLine);
            
            return true;
        }

        /// <summary>
        /// Executes this operation. 
        /// </summary>
        /// <param name="pointId">The ID and entity type for the intersect point</param>
        /// <param name="ent1">The entity type for 1st line (null for no line)</param>
        /// <param name="ent2">The entity type for 2nd line (null for no line)</param>
        internal void Execute(IdHandle pointId, IEntity ent1, IEntity ent2)
        {
            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature x = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription("To", x);

            if (ent1 != null)
            {
                // Lines are not allowed if the direction line is associated with an offset
                // distance (since we would then need to add a point at the start of the
                // direction line). This should have been trapped by the UI. Note that an
                // offset specified using an OffsetPoint is valid.

                if (m_Direction.Offset is OffsetDistance)
                    throw new ApplicationException("Cannot add direction line because a distance offset is involved");

                IFeature f = new FeatureStub(this, ent1, null);
                ff.AddFeatureDescription("DirLine", f);
            }

            if (ent2 != null)
            {
                IFeature f = new FeatureStub(this, ent2, null);
                ff.AddFeatureDescription("DistLine", f);
            }

            base.Execute(ff);

            /*
            // Calculate the position of the point of intersection.
            IPosition xsect = Calculate(m_Direction, m_Distance, m_From, m_Default);
            if (xsect==null)
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_To = AddIntersection(xsect, pointId);

            // If we have a defined entity types for lines, add them too.
            CadastralMapModel map = MapModel;

            if (ent1!=null)
            {
                IPosition start = m_Direction.StartPosition;
                PointFeature ps = map.EnsurePointExists(start, this);
                m_DirLine = map.AddLine(ps, m_To, ent1, this);
            }

            if (ent2!=null)
                m_DistLine = map.AddLine(m_From, m_To, ent2, this);

            // Peform standard completion steps
            Complete();
             */
        }

        /// <summary>
        /// Calculates the intersection point.
        /// </summary>
        /// <param name="dir">Direction observation.</param>
        /// <param name="dist">Distance observation.</param>
        /// <param name="from">The point the distance was observed from.</param>
        /// <param name="usedefault">True if the default intersection is required (the one 
        /// closer to the origin of the direction line). False for the other one (if any).</param>
        /// <returns>The position of the intersection (null if it cannot be calculated).</returns>
        IPosition Calculate(Direction dir, Observation distance, PointFeature from, bool usedefault)
        {
            // Call the static function that is also used by the dialog.
            IPosition xsect, x1, x2;
            if (Calculate(dir, distance, from, usedefault, out xsect, out x1, out x2))
                return xsect;
            else
                return null;
        }

        /// <summary>
        /// Calculates the intersection point.
        /// </summary>
        /// <param name="dir">Direction observation.</param>
        /// <param name="distance">Distance observation.</param>
        /// <param name="from">The point the distance was observed from.</param>
        /// <param name="usedefault">True if the default intersection is required (the one 
        /// closer to the origin of the direction line). False for the other one (if any).</param>
        /// <param name="xsect">The position of the intersection (if any).</param>
        /// <param name="xsect1">The 1st choice intersection (if any).</param>
        /// <param name="xsect2">The 2nd choice intersection (if any).</param>
        /// <returns>True if intersections were calculated. False if the distance circles
        /// don't intersect.</returns>
        internal static bool Calculate(Direction dir, Observation distance, PointFeature from, bool usedefault,
                                        out IPosition xsect, out IPosition xsect1, out IPosition xsect2)
        {
            // Initialize intersection positions.
            xsect = xsect1 = xsect2 = null;

            // Get the distance.
            double dist = distance.GetDistance(from).Meters;
            if (dist < Constants.TINY)
                return false;

            // Form circle with a radius that matches the observed distance.
            ICircleGeometry circle = new CircleGeometry(from, dist);

            // See if there is actually an intersection between the direction & the circle.
            IPosition x1, x2;
            uint nx = dir.Intersect(circle, out x1, out x2);
            if (nx==0)
                return false;

            // If we have 2 intersections, and we need the non-default one, pick up the 2nd
            // intersection. If only 1 intersection, use that, regardless of the setting for
            // the "use default" flag.

            if (nx==2 && !usedefault)
                xsect = x2;
            else
                xsect = x1;

            // Return if the distance is an offset point.
            OffsetPoint offset = (distance as OffsetPoint);

            if (offset!=null)
            {
                xsect1 = x1;
                xsect2 = x2;
                return true;
            }

            // Reduce observed distance to the mapping plane.
            ISpatialSystem sys = CadastralMapModel.Current.SpatialSystem;
            dist = dist * sys.GetLineScaleFactor(from, xsect);

            // And calculate the exact intersection (like above)...
            // Form circle with a radius that matches the reduced distance.
            ICircleGeometry circlep = new CircleGeometry(from, dist);

            // See if there is actually an intersection between the direction & the circle.
            nx = dir.Intersect(circle, out x1, out x2);
            if (nx==0)
                return false;

            // If we have 2 intersections, and we need the non-default one, pick up the 2nd
            // intersection. If only 1 intersection, use that, regardless of the setting for
            // the "use default" flag.

            if (nx==2 && !usedefault)
                xsect = x2;
            else
                xsect = x1;

            xsect1 = x1;
            xsect2 = x2;

            return true;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            if (Object.ReferenceEquals(m_From, feat))
                return true;

            if (m_Direction.HasReference(feat))
                return true;

            if (m_Distance.HasReference(feat))
                return true;

            return false;
        }

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="dist1">1st distance observation.</param>
        /// <param name="from1">The point the 1st distance was observed from.</param>
        /// <param name="dist2">2nd distance observation.</param>
        /// <param name="from2">The point the 2nd distance was observed from.</param>
        /// <param name="isdefault">True if the default intersection is required (the one that has the
        /// lowest bearing with respect to the 2 from points). False for the other one (if any).</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(Direction dir, Observation distance,
                                                        PointFeature from, bool isdefault)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddObservation<Direction>("Direction", m_Direction, dir);
            result.AddObservation<Observation>("Distance", m_Distance, distance);
            result.AddFeature<PointFeature>("From", m_From, from);
            result.AddItem<bool>("Default", m_Default, isdefault);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteObservation<Direction>(editSerializer, "Direction");
            data.WriteObservation<Observation>(editSerializer, "Distance");
            data.WriteFeature<PointFeature>(editSerializer, "From");
            data.WriteItem<bool>(editSerializer, "Default");
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadObservation<Direction>(editDeserializer, "Direction");
            result.ReadObservation<Observation>(editDeserializer, "Distance");
            result.ReadFeature<PointFeature>(editDeserializer, "From");
            result.ReadItem<bool>(editDeserializer, "Default");
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
            m_Direction = data.ExchangeObservation<Direction>(this, "Direction", m_Direction);
            m_Distance = data.ExchangeObservation<Observation>(this, "Distance", m_Distance);
            m_From = data.ExchangeFeature<PointFeature>(this, "From", m_From);
            m_Default = data.ExchangeValue<bool>("Default", m_Default);
        }

        /// <summary>
        /// Creates any new spatial features (without any geometry)
        /// </summary>
        /// <param name="ff">The factory class for generating spatial features</param>
        internal override void ProcessFeatures(FeatureFactory ff)
        {
            m_To = ff.CreatePointFeature("To");
            OffsetPoint op = m_Direction.Offset as OffsetPoint;
            PointFeature from = (op == null ? m_Direction.From : op.Point);

            if (ff.HasFeatureDescription("DirLine"))
                m_DirLine = ff.CreateSegmentLineFeature("DirLine", from, m_To);

            if (ff.HasFeatureDescription("DistLine"))
                m_DistLine = ff.CreateSegmentLineFeature("DistLine", m_From, m_To);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            IPosition to = Calculate(m_Direction, m_Distance, m_From, m_Default);
            PointGeometry pg = PointGeometry.Create(to);
            m_To.ApplyPointGeometry(ctx, pg);

            // There's no need to calculate new geometry for the line segments
            // created by the edit, since their geometry is dependent on the
            // position of the end points - we calculated one end above, the
            // other end should have been calculated by a previous edit.
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WritePersistent<Direction>("Direction", m_Direction);
            editSerializer.WritePersistent<Observation>("Distance", m_Distance);
            editSerializer.WriteFeatureRef<PointFeature>("From", m_From);
            editSerializer.WriteBool("Default", m_Default);
            editSerializer.WritePersistent<FeatureStub>("To", new FeatureStub(m_To));

            if (m_DirLine != null)
                editSerializer.WritePersistent<FeatureStub>("DirLine", new FeatureStub(m_DirLine));

            if (m_DistLine != null)
                editSerializer.WritePersistent<FeatureStub>("DistLine", new FeatureStub(m_DistLine));
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="dir">The observed direction.</param>
        /// <param name="dist">The observed distance (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).</param>
        /// <param name="from">The point the distance was measured from.</param>
        /// <param name="isDefault">True if it was the default intersection (the one closest to the origin of the direction).</param>
        /// <param name="to">The created intersection point.</param>
        /// <param name="dirLine">The first line created (if any).</param>
        /// <param name="distLine">The second line created (if any).</param>
        static void ReadData(EditDeserializer editDeserializer, out Direction dir, out Observation dist, out PointFeature from,
                                out bool isDefault, out FeatureStub to, out FeatureStub dirLine, out FeatureStub distLine)
        {
            dir = editDeserializer.ReadPersistent<Direction>("Direction");
            dist = editDeserializer.ReadPersistent<Observation>("Distance");
            from = editDeserializer.ReadFeatureRef<PointFeature>("From");
            isDefault = editDeserializer.ReadBool("Default");
            to = editDeserializer.ReadPersistent<FeatureStub>("To");
            dirLine = editDeserializer.ReadPersistentOrNull<FeatureStub>("DirLine");
            distLine = editDeserializer.ReadPersistentOrNull<FeatureStub>("DistLine");
        }
    }
}
