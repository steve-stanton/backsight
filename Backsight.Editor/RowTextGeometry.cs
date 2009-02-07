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

        // The format string specifies both alphanumeric characters and one or more
        // of the fields in the table to be output as the annotation text. Each
        // attribute field is enclosed in square brackets.
        // 
        //  character ‘%' indicates that an attribute field follows, the data
        //  following the % char has the following rules:
        //	    an integer following this specifies that the contents of the field
        //	      be inserted into the output,
        //	    a k character specifies that the value of the key be output
        //	    a n character attached to a number indicates that the Name of the
        //	      numbered field schema be output,
        //	    a v character attached to a number indicates that the a Name
        //	      associated with the value of the numbered field be output.
        //	      The Name and the Field Value are contained in a List Domain that
        //        must be attached to the Field Schema identified by the number
        //        following the v character,
        //	    a % character (i.e. %%) indicates that the % character is output
        //	    
        //  The Field Schemas are numbered from 1.
        //  Examples:
        //          "C.T. %1" specifies that the characters "C.T. " be followed by
        //            the data contained in FieldSchema 1,
        //          "%1 %v3 %2 Parish of  %v4" specifies the the contents of the
        //            1st field be followed by a space then by the value in the
        //            list domain associated with the contents of the 3rd field,
        //            then by a space, then by the contents of the 2nd field, then
        //            by a space and the word "Parish of", then by the value in the
        //            list domain associated with the contents of the 4th field
        //          "Public Lane" specifies that only this text be output

        /// <summary>
        /// The text string represented by this geometry
        /// </summary>
        public override string Text
        {
            get
            {
                // Get the row's key (it MUST have one in order for the row text to exist at all).
                FeatureId id = m_Row.Id;
                if (id==null)
                    return "ID not available";

                //return m_Template.GetText(m_Row, id.FormattedKey);
                return "?";
            }
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
