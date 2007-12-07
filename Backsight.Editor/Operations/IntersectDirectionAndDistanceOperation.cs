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
    /// <written by="Steve Stanton" on="08-NOV-1997" was="CeIntersectDirDist" />
    /// <summary>
    /// Create point (and optional lines) based on a direction & a distance observation.
    /// </summary>
    class IntersectDirectionAndDistanceOperation : IntersectOperation
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
        /// Creates a new <c>IntersectDirectionAndDistanceOperation</c> with everything set to null.
        /// </summary>
        internal IntersectDirectionAndDistanceOperation()
        {
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
            get { return "Direction-distance intersection"; }
        }

        /// <summary>
        /// Finds the observed length of a line that was created by this operation.
        /// </summary>
        /// <param name="line">The line to find</param>
        /// <returns>The observed length of the line (null if this operation doesn't
        /// reference the specified line)</returns>
        internal override Distance GetDistance(LineFeature line)
        {
            //if (Object.ReferenceEquals(line, m_NewLine))
            //    return m_Length;
            //else
                return null;
        }

        /// <summary>
        /// The features created by this editing operation.
        /// </summary>
        internal override Feature[] Features
        {
            get
            {
                List<Feature> result = new List<Feature>(2);
/*
                if (m_NewPoint!=null)
                    result.Add(m_NewPoint);

                if (m_NewLine!=null)
                    result.Add(m_NewLine);
                */
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
            //m_ExtendLine.AddOp(this);

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
            /*
            // Cut the reference to this op from the line that we extended.
            m_ExtendLine.CutOp(this);

            // Undo the extension point and any extension line
            Rollback(m_NewPoint);
            Rollback(m_NewLine);
            */
            return true;
        }

        /// <summary>
        /// Rollforward this edit in response to some sort of update.
        /// </summary>
        /// <returns>True if operation has been re-executed successfully</returns>
        internal override bool Rollforward()
        {
            /*
            // Return if this operation has not been marked as changed.
            if (!IsChanged)
                return base.OnRollforward();

            // Re-calculate the position of the extension point.
            IPosition xpos = Calculate();

            if (xpos==null)
                throw new RollforwardException(this, "Cannot re-calculate line extension point.");

            // Move the extension point.
            m_NewPoint.Move(xpos);
            */

            // Rollforward the base class.
            return base.OnRollforward();
        }

    }
}
