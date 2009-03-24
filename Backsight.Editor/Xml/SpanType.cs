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
    public partial class SpanType
    {
        /// <summary>
        /// The internal ID of the line representing the span (if this is defined, the
        /// <see cref="Line"/> property should be null).
        /// </summary>
        internal string LineId
        {
            get { return (this.Item as string); }
            set { this.Item = value; }
        }

        /// <summary>
        /// Information about the line that was created to represent this span (if this is defined, the
        /// <see cref="LineId"/> property should be null).
        /// </summary>
        internal CalculatedFeatureType Line
        {
            get { return (this.Item as CalculatedFeatureType); }
            set { this.Item = value; }
        }
    }
}
