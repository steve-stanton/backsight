﻿// <remarks>
// Copyright 2013 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Editor
{
    /// <summary>
    /// Context indicating that the map model is being loaded.
    /// </summary>
    class LoadingContext : EditingContext
    {
        /// <summary>
        /// Remembers a modification to the position of a point.
        /// </summary>
        /// <param name="p"></param>
        internal override void RegisterChange(PointFeature p)
        {
            // Do nothing
        }
    }
}
