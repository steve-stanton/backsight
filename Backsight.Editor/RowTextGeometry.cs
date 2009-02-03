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
    /// <written by="Steve Stanton" on="15-MAY-1998" was="CeRowText" />
    /// <summary>
    /// Row text describes how to format a text string that is
    /// defined via attributes in an associated database table.
    /// </summary>
    class RowTextGeometry : TextGeometry
    {
        #region Class data

        /// <summary>
        /// The row that contains the text
        /// </summary>
        Row m_Row;

        /// <summary>
        /// How to form the text string out of the data in the row.
        /// </summary>
        ITemplate m_Template;        

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public RowTextGeometry()
        {
        }

        #endregion

        /// <summary>
        /// The text string represented by this geometry
        /// </summary>
        public override string Text
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        public override void WriteContent(XmlContentWriter writer)
        {
            // Will need to write out some sort of proxy, since it's not
            // possible to read back RowText until the attribute data has
            // been loaded from the database (and that only happens after
            // all features have been deserialized from the database).

            // To simplify things, let the proxy extend this class so
            // that it can pretend to be the real thing during ReadContent.

            if (this is RowTextContent)
            {
                base.WriteContent(writer);

                writer.WriteString("Id", m_Row.Id.FormattedKey);
                writer.WriteInt("Table", m_Row.Table.Id);
                writer.WriteInt("Template", m_Template.Id);
            }
            else
            {
                new RowTextContent(this).WriteContent(writer);
            }
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        public override void ReadContent(XmlContentReader reader)
        {
            // Read back the proxy that was written by WriteContent, caching
            // the result as part of the reader.

            base.ReadContent(reader);
        }
    }
}
