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

namespace Backsight.Editor
{
    /// <summary>
    /// A factory for generating features that will be thrown away. These
    /// throwaway feature may be needed when previewing the results of an edit
    /// (which the user may decide to cancel).
    /// </summary>
    /// <remarks>Currently used when defining staggered property lots (alternate faces)</remarks>
    class ThrowawayFeatureFactory : FeatureFactory
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ThrowawayFeatureFactory"/> class.
        /// </summary>
        /// <param name="op">The editing operation that needs to create features (not null).</param>
        /// <exception cref="ArgumentNullException">If the supplied editing operation is undefined</exception>
        internal ThrowawayFeatureFactory(Operation op)
            : base(op)
        {
        }

        #endregion

        /// <summary>
        /// Creates a new instance of <see cref="PointFeature"/>, with the currently
        /// active entity type (and a user-perceived ID if it applies), and adds to the model.
        /// </summary>
        /// <returns>The new feature (never null)</returns>
        internal override PointFeature CreatePointFeature(string itemName)
        {
            IFeature f = new FeatureStub(this.Creator, 0, this.PointType, null);
            return new PointFeature(f, null);
        }

        /// <summary>
        /// Creates a new line section
        /// </summary>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="baseLine">The line that this section is part of</param>
        /// <param name="from">The point at the start of the section</param>
        /// <param name="to">The point at the end of the section</param>
        /// <returns>The created section (never null)</returns>
        internal override LineFeature CreateSection(string itemName, LineFeature baseLine,
                                                            PointFeature from, PointFeature to)
        {
            IFeature f = new FeatureStub(this.Creator, 0, baseLine.EntityType, null);
            return new LineFeature(f, baseLine, from, to, false);
        }

    }
}
