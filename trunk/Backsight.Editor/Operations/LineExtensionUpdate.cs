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
using Backsight.Editor.Observations;

namespace Backsight.Editor.Operations
{
    /// <written by="Steve Stanton" on="14-MAY-2009" />
    /// <summary>
    /// Information about an update to an instance of <see cref="LineExtensionOperation"/>
    /// </summary>
    class LineExtensionUpdate : OperationUpdate
    {
        #region Class data

        /// <summary>
        /// The observed length of the extension.
        /// </summary>
        Distance m_Length;

        /// <summary>
        /// True if extending from the end of the line.
        /// False if extending from the start.
        /// </summary>
        bool m_IsExtendFromEnd;        

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="LineExtensionUpdate"/> class
        /// </summary>
        internal LineExtensionUpdate(LineExtensionOperation op, Distance length, bool isExtendFromEnd)
            : base(op)
        {
            m_Length = length;
            m_IsExtendFromEnd = isExtendFromEnd;
        }

        #endregion
    }
}
