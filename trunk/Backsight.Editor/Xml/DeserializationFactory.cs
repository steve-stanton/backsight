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

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// A <see cref="FeatureFactory"/> for use during deserialization from
    /// the database. Having created the factory, deserialization code must
    /// initialize feature information via calls to <see cref="AddFeatureData"/>.
    /// </summary>
    class DeserializationFactory : FeatureFactory
    {
        #region Class data

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="DeserializationFeatureFactory"/> class.
        /// </summary>
        /// <param name="op">The editing operation that needs to create features (not null).</param>
        /// <exception cref="ArgumentNullException">If the supplied editing operation is undefined</exception>
        internal DeserializationFactory(Operation op)
            : base(op)
        {
        }

        #endregion

        internal void AddFeatureStub(string itemName, FeatureData data)
        {
            if (data != null)
                AddFeatureData(itemName, (FeatureStubData)data);
        }

        internal void AddFeatureData(string itemName, FeatureStubData stub)
        {
            AddFeatureDescription(itemName, stub.GetFeatureStub(base.Creator));
        }

        /// <summary>
        /// Creates a new instance of <see cref="PointFeature"/>, using the feature
        /// stub with the specified name.
        /// </summary>
        /// <param name="itemName">The name associated with the feature (unique to the editing
        /// operation that this factory is for).</param>
        /// <returns>The new feature (without any defined geometry).</returns>
        internal override PointFeature CreatePointFeature(string itemName)
        {
            IFeature f = FindFeatureDescription(itemName);
            if (f == null)
                return null;
            else
                return new PointFeature(f, null);
        }

        /// <summary>
        /// Deactivates a line as part of deserialization from the database.
        /// </summary>
        /// <param name="line">The line that needs to be deactivated</param>
        internal override void DeactivateLine(LineFeature line)
        {
            line.Deactivate();
        }
    }
}
