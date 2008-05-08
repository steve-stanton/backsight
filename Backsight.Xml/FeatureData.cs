/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;
using System.Xml.Serialization;

namespace Backsight.Xml
{
    /// <summary>
    /// Basic information for a spatial feature. This class should be used in situations where
    /// the geometry will be re-generated upon materialization. Derived classes should be used
    /// if an edit involves data that has fixed geometry (such as control points, or data imported
    /// from some foreign data source).
    /// </summary>
    [XmlRoot("Feature")]
    public class FeatureData
    {
        #region Class data

        /// <summary>
        /// The unique ID of the feature.
        /// </summary>
        [XmlAttribute]
        public Guid FID;

        /// <summary>
        /// The ID of this feature's entity type.
        /// </summary>
        [XmlAttribute]
        public int EntityId;

        /// <summary>
        /// The user-perceived ID for this feature (may be blank)
        /// </summary>
        [XmlAttribute]
        public string Key;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FeatureData</c> with nothing defined.
        /// </summary>
        public FeatureData()
        {
            EntityId = 0;
            Key = String.Empty;
        }

        /// <summary>
        /// Creates a new <c>FeatureData</c> with every field explicitly defined.
        /// </summary>
        /// <param name="fid">The unique ID of the feature.</param>
        /// <param name="entityId">The ID of this feature's entity type.</param>
        /// <param name="key">The user-perceived ID for this feature (may be blank)</param>
        public FeatureData(Guid fid, int entityId, string key)
        {
            FID = fid;
            EntityId = entityId;
            Key = key;
        }

        #endregion
    }
}
