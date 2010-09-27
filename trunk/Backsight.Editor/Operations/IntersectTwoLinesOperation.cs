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

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectLine" />
    /// <summary>
    /// Operation to intersect 2 lines.
    /// </summary>
    class IntersectTwoLinesOperation : IntersectOperation, IRecallable
    {
        #region Class data

        /// <summary>
        /// The 1st line to intersect.
        /// </summary>
        readonly LineFeature m_Line1;

        /// <summary>
        /// True if the 1st line needs to be split at the intersection.
        /// </summary>
        readonly bool m_IsSplit1;

        /// <summary>
        /// The 2nd line to intersect.
        /// </summary>
        readonly LineFeature m_Line2;

        /// <summary>
        /// True if the 2nd line needs to be split at the intersection.
        /// </summary>
        readonly bool m_IsSplit2;

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of
        /// the end points for the 2 lines).
        /// </summary>
        readonly PointFeature m_CloseTo;

        // Creations ...

        /// <summary>
        /// The created intersection point (if any). May have existed previously.
        /// </summary>
        PointFeature m_Intersection;

        // Relating to line splits ...

        /// <summary>
        /// The portion of m_Line1 prior to the intersection (null if m_IsSplit1==false).
        /// </summary>
        LineFeature m_Line1a;

        /// <summary>
        /// The portion of m_Line1 after the intersection (null if m_IsSplit1==false).
        /// </summary>
        LineFeature m_Line1b;

        /// <summary>
        /// The portion of m_Line2 prior to the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2a;

        /// <summary>
        /// The portion of m_Line2 after the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2b;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="IntersectTwoLinesOperation"/> class
        /// </summary>
        /// <param name="session">The session the new instance should be added to</param>
        /// <param name="sequence">The sequence number of the edit within the session (specify 0 if
        /// a new sequence number should be reserved). A non-zero value is specified during
        /// deserialization from the database.</param>
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="wantsplit1">True if 1st line should be split at the intersection.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="wantsplit2">True if 2nd line should be split at the intersection.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        internal IntersectTwoLinesOperation(Session session, uint sequence, LineFeature line1, bool wantSplit1,
                                            LineFeature line2, bool wantSplit2, PointFeature closeTo)
            : base(session, sequence)
        {
            if (line1==null || line2==null || closeTo==null)
                throw new ArgumentNullException();

            m_Line1 = line1;
            m_IsSplit1 = wantSplit1;
            m_Line2 = line2;
            m_IsSplit2 = wantSplit2;
            m_CloseTo = closeTo;
        }

        #endregion

        /// <summary>
        /// The 1st line to intersect.
        /// </summary>
        internal LineFeature Line1 // was GetpArc1
        {
            get { return m_Line1; }
        }

        /// <summary>
        /// True if the 1st line needs to be split at the intersection.
        /// </summary>
        internal bool IsSplit1
        {
            get { return m_IsSplit1; }
            //set { m_IsSplit1 = value; }
        }

        /// <summary>
        /// The 2nd line to intersect.
        /// </summary>
        internal LineFeature Line2 // was GetpArc2
        {
            get { return m_Line2; }
        }

        /// <summary>
        /// True if the 2nd line needs to be split at the intersection.
        /// </summary>
        internal bool IsSplit2
        {
            get { return m_IsSplit2; }
            //set { m_IsSplit2 = value; }
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
        /// The point closest to the intersection (usually defaulted to one of
        /// the end points for the 2 lines).
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
            get { return "Intersect two lines"; }
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(5);

                // The intersection point MIGHT have existed previously.
                if (m_Intersection!=null && m_Intersection.Creator==this)
                    result.Add(m_Intersection);

                if (m_Line1a!=null)
                    result.Add(m_Line1a);

                if (m_Line1b!=null)
                    result.Add(m_Line1b);

                if (m_Line2a!=null)
                    result.Add(m_Line2a);

                if (m_Line2b!=null)
                    result.Add(m_Line2b);

                return result.ToArray();
            }
        }

        /// <summary>
        /// The unique identifier for this edit.
        /// </summary>
        internal override EditingActionId EditId
        {
            get { return EditingActionId.LineIntersect; }
        }

        /// <summary>
        /// Obtains the features that are referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        /// <returns>The referenced features (never null, but may be an empty array).</returns>
        public override Feature[] GetRequiredFeatures()
        {
            return new Feature[]
            {
                m_Line1,
                m_Line2,
                m_CloseTo
            };
        }

        /// <summary>
        /// Rollback this operation (occurs when a user undoes the last edit).
        /// </summary>
        /// <returns>True if operation was rolled back ok</returns>
        internal override bool Undo()
        {
            base.OnRollback();

            // Cut direct refs made by this operation.
            if (m_Line1!=null)
                m_Line1.CutOp(this);

            if (m_Line2!=null)
                m_Line2.CutOp(this);

            if (m_CloseTo!=null)
                m_CloseTo.CutOp(this);

            // Undo the intersect point and any lines
            Rollback(m_Intersection);
            Rollback(m_Line1a);
            Rollback(m_Line1b);
            Rollback(m_Line2a);
            Rollback(m_Line2b);

            // If we actually did splits, re-activate the original line.
            if (m_Line1a!=null || m_Line1b!=null)
            {
                m_Line1a = null;
                m_Line1b = null;
                m_Line1.Restore();
            }

            if (m_Line2a!=null || m_Line2b!=null)
            {
                m_Line2a = null;
                m_Line2b = null;
                m_Line2.Restore();
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
            if (!m_Line1.Intersect(m_Line2, m_CloseTo, out xsect, out closest))
                throw new RollforwardException(this, "Cannot re-calculate intersection point.");

            // Update the intersection point to the new position.
            m_Intersection.MovePoint(uc, xsect);

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
            return (Object.ReferenceEquals(m_Line1, feat) ||
                    Object.ReferenceEquals(m_Line2, feat) ||
                    Object.ReferenceEquals(m_CloseTo, feat));
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
            if (Object.ReferenceEquals(m_Line1a, line) ||
                Object.ReferenceEquals(m_Line1b, line))
                return m_Line1;

            if (Object.ReferenceEquals(m_Line2a, line) ||
                Object.ReferenceEquals(m_Line2b, line))
                return m_Line2;

            return null;
        }

        /// <summary>
        /// Calculates the position of the intersection (if any).
        /// </summary>
        /// <returns>The position of the intersection (null if it cannot be calculated).</returns>
        IPosition Calculate()
        {
            IPosition xsect;
            PointFeature closest;
            if (m_Line1.Intersect(m_Line2, m_CloseTo, out xsect, out closest))
                return xsect;
            else
                return null;
        }

        /// <summary>
        /// Executes this operation.
        /// </summary>
        /// <param name="pointId">The key and entity type to assign to the intersection point.</param>
        internal void Execute(IdHandle pointId)
        {
            FeatureFactory ff = new FeatureFactory(this);

            FeatureId fid = pointId.CreateId();
            IFeature x = new FeatureStub(this, pointId.Entity, fid);
            ff.AddFeatureDescription("To", x);

            if (m_IsSplit1)
            {
                // See FeatureFactory.MakeSection - the only thing that really matters is the
                // session sequence number that will get picked up by the FeatureStub constructor.
                ff.AddFeatureDescription("SplitBefore1", new FeatureStub(this, m_Line1.EntityType, null));
                ff.AddFeatureDescription("SplitAfter1", new FeatureStub(this, m_Line1.EntityType, null));
            }

            if (m_IsSplit2)
            {
                ff.AddFeatureDescription("SplitBefore2", new FeatureStub(this, m_Line2.EntityType, null));
                ff.AddFeatureDescription("SplitAfter2", new FeatureStub(this, m_Line2.EntityType, null));
            }

            base.Execute(ff);

            //////////
            /*
            // Calculate the position of the point of intersection.
            IPosition xsect;
            PointFeature closest;
            if (!m_Line1.Intersect(m_Line2, m_CloseTo, out xsect, out closest))
                throw new Exception("Cannot calculate intersection point");

            // Add the intersection point
            m_Intersection = AddIntersection(xsect, pointId);
            
            // Are we splitting the input lines? If so, do it.
            m_IsSplit1 = wantsplit1;
            if (m_IsSplit1)
                SplitLine(m_Intersection, m_Line1, out m_Line1a, out m_Line1b);

            m_IsSplit2 = wantsplit2;
            if (m_IsSplit2)
                SplitLine(m_Intersection, m_Line2, out m_Line2a, out m_Line2b);

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

            if (m_IsSplit1)
            {
                SectionLineFeature lineBefore1, lineAfter1;
                ff.MakeSections(m_Line1, "SplitBefore1", m_Intersection, "SplitAfter1",
                                    out lineBefore1, out lineAfter1);
                m_Line1a = lineBefore1;
                m_Line1b = lineAfter1;
            }

            if (m_IsSplit2)
            {
                SectionLineFeature lineBefore2, lineAfter2;
                ff.MakeSections(m_Line2, "SplitBefore2", m_Intersection, "SplitAfter2",
                                    out lineBefore2, out lineAfter2);
                m_Line2a = lineBefore2;
                m_Line2b = lineAfter2;
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
            m_Intersection.ApplyPointGeometry(ctx, pg);
        }

        /// <summary>
        /// Updates this operation.
        /// </summary>
        /// <param name="line1">The 1st line to intersect.</param>
        /// <param name="line2">The 2nd line to intersect.</param>
        /// <param name="closeTo">The point the intersection has to be close to. Used if
        /// there is more than one intersection to choose from. If null is specified, a
        /// default point will be selected.</param>
        /// <param name="wantsplit1">True if 1st line should be split at the intersection.</param>
        /// <param name="wantsplit2">True if 2nd line should be split at the intersection.</param>
        /// <returns>True if operation updated ok.</returns>
        internal bool Correct(LineFeature line1, LineFeature line2, PointFeature closeTo,
                                bool wantsplit1, bool wantsplit2)
        {
            throw new NotImplementedException();

            //// Disallow attempts to change the split status

            //if (wantsplit1 != m_IsSplit1 || wantsplit2 != m_IsSplit2)
            //    throw new Exception("You cannot make line splits via update.");

            //// If the lines have changed, cut references to this
            //// operation from the old lines, and change it so the
            //// operation is referenced from the new lines.

            //if (!Object.ReferenceEquals(m_Line1, line1))
            //{
            //    m_Line1.CutOp(this);
            //    m_Line1 = line1;
            //    m_Line1.AddOp(this);
            //}

            //if (!Object.ReferenceEquals(m_Line2, line2))
            //{
            //    m_Line2.CutOp(this);
            //    m_Line2 = line2;
            //    m_Line2.AddOp(this);
            //}

            //if (!Object.ReferenceEquals(m_CloseTo, closeTo))
            //{
            //    if (m_CloseTo != null)
            //        m_CloseTo.CutOp(this);

            //    m_CloseTo = closeTo;

            //    if (m_CloseTo != null)
            //        m_CloseTo.AddOp(this);
            //}

            //return true;
        }

        internal LineFeature Line1BeforeSplit
        {
            get { return m_Line1a; }
            //set { m_Line1a = value; }
        }

        internal LineFeature Line1AfterSplit
        {
            get { return m_Line1b; }
            //set { m_Line1b = value; }
        }

        internal LineFeature Line2BeforeSplit
        {
            get { return m_Line2a; }
            //set { m_Line2a = value; }
        }

        internal LineFeature Line2AfterSplit
        {
            get { return m_Line2b; }
            //set { m_Line2b = value; }
        }
    }
}
