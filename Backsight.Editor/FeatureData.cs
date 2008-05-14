/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using System.Runtime.Serialization;

namespace Backsight.Editor
{
    /// <summary>
    /// Data class for a new feature, excluding the geometry (you use this class
    /// if the geometry will be calculated on-the-fly).
    /// </summary>
    [Serializable]
    class FeatureData
    {
        /// <summary>
        /// The 1-based creation sequence of this feature within the creating edit.
        /// </summary>
        public int CreationSequence;

        /// <summary>
        /// The ID of the entity type assigned to the feature
        /// </summary>
        public int EntityId;

        /// <summary>
        /// The user-perceived ID for the feature (if any)
        /// </summary>
        [OptionalField]
        public string Key;
    }
}
