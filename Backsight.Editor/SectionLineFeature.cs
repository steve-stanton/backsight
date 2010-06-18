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
    /// A line with position defined by an instance of <see cref="SectionGeometry"/>
    /// </summary>
    class SectionLineFeature : LineFeature
    {
        internal SectionLineFeature(Operation creator, SectionGeometry section)
            : base(section.BaseLine.EntityType, creator, (PointFeature)section.Start,
                    (PointFeature)section.End, section)
        {
        }

        internal SectionLineFeature(Operation creator, SectionData t)
            : base(creator, t)
        {
        }

        /// <summary>
        /// The line that the section is based on.
        /// </summary>
        internal LineFeature BaseLine
        {
            get
            {
                SectionGeometry geom = (SectionGeometry)base.LineGeometry;
                return (geom == null ? null : geom.BaseLine);
            }
        }
    }
}
