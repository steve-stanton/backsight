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
    /// An item of text with <see cref="MiscTextGeometry"/>.
    /// </summary>
    class MiscTextFeature : TextFeature
    {
        internal MiscTextFeature(IEntity ent, Operation creator, MiscTextGeometry text)
            : base(ent, creator, text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MiscTextFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="iid">The internal ID for the feature.</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature. If not null,
        /// this will be modified by cross-referencing it to the newly created feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="geom">The metrics for the text (including the text itself).</param>
        /// <param name="isTopological">Is the new feature expected to act as a polygon label?</param>
        /// <param name="polPosition">The position of the polygon reference position (specify null
        /// if the feature is not a polygon label)</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal MiscTextFeature(InternalIdValue iid, FeatureId fid, IEntity ent, Operation creator,
            MiscTextGeometry geom, bool isTopological, PointGeometry polPosition)
            : base(iid, fid, ent, creator, geom, isTopological, polPosition)
        {
        }

        /// <summary>
        /// A value indicating the type of geometry used to represent this feature.
        /// </summary>
        internal override FeatureGeometry Representation
        {
            get { return FeatureGeometry.MiscText; }
        }
    }
}
