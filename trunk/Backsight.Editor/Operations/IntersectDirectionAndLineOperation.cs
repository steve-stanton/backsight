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

using Backsight.Geometry;
using Backsight.Environment;
using Backsight.Editor.Observations;


namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectDirLine" />
    /// <summary>
    /// Operation to intersect a direction with a line.
    /// </summary>
    class IntersectDirectionAndLineOperation : IntersectOperation, IRecallable
    {
        #region Class data

        /// <summary>
        /// The direction observation.
        /// </summary>
        readonly Direction m_Direction;

        /// <summary>
        /// The line the direction needs to intersect.
        /// </summary>
        readonly LineFeature m_Line;

        /// <summary>
        /// True if the line needs to be split at the intersection.
        /// </summary>
        readonly bool m_IsSplit;

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of the end
        /// points for the lines, or the origin of the direction).
        /// </summary>
        readonly PointFeature m_CloseTo;

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
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="dir">Direction observation.</param>
        /// <param name="line">The line to intersect.</param>
        /// <param name="wantsplit">True if line should be split at the intersection.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        internal IntersectDirectionAndLineOperation(Session session, uint sequence, Direction dir,
                                                    LineFeature line, bool wantsplit, PointFeature closeTo)
            : base(session, sequence)
        {
            if (dir==null || line==null || closeTo==null)
                throw new ArgumentNullException();

            m_Direction = dir;
            m_Line = line;
            m_IsSplit = wantsplit;
            m_CloseTo = closeTo;
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
        public override Feature[] GetReferences()
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
            IPosition xsect;
            PointFeature closest;
            if (!m_Direction.Intersect(m_Line, m_CloseTo, out xsect, out closest))
                throw new RollforwardException(this, "Cannot re-calculate intersection point.");

            // Update the intersection point to the new position.
            m_Intersection.MovePoint(uc, xsect);

                /*
                // Defective logic means the intersection point may not
                // coincide with the location that's common to split sections
                // TODO: Looks flakey. Is this worth doing?

                bool moveSplit = IsSplitAtIntersection(m_LineA);

                // Update the intersection point to the new position.
                if (m_Intersection.Move(xsect) && moveSplit)
                {
                    // Perform post-processing of any split sections (this
                    // covers a situation where the intersection coincided
                    // with a location in a CeMultiSegment)
                    SplitPostMove(m_LineA, m_LineB, m_Line);
                }
                */

            // Rollforward the base class.
            return base.OnRollforward();
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
            ff.AddFeatureDescription("To", x);

            if (m_IsSplit)
            {
                // See FeatureFactory.MakeSection - the only thing that really matters is the
                // session sequence number that will get picked up by the FeatureStub constructor.
                ff.AddFeatureDescription("SplitBefore", new FeatureStub(this, m_Line.EntityType, null));
                ff.AddFeatureDescription("SplitAfter", new FeatureStub(this, m_Line.EntityType, null));                
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
                ff.AddFeatureDescription("DirLine", f);
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
            m_Intersection = ff.CreatePointFeature("To");

            if (m_IsSplit)
            {
                SectionLineFeature lineBefore, lineAfter;
                ff.MakeSections(m_Line, "SplitBefore", m_Intersection, "SplitAfter",
                                    out lineBefore, out lineAfter);
                m_LineA = lineBefore;
                m_LineB = lineAfter;
            }

            OffsetPoint op = m_Direction.Offset as OffsetPoint;
            PointFeature from = (op == null ? m_Direction.From : op.Point);

            if (ff.HasFeatureDescription("DirLine"))
                m_DirLine = ff.CreateSegmentLineFeature("DirLine", from, m_Intersection);
        }

        /// <summary>
        /// Performs the data processing associated with this editing operation.
        /// </summary>
        internal override void CalculateGeometry()
        {
            IPosition p = Calculate();
            PointGeometry pg = PointGeometry.Create(p);
            m_Intersection.SetPointGeometry(pg);
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
        /// Updates this operation.
        /// </summary>
        /// <param name="dir">The direction to intersect.</param>
        /// <param name="line">The line to intersect.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        /// <param name="wantsplit">True if line should be split at the intersection.</param>
        /// <param name="dirEnt">The entity type for any line that should be added along the direction
        /// line. Specify null if you don't want a line.</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct(Direction dir, LineFeature line, PointFeature closeTo,
                                bool wantsplit, IEntity dirEnt)
        {
            throw new NotImplementedException();
            /*
            // Calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!dir.Intersect(line, closeTo, out xsect, out closest))
                return false;

            // The following check was originally done only by the Correct
            // function, but it's a bit late by then. Note that we will
            // allow a change only if the user has also specified a new line
            // to intersect with.

            if (m_IsSplit && !wantsplit && Object.ReferenceEquals(m_Line, line))
                throw new Exception("You cannot change line splits via update.");

            // If the line has changed, cut reference to this
            // operation from the old line, and change it so the
            // operation is referenced from the new line.

            if (!Object.ReferenceEquals(m_Line, line))
            {
                m_Line.CutOp(this);
                m_Line = line;
                m_Line.AddOp(this);
            }

            if (!Object.ReferenceEquals(m_CloseTo, closeTo))
            {
                if (m_CloseTo != null)
                    m_CloseTo.CutOp(this);

                m_CloseTo = closeTo;

                if (m_CloseTo != null)
                    m_CloseTo.AddOp(this);
            }

            // Cut the references made by the direction object. If nothing
            // has changed, the references will be re-inserted when the
            // direction is re-saved below.
            m_Direction.CutRef(this);

            // Get rid of the previously defined observation, and replace
            // with the new one (we can't necessarily change the old one
            // because we may have changed the type of observation).

            m_Direction.OnRollback(this);
            m_Direction = dir;
            m_Direction.AddReferences(this);

            // See if the split status has changed.
            // The following is really junky and should be changed...

            if (wantsplit != m_IsSplit)
                throw new Exception("You cannot make line splits via update.");

            // If we have defined entity types for lines, and we did not
            // have a line before, add a new line now.

            if (dirEnt != null)
            {
                if (m_DirLine == null)
                {
                    IPosition from = m_Direction.StartPosition;
                    CadastralMapModel map = MapModel;
                    PointFeature p = map.EnsurePointExists(from, this);
                    m_DirLine = MapModel.AddLine(p, m_Intersection, dirEnt, this);
                }
                else
                {
                    if (m_DirLine.EntityType.Id != dirEnt.Id)
                        throw new Exception("Cannot change entity type via update.");
                }
            }
            else if (m_DirLine!=null)
                throw new Exception("You cannot delete lines via update. Use Line Delete.");

            return true;
             */
        }

        internal LineFeature LineBeforeSplit
        {
            get { return m_LineA; }
        }

        internal LineFeature LineAfterSplit
        {
            get { return m_LineB; }
        }
    }
}
