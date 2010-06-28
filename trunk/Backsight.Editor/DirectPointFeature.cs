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
    /// A point that has an explicit XY(Z) position.
    /// </summary>
    class DirectPointFeature : PointFeature
    {
        /// <summary>
        /// Creates a new <c>DirectPointFeature</c>
        /// </summary>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="g">The geometry for the point (may be null, although this is only really
        /// expected during deserialization)</param>
        internal DirectPointFeature(IEntity ent, Operation creator, PointGeometry g)
            : base(ent, creator, g)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectPointFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature.</param>
        /// <param name="g">The geometry for the point (could be null, although this is only really
        /// expected during deserialization)</param>
        internal DirectPointFeature(IFeature f, PointGeometry g)
            : base(f, g)
        {
        }

        /// <summary>
        /// A value indicating the type of geometry used to represent this feature.
        /// </summary>
        internal override FeatureGeometry Representation
        {
            get { return FeatureGeometry.DirectPoint; }
        }
    }
}
