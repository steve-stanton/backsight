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

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <summary>
    /// A line with position defined by an instance of <see cref="SectionGeometry"/>
    /// </summary>
    class SectionLineFeature : LineFeature
    {
        internal SectionLineFeature(EditDeserializer editDeserializer)
            : base(editDeserializer)
        {
        }

        internal SectionLineFeature(Operation creator, uint sessionSequence, SectionGeometry section)
            : base(creator, sessionSequence, section.BaseLine.EntityType, (PointFeature)section.Start,
                    (PointFeature)section.End, section)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionLineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="f">Basic information about the feature (not null).</param>
        /// <param name="baseLine">The line that this section is part of</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal SectionLineFeature(IFeature f,
            LineFeature baseLine, PointFeature start, PointFeature end, bool isTopological)
            : base(f, start, end, new SectionGeometry(baseLine, start, end), isTopological)
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

        /// <summary>
        /// Attempts to locate the circular arc (if any) that this line is based on.
        /// </summary>
        /// <returns>The result of calling <see cref="BaseLine.GetArcBase"/> (if the
        /// base line is defined). False if the baseline is undefined.</returns>
        internal override ArcFeature GetArcBase()
        {
            LineFeature baseLine = this.BaseLine;
            return (baseLine == null ? null : baseLine.GetArcBase());
        }
    }
}
