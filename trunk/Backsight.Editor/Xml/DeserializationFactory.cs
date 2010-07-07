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
using System.Collections.Generic;

namespace Backsight.Editor.Xml
{
    /// <summary>
    /// A <see cref="FeatureFactory"/> for use during deserialization from
    /// the database. Having created the factory, deserialization code must
    /// initialize feature information via calls to <see cref="AddFeatureDescription"/>.
    /// </summary>
    class DeserializationFactory : FeatureFactory
    {
        #region Class data

        /// <summary>
        /// Information about features that will be created, keyed by a name (that
        /// corresponds to the element name when represented in XML).
        /// </summary>
        readonly Dictionary<string, IFeature> m_Features;

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
            m_Features = new Dictionary<string, IFeature>();
        }

        #endregion

        //internal void AddFeatureData<T>(string itemName, T data) where T : FeatureData
        //{
        //    data.LoadFeature
        //}

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
        /// Records information for a feature that needs to be produced by this factory.
        /// </summary>
        /// <param name="itemName">A name associated with the feature (unique to the editing
        /// operation that this factory is for).</param>
        /// <param name="f">Basic information for the feature.</param>
        internal void AddFeatureDescription(string itemName, IFeature f)
        {
            if (f.Creator != base.Creator)
                throw new ArgumentException();

            m_Features.Add(itemName, f);
        }

        /// <summary>
        /// Creates a new instance of <see cref="DirectPointFeature"/>, using the feature
        /// stub with the specified name.
        /// </summary>
        /// <param name="itemName">The name associated with the feature (unique to the editing
        /// operation that this factory is for).</param>
        /// <returns>The new feature (without any defined geometry).</returns>
        internal override DirectPointFeature CreateDirectPointFeature(string itemName)
        {
            IFeature f = m_Features[itemName];
            return new DirectPointFeature(f, null);
        }

        internal override SegmentLineFeature CreateSegmentLineFeature(string itemName, PointFeature from, PointFeature to)
        {
            IFeature f;
            if (m_Features.TryGetValue(itemName, out f))
                return new SegmentLineFeature(f, from, to, f.EntityType.IsPolygonBoundaryValid);
            else
                return null;
        }
    }
}
