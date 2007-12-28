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

using Backsight.Geometry;
using Backsight.Environment;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="03-DEC-1998" was="CeIntersectDirLine" />
    /// <summary>
    /// Operation to intersect a direction with a line.
    /// </summary>
    [Serializable]
    class IntersectDirectionAndLineOperation : IntersectOperation
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
        /// The portion of m_Line prior to the intersection (0 if m_IsSplit==false).
        /// </summary>
        LineFeature m_LineA;

        /// <summary>
        /// The portion of m_Line after the intersection (0 if m_IsSplit==false).
        /// </summary>
        LineFeature m_LineB;

        /// <summary>
        /// True if the line needs to be split at the intersection.
        /// </summary>
        bool m_IsSplit;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>IntersectDirectionAndLineOperation</c> with everything set to null.
        /// </summary>
        internal IntersectDirectionAndLineOperation()
        {
            m_Direction = null;
            m_Line = null;
            m_CloseTo = null;
            m_Intersection = null;
            m_DirLine = null;
            m_LineA = null;
            m_LineB = null;
            m_IsSplit = false;
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
        /// Adds references to existing features referenced by this operation (including features
        /// that are indirectly referenced by observation classes).
        /// </summary>
        public override void AddReferences()
        {
            m_Line.AddOp(this);
            m_CloseTo.AddOp(this);

            m_Direction.AddReferences(this);
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
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the point of intersection.
            //m_Direction.Intersect( - not implemented
            /*

	CeVertex xsect;
	CePoint* pClosest;

	if ( !m_pDirection->Intersect(*m_pArc,m_pCloseTo,xsect,pClosest) )
		return FALSE;

	// Defective logic means the intersection point may not
	// coincide with the location that's common to split sections,

	const LOGICAL moveSplit = IsSplitAtIntersection(m_pArcA);

	// Update the intersection point to the new position.
	if ( m_pIntersection->Move(xsect) && moveSplit )
	{
		// Perform post-processing of any split sections (this
		// covers a situation where the intersection coincided
		// with a location in a CeMultiSegment)
		SplitPostMove(m_pArcA,m_pArcB,*m_pArc);
	}
             */

            /*
            // Re-calculate the position of the point of intersection.
            IPosition xsect = Calculate(m_Direction, m_Distance, m_From, m_Default);

            if (xsect==null)
                throw new RollforwardException(this, "Cannot re-calculate intersection point.");

            // Update the intersection point to the new position.
            m_To.Move(xsect);
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

        /*
public:
	virtual LOGICAL			GetCircles			( CeObjectList& clist
												, const CePoint& point ) const;
	virtual LOGICAL			Execute				( const CeDirection& dir
												, const CeArc& line
												, const CePoint* const pCloseTo
												, const LOGICAL wantsplit
												, const CeIdHandle& pointId
												, CeEntity* pDirEnt );
	virtual LOGICAL			Execute				( const CeDirection& dir
												, const CeArc& line
												, const CePoint* const pCloseTo
												, const LOGICAL wantsplit
												, const CeEntity* const pPointType
												, CeEntity* pDirEnt );
	virtual LOGICAL			Correct				( const CeDirection& dir
												, const CeArc& line
												, const CePoint* const pCloseTo
												, const LOGICAL wantsplit
												, CeEntity* pDirEnt );
         */
    }
}
