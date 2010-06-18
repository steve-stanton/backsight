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
using Backsight.Editor.Xml;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// A line with position defined by <see cref="SegmentGeometry"/>
    /// </summary>
    class SegmentLineFeature : LineFeature
    {
        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <param name="t">The serialized version of this feature</param>
        internal SegmentLineFeature(Operation op, SegmentData t)
            : base(op, t)
        {
        }

        /// <summary>
        /// Creates a <c>LineFeature</c> consisting of a simple line segment.
        /// </summary>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        internal SegmentLineFeature(IEntity e, Operation creator, PointFeature start, PointFeature end)
            : base(e, creator, start, end, new SegmentGeometry(start, end))
        {
        }

        /// <summary>
        /// Constructor for use during deserialization, for creating a line consisting of
        /// a simple line segment. This version does not define the position for the new line - the
        /// editing operation must subsequently calculate that.
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <param name="t">The serialized version of the information describing this feature</param>
        internal SegmentLineFeature(Operation op, PointFeature start, PointFeature end, FeatureData t)
            : base(op, start, end, new SegmentGeometry(start, end), t)
        {
        }

    }
}
