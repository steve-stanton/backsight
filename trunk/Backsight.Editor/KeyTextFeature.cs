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
    /// An item of text with <see cref="KeyTextGeometry"/>.
    /// </summary>
    class KeyTextFeature : TextFeature
    {
        internal KeyTextFeature(IEntity ent, Operation creator, KeyTextGeometry text)
            : base(ent, creator, text)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyTextFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="geom">The metrics for the text (including the text itself).</param>
        /// <param name="isTopological">Is the new feature expected to act as a polygon label?</param>
        /// <param name="polPosition">The position of the polygon reference position (specify null
        /// if the feature is not a polygon label)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="f"/> is null.</exception>
        internal KeyTextFeature(IFeature f, KeyTextGeometry geom, bool isTopological, PointGeometry polPosition)
            : base(f, geom, isTopological, polPosition)
        {
        }

        /// <summary>
        /// A value indicating the type of geometry used to represent this feature.
        /// </summary>
        internal override FeatureGeometry Representation
        {
            get { return FeatureGeometry.KeyText; }
        }
    }
}
