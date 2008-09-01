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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="23-AUG-2007" />
    /// <summary>
    /// Indicates the side of some object of interest with respect to something else (the "subject").
    /// </summary>
    enum Side
    {
        /// <summary>
        /// The side for the object of interest has not been determined.
        /// </summary>
        Unknown=0,

        /// <summary>
        /// The object of interest is to the left of the subject.
        /// </summary>
        Left = 1,

        /// <summary>
        /// The object of interest is to the right of the subject.
        /// </summary>
        Right=2,

        /// <summary>
        /// The object of interest is coincident with the subject.
        /// </summary>
        On = 3
    }
}
