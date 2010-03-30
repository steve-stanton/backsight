// <remarks>
// Copyright 2010 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Input data for <see cref="AttachPointOperation"/>
    /// </summary>
    class AttachPointInput : OperationInput
    {
        #region Class data

        /// <summary>
        /// The line the attached point should appear on (not null).
        /// </summary>
        internal readonly LineFeature Line;

        /// <summary>
        /// The position ratio of the attached point. A point coincident with the start
        /// of the line is a value of 0. A point at the end of the line is a value of
        /// 1 billion  (1,000,000,000).
        /// </summary>
        internal readonly uint PositionRatio;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachPointInput"/> class
        /// </summary>
        /// <param name="line">The line the attached point should appear on (not null).</param>
        /// <param name="positionRatio">The position ratio of the attached point. A point
        /// coincident with the start of the line is a value of 0. A point at the end of the
        /// line is a value of 1 billion  (1,000,000,000).</param>
        /// <exception cref="ArgumentNullException">If the supplied line is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the supplied position ratio
        /// is greater than 1 billion.</exception>
        internal AttachPointInput(LineFeature line, uint positionRatio)
        {
            if (line == null)
                throw new ArgumentNullException();

            if (positionRatio > 1000000000)
                throw new ArgumentOutOfRangeException();

            Line = line;
            PositionRatio = positionRatio;
        }

        #endregion
    }
}
