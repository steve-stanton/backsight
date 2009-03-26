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

using Backsight.Editor.Observations;


namespace Backsight.Editor.Xml
{
    /// <summary>
    /// A serialized <see cref="OffsetPoint"/>
    /// </summary>
    public partial class OffsetPointType
    {
        public OffsetPointType()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OffsetPointType"/> class.
        /// </summary>
        /// <param name="o">The object to serialize</param>
        internal OffsetPointType(OffsetPoint o)
            : base()
        {
            this.Point = o.Point.DataId;
        }

        /// <summary>
        /// Loads this observation as part of an editing operation
        /// </summary>
        /// <param name="op">The editing operation utilizing the observation</param>
        /// <returns>The observation that was loaded</returns>
        internal override Observation LoadObservation(Operation op)
        {
            return new OffsetPoint(op, this);
        }
    }
}

