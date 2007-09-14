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
    /// <summary>
    /// A series of related map layers (see <see cref="ILayer"/>). For example, a theme
    /// called "Property" might be associated with survey, ownership, and assessment
    /// layers. Layers that have been grouped into a theme are organized into a simple
    /// inheritance hierarchy in which data is shared across layers.
    /// <para/>
    /// Thus, if you add a new feature on the survey layer (part of the property theme),
    /// it will automatically appear on the derived ownership and assessment layers.
    /// If you later edit that feature while working with one of the derived layers,
    /// the edits apply to that layer plus layers derived from it. For example,
    /// if you add a feature on the survey layer, and you later delete that feature
    /// while working with the ownership layer, it will be removed from ownership
    /// and the derived assessment layers; however, it will be retained on the survey
    /// layer.
    /// </summary>
    public interface ITheme : IEnvironmentItem
    {
        /// <summary>
        /// The name of the theme. All distinct themes must have a unique name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The layers associated with the theme, ordered so that the base layer
        /// comes first.
        /// </summary>
        ILayer[] Layers { get; }
    }
}
