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
    /// <summary>
    /// Flag bits for the <c>Operation</c> class
    /// </summary>
    [Flags]
    enum OperationFlag : byte
    {
        /// <summary>
        /// Operation has been "touched" by a change. Set either by SetTouch(), or via Touch().
        /// </summary>
        Touched=0x02,

        /// <summary>
        /// Operation still needs to be calculated. Used to determine the
        /// order in which calls to <see cref="Operation.CalculateGeometry"/>
        /// should be made.
        /// </summary>
        ToCalculate=0x04,
    }
}
