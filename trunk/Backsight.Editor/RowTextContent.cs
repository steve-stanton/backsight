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

using Backsight.Environment;
using Backsight.Editor.Xml;

namespace Backsight.Editor
{
    /// <summary>
    /// A representation of <see cref="RowTextGeometry"/> that is used during
    /// database serialization.
    /// </summary>
    class RowTextContent : RowTextGeometry
    {
        #region Class data

        /// <summary>
        /// The Backsight ID for the database table
        /// </summary>
        int m_TableId;

        /// <summary>
        /// The ID of the formatting template
        /// </summary>
        int m_TemplateId;

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor for use during deserialization
        /// </summary>
        /// <param name="f">The feature that makes use of this geometry</param>
        /// <param name="t">The serialized version of the feature</param>
        internal RowTextContent(TextFeature f, RowTextData t)
            : base(f, t)
        {
            m_TableId = (int)t.Table;
            m_TemplateId = (int)t.Template;
        }

        #endregion

        /// <summary>
        /// Override returns a null spatial extent. The extent will be defined
        /// only after instances of <c>RowTextContent</c> have been associated
        /// with database attributes as part of the deserialization logic that
        /// occurs during application startup (at that time, the geometry will be
        /// replaced with fully defined instances of <see cref="RowTextGeometry"/>).
        /// </summary>
        public override IWindow Extent
        {
            get { return null; }
        }

        /// <summary>
        /// The Backsight ID for the database table
        /// </summary>
        internal int TableId
        {
            get { return m_TableId; }
        }

        /// <summary>
        /// The ID of the formatting template
        /// </summary>
        internal int TemplateId
        {
            get { return m_TemplateId; }
        }

        /// <summary>
        /// The text string represented by this geometry is "NoData" (always).
        /// Instances of <c>RowTextContent</c> should exist only for a short
        /// period during deserialization from the database (however, if database
        /// rows have been deleted unexpectedly, the content object may continue
        /// to exist).
        /// </summary>
        public override string Text
        {
            get { return "NoData"; }
        }
    }
}
