// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
using System.Diagnostics;


namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="11-MAY-2009" />
    /// <summary>
    /// Information about a feature that gets moved as part of an editing revision.
    /// Instances of this class are retained as part of the <see cref="UpdateContext"/> class.
    /// </summary>
    class Move
    {
        #region Class data

        /// <summary>
        /// The moved feature. This will most probably be an instance of <see cref="PointFeature"/>,
        /// possibly a <see cref="TextFeature"/>. Should <b>not</b> be an instance of <see cref="LineFeature"/>
        /// since lines are moved by moving their end points.
        /// </summary>
        readonly Feature m_Feature;

        /// <summary>
        /// The original position of the feature (not null)
        /// </summary>
        readonly PointGeometry m_OldPosition;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Move"/> class, with an "old" position that
        /// corresponds to it's current position.
        /// </summary>
        /// <param name="feature">The point feature that is being moved.</param>
        internal Move(PointFeature feature)
        {
            m_Feature = feature;
            m_OldPosition = new PointGeometry(feature.PointGeometry);
        }

        #endregion

        /// <summary>
        /// The original position of the feature (not null)
        /// </summary>
        PointGeometry OldPosition
        {
            get { return m_OldPosition; }
        }

        /// <summary>
        /// Undoes this move
        /// </summary>
        internal void Undo()
        {
            bool moveUndone = m_Feature.UndoMove(m_OldPosition);
            Debug.Assert(moveUndone);
        }
    }
}
