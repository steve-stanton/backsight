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
using Backsight.Geometry;
using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="10-JAN-1998" was="CeIntersectDist" />
    /// <summary>
    /// Create point (and optional lines) based on 2 distance observations.
    /// </summary>
    class IntersectTwoDistancesOperation : IntersectOperation, IRecallable, IRevisable, IFeatureRef
    {
        #region Class data

        /// <summary>
        /// First observed distance  (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Distance1;

        /// <summary>
        /// The point the 1st distance was measured from.
        /// </summary>
        PointFeature m_From1;

        /// <summary>
        /// Second observed distance  (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).
        /// </summary>
        Observation m_Distance2;

        /// <summary>
        /// The point the 2nd distance was measured from.
        /// </summary>
        PointFeature m_From2;

        /// <summary>
        /// True if it was the default intersection (the one with the lowest bearing
        /// with respect to <see cref="m_From1"/> and <see cref="m_From2"/>).
        /// </summary>
        bool m_Default;

        // Creations ...

        /// <summary>
        /// The created intersection point (if any).
        /// </summary>
        PointFeature m_To;

        /// <summary>
        /// The first created line (if any)
        /// </summary>
        LineFeature m_Line1; // was m_pArc1

        /// <summary>
        /// The second created line (if any)
        /// </summary>
        LineFeature m_Line2; // was m_pArc2

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoDistancesOperation"/> class
        /// </summary>
        /// <param name="dist1">First observed distance  (either a <see cref="Distance"/>, or
        /// an <see cref="OffsetPoint"/>).</param>
        /// <param name="from1">The point the 1st distance was measured from.</param>
        /// <param name="dist2">Second observed distance  (either a <see cref="Distance"/>, or
        /// an <see cref="OffsetPoint"/>).</param>
        /// <param name="from2">The point the 2nd distance was measured from.</param>
        /// <param name="isdefault">True if it was the default intersection (the one with the lowest bearing
        /// with respect to <paramref name="from1"/> and <paramref name="from2"/>).</param>
        internal IntersectTwoDistancesOperation(Observation dist1, PointFeature from1,
                                                Observation dist2, PointFeature from2, bool isdefault)
            : base()
        {
            m_Distance1 = dist1;
            m_From1 = from1;
            m_Distance2 = dist2;
            m_From2 = from2;
            m_Default = isdefault;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoDistancesOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal IntersectTwoDistancesOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            m_Distance1 = editDeserializer.ReadPersistent<Observation>(DataField.Distance1);
            m_From1 = editDeserializer.ReadFeatureRef<PointFeature>(this, DataField.From1);
            m_Distance2 = editDeserializer.ReadPersistent<Observation>(DataField.Distance2);
            m_From2 = editDeserializer.ReadFeatureRef<PointFeature>(this, DataField.From2);
            m_Default = editDeserializer.ReadBool(DataField.Default);
            FeatureStub to = editDeserializer.ReadPersistent<FeatureStub>(DataField.To);
            FeatureStub line1 = editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.Line1);
            FeatureStub line2 = editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.Line2);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub(DataField.To, to);
            dff.AddFeatureStub(DataField.Line1, line1);
            dff.AddFeatureStub(DataField.Line2, line2);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// The first observed distance (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation Distance1
        {
            get { return m_Distance1; }
        }

        /// <summary>
        /// The second observed distance (either a <see cref="Distance"/>, or an <see cref="OffsetPoint"/>).
        /// </summary>
        internal Observation Distance2
        {
            get { return m_Distance2; }
        }

        /// <summary>
        /// The point the 1st distance was measured from.
        /// </summary>
        internal PointFeature Distance1FromPoint // was GetpFrom1
        {
            get { return m_From1; }
        }

        /// <summary>
        /// The point the 2nd distance was measured from.
        /// </summary>
        internal PointFeature Distance2FromPoint // was GetpFrom2
        {
            get { return m_From2; }
        }

        /// <summary>
        /// The first line created (if any).
        /// </summary>
        internal LineFeature CreatedLine1 // was GetpArc1
        {
            get { return m_Line1; }
            set { m_Line1 = value; }
        }

        /// <summary>
        /// The second line created (if any).
        /// </summary>
        internal LineFeature CreatedLine2 // was GetpArc2
        {
            get { return m_Line2; }
            set { m_Line2 = value; }
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
            get { return "Distance - distance intersection"; }
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

                if (m_Line1!=null)
                    result.Add(m_Line1);

                if (m_Line2!=null)
                    result.Add(m_Line2);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.DistIntersect; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();

            result.Add(m_From1);
            result.Add(m_From2);
            result.AddRange(m_Distance1.GetReferences());
            result.AddRange(m_Distance2.GetReferences());

            return result.ToArray();
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        internal override void Undo()
        {
            base.OnRollback();

            // Get rid of the observations.
            m_Distance1.OnRollback(this);
            m_Distance2.OnRollback(this);

            // Cut direct refs made by this operation.
            if (m_From1!=null)
                m_From1.CutOp(this);

            if (m_From2!=null)
                m_From2.CutOp(this);

            // Undo the intersect point and any connecting lines
            Rollback(m_To);
            Rollback(m_Line1);
            Rollback(m_Line2);
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
            ff.AddFeatureDescription(DataField.To, x);

            if (ent1 != null)
            {
                IFeature f = new FeatureStub(this, ent1, null);
                ff.AddFeatureDescription(DataField.Line1, f);
            }

            if (ent2 != null)
            {
                IFeature f = new FeatureStub(this, ent2, null);
                ff.AddFeatureDescription(DataField.Line2, f);
            }

            base.Execute(ff);

            /*
            // Calculate the position of the point of intersection.
            IPosition xsect = Calculate(m_Distance1, m_From1, m_Distance2, m_From2, m_Default);
            if (xsect==null)
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_To = AddIntersection(xsect, pointId);

            // If we have a defined entity types for lines, add them too.
            CadastralMapModel map = MapModel;

            if (ent1!=null)
                m_Line1 = map.AddLine(m_From1, m_To, ent1, this);

            if (ent2!=null)
                m_Line2 = map.AddLine(m_From2, m_To, ent2, this);

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
            m_To = ff.CreatePointFeature(DataField.To);

            if (ff.HasFeatureDescription(DataField.Line1))
                m_Line1 = ff.CreateSegmentLineFeature(DataField.Line1, m_From1, m_To);

            if (ff.HasFeatureDescription(DataField.Line2))
                m_Line2 = ff.CreateSegmentLineFeature(DataField.Line2, m_From2, m_To);

            AssignObservedLengths();
        }

        /// <summary>
        /// Assigns observed lengths to any lines created by this edit.
        /// </summary>
        void AssignObservedLengths()
        {
            if (m_Line1 != null)
                m_Line1.ObservedLength = (m_Distance1 as Distance);

            if (m_Line2 != null)
                m_Line2.ObservedLength = (m_Distance2 as Distance);
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
            return Calculate(m_Distance1, m_From1, m_Distance2, m_From2, m_Default);
        }

        /// <summary>
        /// Calculates the intersection point.
        /// </summary>
        /// <param name="dist1">1st distance observation.</param>
        /// <param name="from1">The point the 1st distance was observed from.</param>
        /// <param name="dist2">2nd distance observation.</param>
        /// <param name="from2">The point the 2nd distance was observed from.</param>
        /// <param name="usedefault">True if the default intersection is required (the one that has the
        /// lowest bearing with respect to the 2 from points). False for the other one (if any).</param>
        /// <returns>The position of the intersection (null if it cannot be calculated).</returns>
        IPosition Calculate(Observation dist1, PointFeature from1, Observation dist2, PointFeature from2, bool usedefault)
        {
            // Call the static function that is also used by the dialog.
            IPosition xsect, x1, x2;
            if (Calculate(dist1, from1, dist2, from2, usedefault, out xsect, out x1, out x2))
                return xsect;
            else
                return null;
        }

        /// <summary>
        /// Calculates intersection points.
        /// </summary>
        /// <param name="dist1">1st distance observation.</param>
        /// <param name="from1">The point the 1st distance was observed from.</param>
        /// <param name="dist2">2nd distance observation.</param>
        /// <param name="from2">The point the 2nd distance was observed from.</param>
        /// <param name="usedefault">True if the default intersection is required (the one that has the
        /// lowest bearing with respect to the 2 from points). False for the other one (if any).</param>
        /// <param name="xsect">The position of the intersection (if any).</param>
        /// <param name="xsect1">The 1st choice intersection (if any).</param>
        /// <param name="xsect2">The 2nd choice intersection (if any).</param>
        /// <returns>True if intersections were calculated. False if the distance circles
        /// don't intersect.</returns>
        internal static bool Calculate(Observation dist1, PointFeature from1, Observation dist2, PointFeature from2, bool usedefault,
                                        out IPosition xsect, out IPosition xsect1, out IPosition xsect2)
        {
            // Initialize intersection positions.
            xsect = xsect1 = xsect2 = null;

            // Get the 2 distances.
            double d1 = dist1.GetDistance(from1).Meters;
            double d2 = dist2.GetDistance(from2).Meters;
            if (d1 < Constants.TINY || d2 < Constants.TINY)
                return false;

            // Form circles with radii that match the observed distances.
            ICircleGeometry circle1 = new CircleGeometry(from1, d1);
            ICircleGeometry circle2 = new CircleGeometry(from2, d2);

            // See if there is actually an intersection between the two circles.
            IPosition x1, x2;
            uint nx = IntersectionHelper.Intersect(circle1, circle2, out x1, out x2);
            if (nx==0)
                return false;

            // If we have 2 intersections, and we need the non-default one, pick up the 2nd
            // intersection. If only 1 intersection, use that, regardless of the setting for
            // the "use default" flag.

            if (nx==2 && !usedefault)
                xsect = x2;
            else
                xsect = x1;

            // Return if both distances are offset points.
            OffsetPoint offset1 = (dist1 as OffsetPoint);
            OffsetPoint offset2 = (dist2 as OffsetPoint);

            if (offset1!=null && offset2!=null)
            {
                xsect1 = x1;
                xsect2 = x2;
                return true;
            }

            // Reduce observed distances to the mapping plane.
            ISpatialSystem sys = CadastralMapModel.Current.SpatialSystem;

            if (offset1==null)
                d1 = d1 * sys.GetLineScaleFactor(from1, xsect);

            if (offset2==null)
                d2 = d2 * sys.GetLineScaleFactor(from2, xsect);

            // And calculate the exact intersection (like above)...
            // Form circles with radii that match the observed distances.
            ICircleGeometry circle1p = new CircleGeometry(from1, d1);
            ICircleGeometry circle2p = new CircleGeometry(from2, d2);

            // See if there is still an intersection between the two circles.
            nx = IntersectionHelper.Intersect(circle1p, circle2p, out x1, out x2);
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
            if (Object.ReferenceEquals(m_From1, feat) ||
                Object.ReferenceEquals(m_From2, feat))
                return true;

            if (m_Distance1.HasReference(feat))
                return true;

            if (m_Distance2.HasReference(feat))
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
        internal UpdateItemCollection GetUpdateItems(Observation dist1, PointFeature from1,
            Observation dist2, PointFeature from2, bool isdefault)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddObservation<Observation>(DataField.Distance1, m_Distance1, dist1);
            result.AddFeature<PointFeature>(DataField.From1, m_From1, from1);
            result.AddObservation<Observation>(DataField.Distance2, m_Distance2, dist2);
            result.AddFeature<PointFeature>(DataField.From2, m_From2, from2);
            result.AddItem<bool>(DataField.Default, m_Default, isdefault);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteObservation<Observation>(editSerializer, DataField.Distance1);
            data.WriteFeature<PointFeature>(editSerializer, DataField.From1);
            data.WriteObservation<Observation>(editSerializer, DataField.Distance2);
            data.WriteFeature<PointFeature>(editSerializer, DataField.From2);
            data.WriteItem<bool>(editSerializer, DataField.Default);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadObservation<Observation>(editDeserializer, DataField.Distance1);
            result.ReadFeature<PointFeature>(editDeserializer, DataField.From1);
            result.ReadObservation<Observation>(editDeserializer, DataField.Distance2);
            result.ReadFeature<PointFeature>(editDeserializer, DataField.From2);
            result.ReadItem<bool>(editDeserializer, DataField.Default);
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
            m_Distance1 = data.ExchangeObservation<Observation>(this, DataField.Distance1, m_Distance1);
            m_From1 = data.ExchangeFeature<PointFeature>(this, DataField.From1, m_From1);
            m_Distance2 = data.ExchangeObservation<Observation>(this, DataField.Distance2, m_Distance2);
            m_From2 = data.ExchangeFeature<PointFeature>(this, DataField.From2, m_From2);
            m_Default = data.ExchangeValue<bool>(DataField.Default, m_Default);

            AssignObservedLengths();
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WritePersistent<Observation>(DataField.Distance1, m_Distance1);
            editSerializer.WriteFeatureRef<PointFeature>(DataField.From1, m_From1);
            editSerializer.WritePersistent<Observation>(DataField.Distance2, m_Distance2);
            editSerializer.WriteFeatureRef<PointFeature>(DataField.From2, m_From2);
            editSerializer.WriteBool(DataField.Default, m_Default);
            editSerializer.WritePersistent<FeatureStub>(DataField.To, new FeatureStub(m_To));

            if (m_Line1 != null)
                editSerializer.WritePersistent<FeatureStub>(DataField.Line1, new FeatureStub(m_Line1));

            if (m_Line2 != null)
                editSerializer.WritePersistent<FeatureStub>(DataField.Line2, new FeatureStub(m_Line2));
        }

        /// <summary>
        /// Ensures that a persistent field has been associated with a spatial feature.
        /// </summary>
        /// <param name="field">A tag associated with the item</param>
        /// <param name="feature">The feature to assign to the field (not null).</param>
        /// <returns>
        /// True if a matching field was processed. False if the field is not known to this
        /// class (may be known to another class in the type hierarchy).
        /// </returns>
        public bool ApplyFeatureRef(DataField field, Feature feature)
        {
            switch (field)
            {
                case DataField.From1:
                    m_From1 = (PointFeature)feature;
                    return true;

                case DataField.From2:
                    m_From2 = (PointFeature)feature;
                    return true;
            }

            return false;
        }
    }
}
