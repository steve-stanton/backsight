// <remarks>
// Copyright 2008 - Steve Stanton. This file is part of Backsight
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
using Backsight.Editor.Operations;
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <summary>
    /// Helper class that is used to construct a connection path on
    /// deserialization from the database.
    /// </summary>
    class PathBuilder : IXmlContentHelper
    {
        #region Class data

        /// <summary>
        /// The edit that's being deserialized
        /// </summary>
        readonly PathOperation m_Edit;

        /// <summary>
        /// Information about the items defining the path
        /// </summary>
        readonly PathData m_Data;

        /// <summary>
        /// Information about features that were created as a result of a <see cref="PathOperation"/> edit.
        /// The key in the creation sequence number.
        /// </summary>
        readonly Dictionary<uint, FeatureData> m_Features;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>PathBuilder</c>
        /// </summary>
        /// <param name="op">The editing operation that's being deserialized from the database</param>
        /// <param name="pd">Information about the entered path</param>
        /// <param name="features">Information about features created on the path</param>
        internal PathBuilder(PathOperation op, PathData pd, FeatureData[] features)
        {
            m_Edit = op;
            m_Data = pd;
            m_Features = new Dictionary<uint, FeatureData>(features.Length);

            foreach (FeatureData fd in features)
                m_Features.Add(fd.CreationSequence, fd);
        }

        #endregion
    }
}
