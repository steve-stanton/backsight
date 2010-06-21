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

namespace Backsight.Editor
{
    /// <summary>
    /// Identifies a type of geometry for spatial features (for use as part of the
    /// <see cref="FeatureStub"/> class).
    /// </summary>
    enum FeatureGeometry
    {
        /// <summary>
        /// A point with an explicit position
        /// (corresponding to the <see cref="DirectPointFeature"/> class)
        /// </summary>
        DirectPoint = 10,

        /// <summary>
        /// A point that shares the same position as another point
        /// (corresponding to the <see cref="SharedPointFeature"/> class)
        /// </summary>
        SharedPoint = 11,

        /// <summary>
        /// A simple line segment, consisting of two positions
        /// (corresponding to the <see cref="SegmentLineFeature"/> class).
        /// </summary>
        Segment = 20,

        /// <summary>
        /// A circular arc
        /// (corresponding to the <see cref="ArcFeature"/> class).
        /// </summary>
        Arc = 21,

        /// <summary>
        /// A line containing 3 or more explicit positions
        /// (corresponding to the <see cref="MultiSegmentLineFeature"/> class).
        /// </summary>
        MultiSegment = 22,

        /// <summary>
        /// A text label that portrays the user-perceived ID for a feature
        /// (corresponding to the <see cref="KeyTextFeature"/> class).
        /// </summary>
        KeyText = 30,

        /// <summary>
        /// A text label that portrays database attributes of a feature.
        /// (corresponding to the <see cref="RowTextFeature"/> class).
        /// </summary>
        RowText=31,

        /// <summary>
        /// A text label that portrays a miscellaneous item of text
        /// (corresponding to the <see cref="MiscTextFeature"/> class).
        /// </summary>
        MiscText=32,
    }
}
