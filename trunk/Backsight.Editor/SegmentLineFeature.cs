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
    /// A line with position defined by <see cref="SegmentGeometry"/>
    /// </summary>
    class SegmentLineFeature : LineFeature
    {
        /// <summary>
        /// Creates a <c>LineFeature</c> consisting of a simple line segment.
        /// </summary>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="sessionSequence">The 1-based creation sequence of this feature within the
        /// session that created it.</param>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        internal SegmentLineFeature(Operation creator, uint sessionSequence, IEntity e, PointFeature start, PointFeature end)
            : base(creator, sessionSequence, e, start, end, new SegmentGeometry(start, end))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentLineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="start">The point at the start of the line (not null).</param>
        /// <param name="end">The point at the end of the line (not null).</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        internal SegmentLineFeature(IFeature f, PointFeature start, PointFeature end)
            : this(f, start, end, f.EntityType.IsPolygonBoundaryValid)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentLineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="start">The point at the start of the line (not null).</param>
        /// <param name="end">The point at the end of the line (not null).</param>
        /// <param name="isTopological">Should the line be tagged as a polygon boundary?</param>
        /// <exception cref="ArgumentNullException">If any parameter is null.</exception>
        internal SegmentLineFeature(IFeature f, PointFeature start, PointFeature end, bool isTopological)
            : base(f, start, end, new SegmentGeometry(start, end), isTopological)
        {
        }

        /// <summary>
        /// A value indicating the type of geometry used to represent this feature.
        /// </summary>
        internal override FeatureGeometry Representation
        {
            get { return FeatureGeometry.Segment; }
        }

    }
}
