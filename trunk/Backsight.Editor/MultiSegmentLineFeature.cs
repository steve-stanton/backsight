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
using System.Diagnostics;

using Backsight.Environment;


namespace Backsight.Editor
{
    /// <summary>
    /// A line with position defined by <see cref="MultiSegmentGeometry"/>
    /// </summary>
    class MultiSegmentLineFeature : LineFeature
    {
        /// <summary>
        /// Creates a <c>MultiSegmentLineFeature</c> consisting of a series of connected line segments.
        /// </summary>
        /// <param name="creator">The operation that created the feature (not null)</param>
        /// <param name="sessionSequence">The 1-based creation sequence of this feature within the
        /// session that created it.</param>
        /// <param name="e">The entity type for the feature.</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <param name="data">The positions defining the shape of the line. The first position must
        /// coincide precisely with the supplied <paramref name="start"/>, and the last position
        /// must coincide precisely with <paramref name="end"/>. Expected to be more than two positions.</param>
        internal MultiSegmentLineFeature(Operation creator, uint sessionSequence, IEntity e,
                                            PointFeature start, PointFeature end, IPointGeometry[] data)
            : base(creator, sessionSequence, e, start, end, new MultiSegmentGeometry(start, end, data))
        {
            Debug.Assert(data.Length>2);
            Debug.Assert(start.Geometry.IsCoincident(data[0]));
            Debug.Assert(end.Geometry.IsCoincident(data[data.Length-1]));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MultiSegmentLineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <param name="g">The geometry for the line (could be null, although this is only really
        /// expected during deserialization)</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal MultiSegmentLineFeature(IFeature f, 
                            PointFeature start, PointFeature end, MultiSegmentGeometry g, bool isTopological)
            : base(f, start, end, g, isTopological)
        {
        }
    }
}
