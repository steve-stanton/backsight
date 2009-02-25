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
using System.Text;
using System.Diagnostics;
using System.Data;

using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="15-MAY-1998" was="CeRowText" />
    /// <summary>
    /// Row text describes how to format a text string that is
    /// defined via attributes in an associated database table.
    /// </summary>
    class RowTextGeometry : TextGeometry, IXmlAlternateContent
    {
        #region Class data

        /// <summary>
        /// The row that contains the information to format
        /// </summary>
        Row m_Row;

        /// <summary>
        /// How to form the text string out of the data in the row.
        /// </summary>
        /// <remarks>It may be better to hold some sort of "prepared" template here (see additional
        /// remarks for <see cref="GetDomainValue"/>)</remarks>
        ITemplate m_Template;        

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor (for serialization)
        /// </summary>
        public RowTextGeometry()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowTextGeometry"/> class.
        /// </summary>
        /// <param name="row">The row that contains the information to format</param>
        /// <param name="template">How to form the text string out of the data in the row</param>
        /// <param name="pos">Position of the text's reference point (always the top left corner of the string).</param>
        /// <param name="font">The text style (defines the type-face and the height of the text).</param>
        /// <param name="height">The height of the text, in meters on the ground.</param>
        /// <param name="width">The total width of the text, in meters on the ground.</param>
        /// <param name="rotation">Clockwise rotation from horizontal</param>
        internal RowTextGeometry(Row row, ITemplate template,
                                 PointGeometry pos, IFont font, double height, double width, float rotation)
            : base(pos, font, height, width, rotation)
        {
            if (row==null || template==null)
                throw new ArgumentNullException();

            m_Row = row;
            m_Template = template;
        }

        /// <summary>
        /// Copy constructor (for use by the <see cref="RowTextContent"/> class)
        /// </summary>
        /// <param name="copy">The geometry to copy</param>
        protected RowTextGeometry(RowTextGeometry copy)
            : base(copy)
        {
            m_Row = copy.m_Row;
            m_Template = copy.m_Template;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RowTextGeometry"/> class, using the supplied
        /// content placeholder and the row of attribute data that has been discovered.
        /// </summary>
        /// <param name="row">The row that contains the information to format</param>
        /// <param name="content">The content placeholder read from the database</param>
        /// <remarks>This constructor is utilized during deserialization from the database, which
        /// occurs when an editing job is opened.</remarks>
        internal RowTextGeometry(Row row, RowTextContent content)
            : base(content)
        {
            if (row == null)
                throw new ArgumentNullException();

            m_Row = row;
            m_Template = EnvironmentContainer.FindTemplateById(content.TemplateId);

            if (m_Template == null)
                throw new InvalidOperationException("Cannot locate text template: "+content.TemplateId);
        }

        #endregion

        /// <summary>
        /// The row that contains the information to format
        /// </summary>
        internal Row Row
        {
            get { return m_Row; }
        }

        /// <summary>
        /// How to form the text string out of the data in the row.
        /// </summary>
        internal ITemplate Template
        {
            get { return m_Template; }
        }

        /// <summary>
        /// The text string represented by this geometry
        /// </summary>
        public override string Text
        {
            get
            {
                Debug.Assert(m_Row.Table.Id == m_Template.Schema.Id);
                return GetText(m_Row.Data, m_Template);
            }
        }

        /// <summary>
        /// Generates the text that should be displayed for the supplied row and template
        /// </summary>
        /// <param name="row">The row containing the information to display</param>
        /// <param name="template">The template that defines the output format</param>
        /// <returns>The text that needs to be displayed</returns>
        /// <remarks>
        /// The format string specifies both alphanumeric characters and one or more
        /// of the fields in the table to be output as the annotation text. Each
        /// attribute field is enclosed in square brackets (e.g. [ID] means the
        /// value of the ID column should be substituted).
        /// <para/>
        /// If the column name ends with a "+" character, it means the data value
        /// should be used to perform a lookup on a domain table attached to that
        /// column.
        /// <para/>
        /// Newlines can be inserted by specifying the string "\n". The text on each
        /// successive line will be horizontally center-aligned.
        /// <para/>
        /// TODO: Provide escape for []+ characters.
        /// <para/>
        /// Examples:
        /// <para/>
        /// "C.T. [Certificate_of_Title_Name]" - specifies that the characters "C.T. " be followed by
        /// the data contained in the <c>Certificate_of_Title_Name</c> column.
        /// <para/>
        /// "[Par_Lot_Type+] [Par_Lot_ID]\nParish of [Parish+]" - specifies that the content of
        /// the <c>Par_Lot_Type</c> column should be used to perform a lookup on an associated domain
        /// table, followed by a space, then the value of the <c>Par_Lot_ID</c> column. On the next
        /// line, the text "Parish of " will be followed by a lookup on the associated domain table.
        /// <para/>
        /// "Public Lane" - specifies that only this text be output
        /// </remarks>
        internal static string GetText(DataRow data, ITemplate template)
        {
            ITable schema = template.Schema;
            Debug.Assert(schema!=null);

            StringBuilder result = new StringBuilder(100);
            string fmt = template.Format;
            int startIndex = 0;

            while (startIndex < fmt.Length)
            {
                // Locate the start of the next field
                int startField = fmt.IndexOf('[', startIndex);

                // If we didn't find any, ensure we copy the rest of the format to the output buffer
                if (startField < 0)
                {
                    AppendText(result, fmt.Substring(startIndex));
                    startIndex = fmt.Length;
                }
                else
                {
                    // If the start of the field is preceded by stuff that hasn't been copied, do it now
                    if (startField > startIndex)
                        AppendText(result, fmt.Substring(startIndex, startField-startIndex));

                    // Locate the end bracket (and disallow something like "[]")
                    int endField = fmt.IndexOf(']', startField);
                    Debug.Assert(endField > startField);
                    if (endField <= (startField+1))
                        throw new FormatException("Cannot decode template format: "+fmt);

                    // Grab the name of the relevant column
                    string fieldName = fmt.Substring(startField+1, endField-startField-1);
                    Debug.Assert(fieldName.Length > 0);

                    // If the field name ends in a "+" character, it's shorthand to expand the value
                    // according to the field's domain table.
                    bool expand = fieldName.EndsWith("+");
                    if (expand)
                        fieldName = fieldName.Substring(0, fieldName.Length-1);

                    // Grab the value from the row
                    DataColumn dc = data.Table.Columns[fieldName];
                    int columnIndex = dc.Ordinal;
                    string s = (data.IsNull(columnIndex) ? String.Empty : data[columnIndex].ToString());

                    if (expand)
                        s = GetDomainValue(s, fieldName, schema);

                    result.Append(s);

                    startIndex = endField+1;
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Attempts to expand a domain lookup value
        /// </summary>
        /// <param name="shortValue">The short lookup string</param>
        /// <param name="columnName">The name of the column the lookup value was obtained from</param>
        /// <param name="table">The table containing the column</param>
        /// <returns>The expanded value in the domain associated with the column. If the column is
        /// not associated with a domain table, or the lookup string cannot be found, you get back
        /// the supplied <paramref name="shortValue"/>.</returns>
        /// <remarks>There is some scope for efficiency improvement here. Rather than doing this
        /// every time some RowTextGeometry is drawn, it would make sense to hold something like
        /// a <c>PreparedTemplate</c> object that holds a parsed template that points more directly
        /// to any domains involved.</remarks>
        static string GetDomainValue(string shortValue, string columnName, ITable table)
        {
            // Attempt to locate the column in question
            IColumnDomain[] cds = table.ColumnDomains;
            IColumnDomain cd = Array.Find<IColumnDomain>(cds, delegate(IColumnDomain t)
                { return String.Compare(t.ColumnName, columnName, true)==0; });
            if (cd == null)
                return shortValue;

            // Perform a lookup on the domain
            IDomainTable dt = cd.Domain;
            string longValue = dt.Lookup(shortValue);

            // If it's not there, it SHOULD be blank
            if (String.IsNullOrEmpty(longValue))
                return shortValue;
            else
                return longValue;
        }

        /// <summary>
        /// Appends plain text to a string buffer, expanding any "\n" substrings into
        /// a newline character sequence.
        /// </summary>
        /// <param name="sb">The buffer to append to</param>
        /// <param name="text">The text that might contain embedded newline tokens</param>
        static void AppendText(StringBuilder sb, string text)
        {
            const string newLine = @"\n";
            int slashIndex = text.IndexOf(newLine);

            if (slashIndex < 0)
            {
                sb.Append(text);
            }
            else
            {
                int startIndex = 0;

                while (slashIndex >= startIndex && startIndex < text.Length)
                {
                    if (startIndex < slashIndex)
                        sb.Append(text.Substring(startIndex, slashIndex-startIndex));

                    sb.Append(System.Environment.NewLine);
                    startIndex = slashIndex+2;
                    slashIndex = text.IndexOf(newLine, startIndex);
                }

                if (startIndex < text.Length)
                    sb.Append(text.Substring(startIndex));
            }
        }

        /// <summary>
        /// Writes the content of this class. This is called by
        /// <see cref="XmlContentWriter.WriteElement"/>
        /// after the element name and class type (xsi:type) have been written.
        /// </summary>
        /// <param name="writer">The writing tool</param>
        /*
        public override void WriteContent(XmlContentWriter writer)
        {
            // Will need to write out some sort of proxy, since it's not
            // possible to read back RowText until the attribute data has
            // been loaded from the database (and that only happens after
            // all features have been deserialized from the database).

            // To simplify things, let the proxy extend this class so
            // that it can pretend to be the real thing during ReadContent.

            // The XmlContentWriter should see that this class implements IXmlAlternateContent,
            // directing the WriteContent call through an instance of the alternate content
            // class. So if we end up here and we DON'T have an instance of the alternate,
            // then something is wrong.

            if (this is RowTextContent)
                base.WriteContent(writer);
            else
                throw new InvalidOperationException("Attempt to write content directly");
        }
         */

        /// <summary>
        /// Obtains an instance of the content object that can be persisted in the
        /// database. On deserialization, the alternate will usually need to be
        /// converted into an instance of the original class (this is done by the
        /// <see cref="FeatureId.AddReference(Row)"/> method).
        /// </summary>
        /// <returns>The content to save to the database</returns>
        public IXmlContent GetAlternate()
        {
            return new RowTextContent(this);
        }

        /// <summary>
        /// Loads the content of this class. This is called by
        /// <see cref="XmlContentReader"/> during deserialization from XML (just
        /// after the default constructor has been invoked).
        /// </summary>
        /// <param name="reader">The reading tool</param>
        /*
        public override void ReadContent(XmlContentReader reader)
        {
            Debug.Assert(this is RowTextContent);
            base.ReadContent(reader);
        }
         */
    }
}
