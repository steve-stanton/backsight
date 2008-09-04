// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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
using System.Xml.Serialization;

namespace Backsight.Content
{
    [XmlInclude(typeof(StraightLegContent))]
    abstract public class LegContent
    {
        /// <summary>
        /// The data that defines each span on this leg (should always contain at least
        /// one element).
        /// </summary>
        [XmlArray]
        SpanDataContent[] Spans;

        /// <summary>
        /// The face number of this leg (if this leg is staggered). In the range [0,2]. A value
        /// of zero means the leg is not staggered.
        /// </summary>
        [XmlAttribute]
        byte FaceNumber;
    }
}
