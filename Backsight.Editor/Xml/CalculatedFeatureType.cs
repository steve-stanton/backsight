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
    public partial class CalculatedFeatureType
    {
        public CalculatedFeatureType()
        {
        }

        internal CalculatedFeatureType(Feature f)
            : this(f, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalculatedFeatureType"/> class.
        /// </summary>
        /// <param name="f">The feature that is being serialized</param>
        /// <param name="allFields">Should all attributes be serialized? Specify <c>false</c> only
        /// if the feature actually represents something that previously existed at the calculated
        /// position.</param>
        internal CalculatedFeatureType(Feature f, bool allFields)
        {
            this.Id = f.DataId;

            if (allFields)
            {
                this.Type = f.EntityType.Id;

                FeatureId fid = f.Id;
                if (fid != null)
                {
                    if (fid is NativeId)
                    {
                        this.Key = fid.RawId;
                        this.KeySpecified = true;
                    }
                    else
                        this.ForeignKey = fid.FormattedKey;
                }
            }
        }
    }
}
