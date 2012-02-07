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

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// Basic information about a feature
    /// </summary>
    /// <remarks>
    /// Implemented by <see cref="Feature"/> and <see cref="FeatureStub"/> 
    /// </remarks>
    interface IFeature
    {
        /// <summary>
        /// The editing operation that created the feature (never null).
        /// </summary>
        Operation Creator { get; }

        /// <summary>
        /// The internal ID of this feature (holds the 1-based creation sequence
        /// of this feature within the project that created it).
        /// </summary>
        InternalIdValue InternalId { get; }

        /// <summary>
        /// The type of real-world object that the feature corresponds to.
        /// </summary>
        IEntity EntityType { get; }

        /// <summary>
        /// The user-perceived ID (if any) for the feature. This is the ID that
        /// is used to associate the feature with any miscellaneous attributes
        /// that may be held in a database.
        /// </summary>
        FeatureId FeatureId { get; }
    }
}
