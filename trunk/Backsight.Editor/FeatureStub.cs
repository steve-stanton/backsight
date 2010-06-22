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


namespace Backsight.Editor
{
    /// <summary>
    /// A placeholder for a spatial feature, used during deserialization from the database.
    /// </summary>
    class FeatureStub : Feature
    {
        #region Class data

        /// <summary>
        /// The type of geometry that is used for the feature (when converted from a stub).
        /// </summary>
        readonly FeatureGeometry m_GeometricType;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="FeatureStub"/> class
        /// </summary>
        /// <param name="iid">The internal ID for the feature.</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal FeatureStub(InternalIdValue iid, FeatureId fid, IEntity ent, Operation creator, FeatureGeometry geometricType)
            : base(iid, fid, ent, creator)
        {
            m_GeometricType = geometricType;
        }

        #endregion

        public override SpatialType SpatialType
        {
            get
            {
                switch (m_GeometricType)
                {
                    case FeatureGeometry.DirectPoint:
                    case FeatureGeometry.SharedPoint:
                        return SpatialType.Point;

                    case FeatureGeometry.Segment:
                    case FeatureGeometry.Arc:
                    case FeatureGeometry.MultiSegment:
                    case FeatureGeometry.Section:
                        return SpatialType.Line;

                    case FeatureGeometry.KeyText:
                    case FeatureGeometry.RowText:
                    case FeatureGeometry.MiscText:
                        return SpatialType.Text;
                }

                throw new ApplicationException("Unexpected geometric type: "+m_GeometricType);
            }
        }

        /// <summary>
        /// Draws this object on the specified display
        /// </summary>
        /// <param name="display">The display to draw to</param>
        /// <param name="style">The drawing style</param>
        public override void Render(ISpatialDisplay display, IDrawStyle style)
        {
        }

        /// <summary>
        /// The covering rectangle that encloses this feature.
        /// </summary>
        /// <value>Null (always)</value>
        public override IWindow Extent
        {
            get { return null; }
        }

        /// <summary>
        /// The shortest distance between this object and the specified position.
        /// </summary>
        /// <param name="point">The position of interest</param>
        /// <returns>
        /// The shortest distance between the specified position and this object (always null).
        /// </returns>
        public override ILength Distance(IPosition point)
        {
            return null;
        }

        /// <summary>
        /// A value indicating the type of geometry used to represent this feature.
        /// </summary>
        internal override FeatureGeometry Representation
        {
            get { return FeatureGeometry.Stub; }
        }
    }
}
