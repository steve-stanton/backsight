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
        /// Writes the attributes of this class.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteAttributes(XmlContentWriter writer)
        {
            base.WriteAttributes(writer);

            writer.WriteString("Id", m_Id);
            writer.WriteInt("Table", m_TableId);
            writer.WriteInt("Template", m_TemplateId);
        }

        /// <summary>
        /// Writes any child elements of this class. This will be called after
        /// all attributes have been written via <see cref="WriteAttributes"/>.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteChildElements(XmlContentWriter writer)
        {
            base.WriteChildElements(writer);
        }

        /// <summary>
        /// Defines the attributes of this content
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadAttributes(XmlContentReader reader)
        {
            base.ReadAttributes(reader);

            m_Id = reader.ReadString("Id");
            m_TableId = reader.ReadInt("Table");
            m_TemplateId = reader.ReadInt("Template");
        }

        /// <summary>
        /// Defines any child content related to this instance. This will be called after
        /// all attributes have been defined via <see cref="ReadAttributes"/>.
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadChildElements(XmlContentReader reader)
        {
            base.ReadChildElements(reader);
        }

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
