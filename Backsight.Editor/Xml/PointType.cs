// <remarks>
// Copyright 2009 - Steve Stanton. This file is part of Backsight
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
    /// A serialized point feature.
    /// </summary>
    public partial class PointType
    {
        public PointType()
        {
        }

        internal PointType(PointFeature p)
        {
            this.Id = p.DataId;
            this.Type = p.EntityType.Id;

            if (p.Id != null)
            {
                FeatureId fid = p.Id;
                if (fid.RawId == 0)
                    this.ForeignKey = fid.FormattedKey;
                else
                {
                    this.Key = fid.RawId;
                    this.KeySpecified = true;
                }
            }

            IPointGeometry pg = p.Geometry;
            this.X = pg.Easting.Microns;
            this.Y = pg.Northing.Microns;
        }

        /// <summary>
        /// Loads this feature as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation creating the feature</param>
        /// <returns>The spatial feature that was loaded</returns>
        internal override Feature LoadFeature(Operation op)
        {
            return new PointFeature(op, this);
        }
    }
}
