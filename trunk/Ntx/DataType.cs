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

namespace Ntx
{
    /// <summary>
    /// The NTX data types supported by this package.
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// Undefined data type.
        /// </summary>
        None = -1,

        /// <summary>
        /// Packed lines (delta XY format)
        /// </summary>
		DeltaLine = 1,

        /// <summary>
        /// Point-to-point lines
        /// </summary>
		Line = 3,

        /// <summary>
        /// Names (act as polygon labels if marked as topological)
        /// </summary>
		Name = 7,

        /// <summary>
        /// Point symbols.
        /// </summary>
		Symbol = 8,
    }
}
