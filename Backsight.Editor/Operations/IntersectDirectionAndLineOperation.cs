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


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectDirLine" />
    /// <summary>
    /// Operation to intersect a direction with a line.
    /// </summary>
    class IntersectDirectionAndLineOperation : IntersectOperation, IRecallable, IRevisable
    {
        #region Class data

        /// <summary>
        /// The direction observation.
        /// </summary>
        Direction m_Direction;

        /// <summary>
        /// The line the direction needs to intersect.
        /// </summary>
        LineFeature m_Line;

        /// <summary>
        /// True if the line needs to be split at the intersection.
        /// </summary>
        readonly bool m_IsSplit;

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of the end
        /// points for the lines, or the origin of the direction).
        /// </summary>
        PointFeature m_CloseTo;

        // Creations ...

        /// <summary>
        /// The created intersection point (if any). May have existed previously.
        /// </summary>
        PointFeature m_Intersection;

        /// <summary>
        /// Line (if any) that was added along the direction line. Should always be null
        /// if the direction has an offset.
        /// </summary>
        LineFeature m_DirLine;

        // Relating to line split ...

        /// <summary>
        /// The portion of m_Line prior to the intersection (null if m_IsSplit==false).
        /// </summary>
        LineFeature m_LineA;

        /// <summary>
        /// The portion of m_Line after the intersection (null if m_IsSplit==false).
        /// </summary>
        LineFeature m_LineB;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectDirectionAndLineOperation"/> class
        /// </summary>
        /// <param name="dir">Direction observation.</param>
        /// <param name="line">The line to intersect.</param>
        /// <param name="wantsplit">True if line should be split at the intersection.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        internal IntersectDirectionAndLineOperation(Direction dir, LineFeature line, bool wantsplit, PointFeature closeTo)
            : base()
        {
            if (dir==null || line==null || closeTo==null)
                throw new ArgumentNullException();

            m_Direction = dir;
            m_Line = line;
            m_IsSplit = wantsplit;
            m_CloseTo = closeTo;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectDirectionAndLineOperation"/> class
        /// using the data read from persistent storage.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        internal IntersectDirectionAndLineOperation(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
            FeatureStub to, dirLine;
            string idLineA, idLineB;
            ReadData(editDeserializer, out m_Direction, out m_Line, out m_CloseTo,
                            out to, out dirLine, out idLineA, out idLineB);

            m_IsSplit = (idLineA != null && idLineB != null);

            DeserializationFactory dff = new DeserializationFactory(this);
            dff.AddFeatureStub(DataField.To, to);
            dff.AddFeatureStub(DataField.DirLine, dirLine);
            dff.AddLineSplit(m_Line, DataField.SplitBefore, idLineA);
            dff.AddLineSplit(m_Line, DataField.SplitAfter, idLineB);
            ProcessFeatures(dff);
        }

        #endregion

        /// <summary>
        /// The direction observation.
        /// </summary>
        internal Direction Direction
        {
            get { return m_Direction; }
        }

        /// <summary>
        /// Line (if any) that was added along the direction line. Should always be null
        /// if the direction has an offset.
        /// </summary>
        internal LineFeature CreatedDirectionLine // was GetpDirArc
        {
            get { return m_DirLine; }
        }

        /// <summary>
        /// The line the direction needs to intersect.
        /// </summary>
        internal LineFeature Line // was GetpArc
        {
            get { return m_Line; }
        }

        /// <summary>
        /// True if the line needs to be split at the intersection.
        /// </summary>
        internal bool IsSplit
        {
            get { return m_IsSplit; }
        }

        /// <summary>
        /// The point feature at the intersection created by this edit.
        /// </summary>
        internal override PointFeature IntersectionPoint
        {
            get { return m_Intersection; }
        }

        /// <summary>
        /// Was the intersection created at it's default position? Always true.
        /// </summary>
        internal override bool IsDefault
        {
            get { return true; }
        }

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of the end
        /// points for the lines, or the origin of the direction).
        /// For use when relocating the intersection as part of rollforward processing).
        /// </summary>
        internal override PointFeature ClosePoint
        {
            get { return m_CloseTo; }
        }

        /// <summary>
        /// A user-perceived title for this operation.
        /// </summary>
        public override string Name
        {
            get { return "Direction and line intersection"; }
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(4);

                // The intersection point MIGHT have existed previously.
                if (m_Intersection!=null && m_Intersection.Creator==this)
                    result.Add(m_Intersection);

                if (m_DirLine!=null)
                    result.Add(m_DirLine);

                if (m_LineA!=null)
                    result.Add(m_LineA);

                if (m_LineB!=null)
                    result.Add(m_LineB);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.DirLineIntersect; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            List<Feature> result = new List<Feature>();

            result.Add(m_Line);
            result.Add(m_CloseTo);
            result.AddRange(m_Direction.GetReferences());

            return result.ToArray();
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Get rid of the observation
            m_Direction.OnRollback(this);

            // Cut direct refs made by this operation.
            if (m_Line!=null)
                m_Line.CutOp(this);

            if (m_CloseTo!=null)
                m_CloseTo.CutOp(this);

            // Undo the intersect point and any lines
            Rollback(m_Intersection);
            Rollback(m_DirLine);
            Rollback(m_LineA);
            Rollback(m_LineB);

            // If we actually did a split, re-activate the original line.
            if (m_LineA!=null || m_LineB!=null)
            {
                m_LineA = null;
                m_LineB = null;
                m_Line.Restore();
            }

            return true;
        }

        /// <summary>
        /// Checks whether this operation makes reference to a specific feature.
        /// </summary>
        /// <param name="feat">The feature to check for.</param>
        /// <returns>True if this edit depends on (contains a reference to) the supplied feature</returns>
        bool HasReference(Feature feat)
        {
            if (Object.ReferenceEquals(m_Line, feat) || Object.ReferenceEquals(m_CloseTo, feat))
                return true;

            if (m_Direction.HasReference(feat))
                return true;

            return false;
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
            if (Object.ReferenceEquals(m_LineA, line) ||
                Object.ReferenceEquals(m_LineB, line))
                return m_Line;

            return null;
        }

        /// <summary>
        /// Calculates the position of the intersection
        /// </summary>
        /// <returns>The calculated position (null if the direction doesn't intersect the line)</returns>
        IPosition Calculate()
        {
            IPosition xsect;
            PointFeature closest;
            if (m_Direction.Intersect(m_Line, m_CloseTo, out xsect, out closest))
                return xsect;
            else
                return null;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="pointId">The key and entity type to assign to the intersection point.</param>
        /// <param name="dirEnt">The entity type for any line that should be added along the direction
        /// line. Specify null if you don't want a line.</param>
        internal void Execute(IdHandle pointId, IEntity dirEnt)
        {
            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature x = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription(DataField.To, x);

            if (m_IsSplit)
            {
                // See FeatureFactory.MakeSection - the only thing that really matters is the
                // session sequence number that will get picked up by the FeatureStub constructor.
                ff.AddFeatureDescription(DataField.SplitBefore, new FeatureStub(this, m_Line.EntityType, null));
                ff.AddFeatureDescription(DataField.SplitAfter, new FeatureStub(this, m_Line.EntityType, null));                
            }

            if (dirEnt != null)
            {
                // Lines are not allowed if the direction line is associated with an offset
                // distance (since we would then need to add a point at the start of the
                // direction line). This should have been trapped by the UI. Note that an
                // offset specified using an OffsetPoint is valid.

                if (m_Direction.Offset is OffsetDistance)
                    throw new ApplicationException("Cannot add direction line because a distance offset is involved");

                IFeature f = new FeatureStub(this, dirEnt, null);
                ff.AddFeatureDescription(DataField.DirLine, f);
            }

            base.Execute(ff);

            //////////
            /*
            // Calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!m_Direction.Intersect(m_Line, m_CloseTo, out xsect, out closest))
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_Intersection = AddIntersection(xsect, pointId);

            // Are we splitting the input line? If so, do it.
            m_IsSplit = wantsplit;
            if (m_IsSplit)
                SplitLine(m_Intersection, m_Line, out m_LineA, out m_LineB);

            // If we have a defined entity type for the direction line, add a line too.
            CadastralMapModel map = MapModel;
            if (dirEnt!=null)
                m_DirLine = map.AddLine(m_Direction.From, m_Intersection, dirEnt, this);

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
            m_Intersection = ff.CreatePointFeature(DataField.To);

            if (m_IsSplit)
            {
                LineFeature lineBefore, lineAfter;
                ff.MakeSections(m_Line, DataField.SplitBefore, m_Intersection, DataField.SplitAfter,
                                    out lineBefore, out lineAfter);
                m_LineA = lineBefore;
                m_LineB = lineAfter;
            }

            OffsetPoint op = m_Direction.Offset as OffsetPoint;
            PointFeature from = (op == null ? m_Direction.From : op.Point);

            if (ff.HasFeatureDescription(DataField.DirLine))
                m_DirLine = ff.CreateSegmentLineFeature(DataField.DirLine, from, m_Intersection);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        /// <param name="ctx">The context in which the geometry is being calculated.</param>
        internal override void CalculateGeometry(EditingContext ctx)
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_Intersection.ApplyPointGeometry(ctx, pg);
        }

        ///// <summary>
        ///// Executes this operation.
        ///// </summary>
        ///// <param name="wantsplit">True if line should be split at the intersection.</param>
        ///// <param name="pointType">The entity type to assign to the intersection point.</param>
        ///// <param name="dirEnt">The entity type for any line that should be added along the direction
        ///// line. Specify null if you don't want a line.</param>
        //internal void Execute(bool wantsplit, IEntity pointType, IEntity dirEnt)
        //{
        //    // Calculate the position of the point of intersection.
        //    IPosition xsect;
        //    PointFeature closest;
        //    if (!m_Direction.Intersect(m_Line, m_CloseTo, out xsect, out closest))
        //        throw new Exception("Cannot calculate intersection point");

        //    // Add the intersection point
        //    m_Intersection = AddIntersection(xsect, pointType);

        //    // Are we splitting the input line? If so, do it.
        //    m_IsSplit = wantsplit;
        //    if (m_IsSplit)
        //        SplitLine(m_Intersection, m_Line, out m_LineA, out m_LineB);

        //    // If we have a defined entity type for the direction line, add a line too.
        //    CadastralMapModel map = MapModel;
        //    if (dirEnt != null)
        //        m_DirLine = map.AddLine(m_Direction.From, m_Intersection, dirEnt, this);

        //    // Peform standard completion steps
        //    Complete();
        //}

        /// <summary>
        /// Obtains update items for a revised version of this edit
        /// (for later use with <see cref="ExchangeData"/>).
        /// </summary>
        /// <param name="dir">The direction to intersect.</param>
        /// <param name="line">The line to intersect.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        /// <returns>The items representing the change (may be subsequently supplied to
        /// the <see cref="ExchangeUpdateItems"/> method).</returns>
        internal UpdateItemCollection GetUpdateItems(Direction dir, LineFeature line,
                                                        PointFeature closeTo)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.AddObservation<Direction>(DataField.Direction, m_Direction, dir);
            result.AddFeature<LineFeature>(DataField.Line, m_Line, line);
            result.AddFeature<PointFeature>(DataField.CloseTo, m_CloseTo, closeTo);
            return result;
        }

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        public void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data)
        {
            data.WriteObservation<Direction>(editSerializer, DataField.Direction);
            data.WriteFeature<LineFeature>(editSerializer, DataField.Line);
            data.WriteFeature<PointFeature>(editSerializer, DataField.CloseTo);
        }

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        public UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer)
        {
            UpdateItemCollection result = new UpdateItemCollection();
            result.ReadObservation<Direction>(editDeserializer, DataField.Direction);
            result.ReadFeature<LineFeature>(editDeserializer, DataField.Line);
            result.ReadFeature<PointFeature>(editDeserializer, DataField.CloseTo);
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
            m_Direction = data.ExchangeObservation<Direction>(this, DataField.Direction, m_Direction);
            m_Line = data.ExchangeFeature<LineFeature>(this, DataField.Line, m_Line);
            m_CloseTo = data.ExchangeFeature<PointFeature>(this, DataField.CloseTo, m_CloseTo);
        }

        internal LineFeature LineBeforeSplit
        {
            get { return m_LineA; }
        }

        internal LineFeature LineAfterSplit
        {
            get { return m_LineB; }
        }

        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        public override void WriteData(EditSerializer editSerializer)
        {
            base.WriteData(editSerializer);

            editSerializer.WritePersistent<Direction>(DataField.Direction, m_Direction);
            editSerializer.WriteFeatureRef<LineFeature>(DataField.Line, m_Line);
            editSerializer.WriteFeatureRef<PointFeature>(DataField.CloseTo, m_CloseTo);
            editSerializer.WritePersistent<FeatureStub>(DataField.To, new FeatureStub(m_Intersection));

            if (m_DirLine != null)
                editSerializer.WritePersistent<FeatureStub>(DataField.DirLine, new FeatureStub(m_DirLine));

            if (m_LineA != null)
                editSerializer.WriteInternalId(DataField.SplitBefore, m_LineA.InternalId);

            if (m_LineB != null)
                editSerializer.WriteInternalId(DataField.SplitAfter, m_LineB.InternalId);
        }

        /// <summary>
        /// Reads data that was previously written using <see cref="WriteData"/>
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <param name="dir">The direction observation.</param>
        /// <param name="line">The line the direction needs to intersect.</param>
        /// <param name="closeTo">The point closest to the intersection.</param>
        /// <param name="to">The created intersection point. May have existed previously.</param>
        /// <param name="dirLine">Line (if any) that was added along the direction line.</param>
        /// <param name="idLineA">The ID of the portion of the intersected line prior to the intersection (null if no split).</param>
        /// <param name="idLineB">The ID of the portion of the intersected line after the intersection (null if no split).</param>
        static void ReadData(EditDeserializer editDeserializer, out Direction dir, out LineFeature line, out PointFeature closeTo,
                                out FeatureStub to, out FeatureStub dirLine, out string idLineA, out string idLineB)
        {
            dir = editDeserializer.ReadPersistent<Direction>(DataField.Direction);
            line = editDeserializer.ReadFeatureRef<LineFeature>(DataField.Line);
            closeTo = editDeserializer.ReadFeatureRef<PointFeature>(DataField.CloseTo);
            to = editDeserializer.ReadPersistent<FeatureStub>(DataField.To);
            dirLine = editDeserializer.ReadPersistentOrNull<FeatureStub>(DataField.DirLine);
            idLineA = (editDeserializer.IsNextField(DataField.SplitBefore) ? editDeserializer.ReadString(DataField.SplitBefore) : null);
            idLineB = (editDeserializer.IsNextField(DataField.SplitAfter) ? editDeserializer.ReadString(DataField.SplitAfter) : null);
        }
    }
}
