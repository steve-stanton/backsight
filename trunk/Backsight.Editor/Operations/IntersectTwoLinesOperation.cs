/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Collections.Generic;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectLine" />
    /// <summary>
    /// Operation to intersect 2 lines.
    /// </summary>
    [Serializable]
    class IntersectTwoLinesOperation : IntersectOperation
    {
        #region Class data

        /// <summary>
        /// The 1st line to intersect.
        /// </summary>
        LineFeature m_Line1;

        /// <summary>
        /// The 2nd line to intersect.
        /// </summary>
        LineFeature m_Line2;

        /// <summary>
        /// The point closest to the intersection (usually defaulted to one of
        /// the end points for the 2 lines).
        /// </summary>
        PointFeature m_CloseTo;

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
        /// True if the 1st line needs to be split at the intersection.
        /// </summary>
        bool m_IsSplit1;

        /// <summary>
        /// The portion of m_Line2 prior to the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2a;

        /// <summary>
        /// The portion of m_Line2 after the intersection (null if m_IsSplit2==false).
        /// </summary>
        LineFeature m_Line2b;

        /// <summary>
        /// True if the 2nd line needs to be split at the intersection.
        /// </summary>
        bool m_IsSplit2;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectTwoLinesOperation</c> with everything set to null.
        /// </summary>
        internal IntersectTwoLinesOperation()
        {
            m_Line1 = null;
            m_Line2 = null;
            m_CloseTo = null;
            m_Intersection = null;
            m_Line1a = null;
            m_Line1b = null;
            m_Line2a = null;
            m_Line2b = null;
            m_IsSplit1 = false;
            m_IsSplit2 = false;
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
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_Line1.AddOp(this);
            m_Line2.AddOp(this);
            m_CloseTo.AddOp(this);
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
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
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
            m_Intersection.Move(xsect);

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

        /*
	virtual LOGICAL			Execute				( const CeArc& line1
												, const CeArc& line2
												, const CePoint* const pCloseTo
												, const LOGICAL wantsplit1
												, const LOGICAL wantsplit2
												, const CeIdHandle& pointId );
	virtual LOGICAL			Execute				( const CeArc& line1
												, const CeArc& line2
												, const CePoint* const pCloseTo
												, const LOGICAL wantsplit1
												, const LOGICAL wantsplit2
												, const CeEntity* const pPointType );
	virtual	LOGICAL			Correct				( const CeArc& line1
												, const CeArc& line2
												, const CePoint* const pCloseTo
												, const LOGICAL wantsplit1
												, const LOGICAL wantsplit2 );
         */
    }
}
