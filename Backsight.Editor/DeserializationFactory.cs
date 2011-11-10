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

namespace Backsight.Editor
{
    /// <summary>
    /// A <see cref="FeatureFactory"/> for use during deserialization from
    /// the database. Having created the factory, deserialization code must
    /// initialize feature information via calls to <see cref="AddFeatureData"/>.
    /// </summary>
    class DeserializationFactory : FeatureFactory
    {
        #region Class data

        // none

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

        internal void AddFeatureStub(string itemName, FeatureStubData data)
        {
            if (data != null)
                AddFeatureDescription(itemName, data.GetFeatureStub(base.Creator));
        }

        internal void AddFeatureStub(string itemName, FeatureStub stub)
        {
            if (stub != null)
                AddFeatureDescription(itemName, stub);
        }

        /// <summary>
        /// Records information for a line split
        /// </summary>
        /// <param name="parentLine">The line that may be getting split</param>
        /// <param name="itemName">The name of the item that should be attached to the line split info</param>
        /// <param name="dataId">The ID for the section (null if there is no split)</param>
        /// <returns>True if a line split was recorded, false if the <paramref name="splitSection"/> is null.</returns>
        internal bool AddLineSplit(LineFeature parentLine, string itemName, string dataId)
        {
            if (dataId == null)
                return false;

            uint sessionId, ss;
            InternalIdValue.Parse(dataId, out sessionId, out ss);
            AddFeatureDescription(itemName, new FeatureStub(Creator, ss, parentLine.EntityType, null));
            return true;
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
        /// Creates a new <see cref="SegmentLineFeature"/> using the feature
        /// stub with the specified name.
        /// </summary>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The created feature (null if there is no feature stub)</returns>
        internal override SegmentLineFeature CreateSegmentLineFeature(string itemName, PointFeature from, PointFeature to)
        {
            IFeature f = FindFeatureDescription(itemName);

            if (f == null)
                return null;
            else
                return new SegmentLineFeature(f, from, to);
        }

        /// <summary>
        /// Creates a new <see cref="ArcFeature"/> using information previously
        /// recorded via a call to <see cref="AddFeatureDescription"/>.
        /// </summary>
        /// <param name="itemName">The name for the item involved</param>
        /// <param name="from">The point at the start of the line (not null).</param>
        /// <param name="to">The point at the end of the line (not null).</param>
        /// <returns>The new feature (null if a feature description was not previously added)</returns>
        internal override ArcFeature CreateArcFeature(string itemName, PointFeature from, PointFeature to)
        {
            IFeature f = FindFeatureDescription(itemName);

            if (f == null)
                return null;
            else
                return new ArcFeature(f, from, to, null, f.EntityType.IsPolygonBoundaryValid);
        }

        /// <summary>
        /// Deactivates a feature as part of deserialization from the database.
        /// </summary>
        /// <param name="f">The feature that needs to be deactivated</param>
        internal override void Deactivate(Feature f)
        {
            // When a line is deactivated during the course of regular editing work,
            // any topological constructs will be removed when the model is cleaned
            // at the end of the edit. During deserialization, the model doesn't get
            // cleaned, so remove any topological stuff now.

            LineFeature line = (f as LineFeature);
            if (line != null)
                line.RemoveTopology();

            base.Deactivate(f);
        }
    }
}
