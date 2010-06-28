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
    /// A point that shares the same location as another point.
    /// </summary>
    class SharedPointFeature : PointFeature
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedPointFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null)</param>
        /// <param name="firstPoint">The point feature that the new point coincides with (not null)</param>
        /// <exception cref="ArgumentNullException">If <paramref name="firstPoint"/> is null.</exception>
        internal SharedPointFeature(IFeature f, PointFeature firstPoint)
            : base(f, firstPoint)
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

        /// <summary>
        /// A value indicating the type of geometry used to represent this feature.
        /// </summary>
        internal override FeatureGeometry Representation
        {
            get { return FeatureGeometry.SharedPoint; }
        }
    }
}
