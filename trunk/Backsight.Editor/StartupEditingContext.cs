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

namespace Backsight.Editor
{
    /// <summary>
    /// Changes arising while the a <see cref="CadastralMapModel"/> is being loaded (deserialized
    /// from the database).
    /// </summary>
    /// <seealso cref="UpdateEditingContext"/>
    abstract class StartupEditingContext : EditingContext
    {
        /// <summary>
        /// Remembers a modification to the position of a point.
        /// <para/>
        /// This implementation does nothing (any previously assigned geometry is just thrown away).
        /// </summary>
        /// <param name="point">The point that is about to be modified</param>
        internal override void RegisterChange(PointFeature point)
        {
            // Do nothing
        }
    }
}
