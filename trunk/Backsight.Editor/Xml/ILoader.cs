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

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// Helps with deserialization of XML data (when loading from the database).
    /// </summary>
    interface ILoader
    {
        /// <summary>
        /// Attempts to locate a spatial feature based on its internal ID.
        /// </summary>
        /// <param name="s">The formatted version of an internal ID (as produced
        /// by a prior call to <see cref="InternalIdValue.Format"/>)</param>
        /// <returns>The corresponding feature (null if not found)</returns>
        T Find<T>(string s) where T : Feature;
    }
}
