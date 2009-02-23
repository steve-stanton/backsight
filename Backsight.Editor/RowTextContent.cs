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
        /// The formatted ID of the spatial feature
        /// </summary>
        string m_Id;

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
        /// Default constructor (for serialization)
        /// </summary>
        public RowTextContent()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowTextContent"/> class that matches
        /// an instance of <see cref="RowTextGeometry"/>
        /// </summary>
        /// <param name="copy">The text geometry to copy from</param>
        internal RowTextContent(RowTextGeometry copy)
            : base(copy)
        {
            Row r = copy.Row;
            ITemplate t = copy.Template;

            m_Id = r.Id.FormattedKey;
            m_TableId = r.Table.Id;
            m_TemplateId = t.Id;
        }

        #endregion

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            base.WriteContent(writer);

            writer.WriteString("Id", m_Id);
            writer.WriteInt("Table", m_TableId);
            writer.WriteInt("Template", m_TemplateId);
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            base.ReadContent(reader);

            m_Id = reader.ReadString("Id");
            m_TableId = reader.ReadInt("Table");
            m_TemplateId = reader.ReadInt("Template");
        }
    }
}
