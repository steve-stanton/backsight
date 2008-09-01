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

namespace Backsight
{
	/// <written by="Steve Stanton" on="22-JUN-07" />
    /// <summary>
    /// A spatial object that refers to a point in space.
    /// </summary>
    public interface IPoint : ISpatialObject
    {
        /// <summary>
        /// The geometry for this point.
        /// </summary>
        IPointGeometry Geometry { get; }
    }
}
