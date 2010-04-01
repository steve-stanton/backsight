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

using Backsight.Geometry;
using Backsight.Environment;
using Backsight.Editor.Observations;
using Backsight.Editor.Xml;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersectDirDist" />
    /// <summary>
    /// Create point (and optional lines) based on a direction and a distance observation.
    /// </summary>
    class IntersectDirectionAndDistanceOperation : IntersectOperation, IRecallable
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
        /// Constructor for use during deserialization. The point created by this edit
        /// is defined without any geometry. A subsequent call to <see cref="CalculateGeometry"/>
        /// is needed to define the geometry.
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        /// <param name="t">The serialized version of this instance</param>
        internal IntersectDirectionAndDistanceOperation(Session s, IntersectDirectionAndDistanceData t)
            : base(s, t)
        {
            m_From = s.MapModel.Find<PointFeature>(t.From);
            m_Default = t.Default;
            m_Direction = (Direction)t.Direction.LoadObservation(this);
            m_Distance = t.Distance.LoadObservation(this);
            m_To = new PointFeature(this, t.To);

            if (t.DirLine == null)
                m_DirLine = null;
            else
                m_DirLine = new LineFeature(this, m_Direction.From, m_To, t.DirLine);

            if (t.DistLine == null)
                m_DistLine = null;
            else
                m_DistLine = new LineFeature(this, m_From, m_To, t.DistLine);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectDirectionAndDistanceOperation"/> class
        /// </summary>
        /// <param name="s">The session the new instance should be added to</param>
        internal IntersectDirectionAndDistanceOperation(Session s)
            : base(s)
        {
            m_Direction = null;
            m_Distance = null;
            m_From = null;
            m_Default = true;

            m_To = null;
            m_DirLine = null;
            m_DistLine = null;
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
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_From.AddOp(this);

            m_Direction.AddReferences(this);
            m_Distance.AddReferences(this);
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
            IPosition xsect = Calculate(m_Direction, m_Distance, m_From, m_Default);

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
        /// <param name="dir">Direction observation.</param>
        /// <param name="dist">Distance observation.</param>
        /// <param name="from">The point the distance was observed from.</param>
        /// <param name="usedefault">True if the default intersection is required (the one 
        /// closer to the origin of the direction line). False for the other one (if any).</param>
        /// <param name="pointId">The ID and entity type for the intersect point</param>
        /// <param name="ent1">The entity type for 1st line (null for no line)</param>
        /// <param name="ent2">The entity type for 2nd line (null for no line)</param>
        internal void Execute(Direction dir, Observation distance, PointFeature from, bool isdefault,
                                IdHandle pointId, IEntity ent1, IEntity ent2)
        {
            // Calculate the position of the point of intersection.
            IPosition xsect = Calculate(dir, distance, from, isdefault);
            if (xsect==null)
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_To = AddIntersection(xsect, pointId);

            // Remember input
            m_Direction = dir;
            m_Distance = distance;
            m_From = from;
            m_Default = isdefault;

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
            ICoordinateSystem sys = CadastralMapModel.Current.CoordinateSystem;
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
        /// Updates this operation. 
        /// </summary>
        /// <param name="dist1">1st distance observation.</param>
        /// <param name="from1">The point the 1st distance was observed from.</param>
        /// <param name="dist2">2nd distance observation.</param>
        /// <param name="from2">The point the 2nd distance was observed from.</param>
        /// <param name="isdefault">True if the default intersection is required (the one that has the
        /// lowest bearing with respect to the 2 from points). False for the other one (if any).</param>
        /// <param name="ent1">The entity type for 1st line (null for no line)</param>
        /// <param name="ent2">The entity type for 2nd line (null for no line)</param>
        /// <returns></returns>
        internal bool Correct(Direction dir, Observation distance, PointFeature from, bool isdefault,
                        IEntity ent1, IEntity ent2)
        {
            if ((ent1==null && m_DirLine!=null) || (ent2==null && m_DistLine!=null))
                throw new Exception("You cannot delete lines via update. Use Line Delete.");

            // Calculate the position of the point of intersection.
            IPosition xsect = Calculate(dir, distance, from, isdefault);
            if (xsect==null)
                return false;

            // If the point the distance was observed from has changed, cut
            // the reference the old point makes to this op, and change it
            // so the operation is referenced by the new point.

            if (!Object.ReferenceEquals(m_From, from))
            {
                m_From.CutOp(this);
                m_From = from;
                m_From.AddOp(this);
            }

            // Cut the references made by the direction object. If nothing
            // has changed, the references will be re-inserted when the
            // direction is re-saved below.
            m_Direction.CutRef(this);

            // Get rid of the previously defined observations, and replace
            // with the new ones (we can't necessarily change the old ones
            // because we may have changed the type of observation).

            m_Direction.OnRollback(this);
            m_Distance.OnRollback(this);

            m_Direction = dir;
            m_Direction.AddReferences(this);

            m_Distance = distance;
            m_Distance.AddReferences(this);

            // Save option about whether we want default intersection or not.
            m_Default = isdefault;

            // If we have defined entity types for lines, and we did not
            // have a line before, add a new line now.

            if (ent1!=null)
            {
                if (m_DirLine==null)
                {
                    CadastralMapModel map = MapModel;
                    IPosition start = m_Direction.StartPosition;
                    PointFeature ps = map.EnsurePointExists(start, this);
                    m_DirLine = map.AddLine(ps, m_To, ent1, this);
                }
                else if (m_DirLine.EntityType.Id != ent1.Id)
                    throw new NotImplementedException("IntersectDirectionAndDistancesOperation.Correct");
                    //m_DirLine.EntityType = ent1;
            }

            if (ent2!=null)
            {
                if (m_DistLine==null)
                    MapModel.AddLine(m_From, m_To, ent2, this);
                else
                    throw new NotImplementedException("IntersectDirectionAndDistancesOperation.Correct");
                    //m_DistLine.EntityType = ent2;
            }

            return true;
        }

        /// <summary>
        /// Returns an object that represents this edit, and that can be serialized using
        /// the <c>XmlSerializer</c> class.
        /// <returns>The serializable version of this edit</returns>
        internal override OperationData GetSerializableEdit()
        {
            IntersectDirectionAndDistanceData t = new IntersectDirectionAndDistanceData();
            base.SetSerializableEdit(t);

            t.From = m_From.DataId;
            t.Default = m_Default;
            t.Direction = (DirectionData)m_Direction.GetSerializableObservation();
            t.Distance = m_Distance.GetSerializableObservation();
            t.To = new CalculatedFeatureData(m_To);

            if (m_DirLine != null)
                t.DirLine = new CalculatedFeatureData(m_DirLine);

            if (m_DistLine != null)
                t.DistLine = new CalculatedFeatureData(m_DistLine);

            return t;
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void RunEdit()
        {
            IPosition to = Calculate(m_Direction, m_Distance, m_From, m_Default);
            PointGeometry pg = PointGeometry.Create(to);
            m_To.PointGeometry = pg;

            if (m_DirLine!=null)
                m_DirLine.LineGeometry = new SegmentGeometry(m_Direction.From, m_To);

            if (m_DistLine!=null)
                m_DistLine.LineGeometry = new SegmentGeometry(m_From, m_To);
        }
    }
}
