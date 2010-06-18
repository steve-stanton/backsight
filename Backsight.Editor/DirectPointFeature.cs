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

using System;
using Backsight.Environment;
using Backsight.Editor.Xml;

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
        /// <param name="e">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="g">The geometry for the point (may be null, although this is only really
        /// expected during deserialization)</param>
        internal DirectPointFeature(IEntity e, Operation creator, PointGeometry g)
            : base(e, creator, g)
        {
        }

        internal DirectPointFeature(Operation creator, PointData p)
            : base(creator, p)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectPointFeature"/> class,
        /// without any defined position.
        /// </summary>
        /// <param name="creator">The operation creating the point.</param>
        /// <param name="f">Information about the point.</param>
        internal DirectPointFeature(Operation creator, FeatureData f)
            : base(creator, f)
        {
        }
    }
}
