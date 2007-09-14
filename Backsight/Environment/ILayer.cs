/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

namespace Backsight.Environment
{
	/// <written by="Steve Stanton" on="12-MAR-2007" />
    /// <summary>A layer of data in a map. Layers may be grouped into
    /// a <see cref="Theme"/></summary>
    public interface ILayer : IEnvironmentItem
    {
        string Name { get; }
        IEntity DefaultPointType { get; }
        IEntity DefaultLineType { get; }
        IEntity DefaultTextType { get; }
        IEntity DefaultPolygonType { get; }

        /// <summary>
        /// The theme that this map layer is part of (null if the layer isn't
        /// associated with a theme).
        /// </summary>
        ITheme Theme { get; }

        /// <summary>
        /// The sequence of this map layer in any associated theme (0 if
        /// the layer isn't associated with a theme). The layer at the base of a
        /// theme has <c>ThemeSequence==1</c>.
        /// </summary>
        int ThemeSequence { get; }
    }
}
