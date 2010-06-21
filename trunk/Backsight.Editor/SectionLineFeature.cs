﻿// <remarks>
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
        internal SectionLineFeature(Operation creator, SectionGeometry section)
            : base(section.BaseLine.EntityType, creator, (PointFeature)section.Start,
                    (PointFeature)section.End, section)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SectionLineFeature"/> class, and records it
        /// as part of the map model.
        /// </summary>
        /// <param name="iid">The internal ID for the feature.</param>
        /// <param name="fid">The (optional) user-perceived ID for the feature. If not null,
        /// this will be modified by cross-referencing it to the newly created feature.</param>
        /// <param name="ent">The entity type for the feature (not null)</param>
        /// <param name="creator">The operation creating the feature (not null). Expected to
        /// refer to an editing session that is consistent with the session ID that is part
        /// of the feature's internal ID.</param>
        /// <param name="baseLine">The line that this section is part of</param>
        /// <param name="start">The point at the start of the line</param>
        /// <param name="end">The point at the end of the line</param>
        /// <exception cref="ArgumentNullException">If either <paramref name="ent"/> or
        /// <paramref name="creator"/> is null.</exception>
        internal SectionLineFeature(InternalIdValue iid, FeatureId fid, IEntity ent, Operation creator,
            LineFeature baseLine, PointFeature start, PointFeature end, bool isTopological)
            : base(iid, fid, ent, creator, start, end, new SectionGeometry(baseLine, start, end), isTopological)
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