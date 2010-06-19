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
    /// A point that shares the same location as another point.
    /// </summary>
    class SharedPointFeature : PointFeature
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedPointFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="iid">The internal ID for the feature.</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature. If not null,
        /// this will be modified by cross-referencing it to the newly created feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="firstPoint">The point feature that the new point coincides with (not null)</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> or <paramref name="firstPoint"/> is null.</exception>
        internal SharedPointFeature(InternalIdValue iid, FeatureId fid, IEntity ent, Operation creator, PointFeature firstPoint)
            : base(iid, fid, ent, creator, firstPoint)
        {
        }

        #endregion

        /// <summary>
        /// The point that defines the position that is shared by this point.
        /// </summary>
        internal PointFeature FirstPoint
        {
            get
            {
                Node n = base.Node;
                return (n == null ? null : n.FirstPoint);
            }
        }
    }
}
