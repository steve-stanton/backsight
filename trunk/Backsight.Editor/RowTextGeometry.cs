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
        // attribute field is enclosed in square brackets (e.g. [ID] means the
        // value of the ID column should be substituted).
        //
        // If the column name ends with a "+" character, it means the data value
        // should be used to perform a lookup on a domain table attached to that
        // column.
        //
        // Newlines can be inserted by specifying the string "\n". The text on each
        // successive line will be horizontally center-aligned.
        // 
        // TODO: Provide escape for []+ characters.
        //
        // Examples:
        //
        // "C.T. [Certificate_of_Title_Name]" specifies that the characters "C.T. " be followed by
        // the data contained in the <c>Certificate_of_Title_Name</c> column.
        //
        // "[Par_Lot_Type+] [Par_Lot_ID]\nParish of [Parish+]" specifies that the content of
        // the Par_Lot_Type column should be used to perform a lookup on an associated domain
        // table, followed by a space, then the value of the Par_Lot_ID column. On the next
        // line, the text "Parish of " will be followed by a lookup on the associated domain table.
        //
        //  "Public Lane" specifies that only this text be output

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
                return GetText(m_Row, id.FormattedKey, m_Template);
            }
        }

        static string GetText(Row row, string key, ITemplate template)
        {
            // The row must have a defined schema (and presumably the same
            // as the same as the template's schema, although we won't check).
            ITable schema = row.Table;
            if (schema == null)
                return null;

            StringBuilder result = new StringBuilder(100);
            string fmt = template.Format;
            int startIndex = 0;
            DataRow data = row.Data;

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
                    result.Append(s);

                    startIndex = endField+1;
                }
            }

            return result.ToString();
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

        /*
	CHARS* EntryName=0;
	CHARS AChar;
	LOGICAL IsOK = TRUE;
	INT4 digit;
	CHARS* charpos;
	INT4 fieldnum = 0;
	static CHARS* digits = "0123456789";

	CHARS* pbuffer = new CHARS[strlen(m_pFormat)+1];
	const UINT4 maxsize = pSchema->GetSchemaMemSize()+1;
	CHARS* pfieldata = new CHARS[maxsize];
	pbuffer[0]='\0';
	UINT2 index = strcspn(m_pFormat,"%");
	UINT2 numchars = index;
	UINT2 startpos = 0;
	UINT2 lastpos = strlen(m_pFormat);
	LOGICAL OnField = (index < lastpos);
	LOGICAL GetDigits;
	text.Empty(); // clear out string at start
	if(numchars)
	{// have found other chars before the %, copy them to output string
		strncpy(pbuffer,m_pFormat+startpos,numchars);
		pbuffer[numchars] = '\0';
		text += pbuffer;
		pbuffer[0]='\0';
	}
	while( IsOK && index < lastpos)
	{
		AChar = ' ';
		GetDigits = FALSE;
		index++; // next char pos after %
		if( m_pFormat[index]=='%') // next char is % - add it to output & go
		// past it
		{
			text += "%";
			index++;
		}
		else if ( m_pFormat[index]=='k'  ||
				m_pFormat[index]=='K') // key char - add it to output & go past it
		{
			// If the row does not have an ID, get the key from the
			// ID handle (if one was supplied).

			text += keystr;
			index++;
		}
		else if ( m_pFormat[index]=='v'  ||
				m_pFormat[index]=='n') // a number is attached to 
		{
			if ( m_pFormat[index]=='v' ) AChar = 'v';
			else AChar = 'n';
			GetDigits = TRUE;
			index++;
		}
		else // check for digit(s) and for them being in range
			// of fieldschemas
		{
			AChar = ' ';
			GetDigits = TRUE;
		}

		if(GetDigits)
		{
			fieldnum = 0;
			startpos = index;
			charpos = strchr(digits,m_pFormat[index]);
			while( index < lastpos &&  charpos )
			{
				digit = (charpos - digits);
				fieldnum = fieldnum*10 + digit;
				index++;
				charpos = strchr(digits,m_pFormat[index]);
			}
			if(index != startpos)
			{ // have found a valid number - use it
				fieldnum--; // change fieldnum to vector index
				if(fieldnum >= 0)
				{
					IsOK = (fieldnum < pSchema->ReturnNumFieldSchemas());
					OnField = FALSE;
				}
				else IsOK = FALSE;
				if( IsOK )
				{ // have got a valid fieldnumber , get the value specified
					if( AChar == ' ' ) // just get the data stored in field
					{
						IsOK = row.GetFieldChars(pfieldata,maxsize,fieldnum);
						if(IsOK) text += pfieldata;
					}
					else if( AChar == 'v' )
					{ // get the name of the entry in a list domain
						const CeDomain* pDomain =
							pSchema->ReturnFieldSchema(fieldnum)->GetDomain();
						IsOK = row.GetFieldChars(pfieldata,maxsize,fieldnum);
						os_string Errmess;
						IsOK = pDomain->GetEntryName(pfieldata,EntryName,Errmess);
						if(IsOK) text += EntryName;
						else ShowMessage(Errmess.c_str()); // output error
					}
					else if( AChar == 'n' ) // get the name of the field
						text += 
							pSchema->ReturnFieldSchema(fieldnum)->GetName();
				}
			}
			else IsOK = FALSE; // next char is not a numeric char
		}
		if(index < lastpos)
		{
			startpos = index; // 1st char pos after field number
			index += strcspn(m_pFormat+index,"%");
			numchars = index - startpos;
			if(numchars)
			{// have found other chars before the %, copy them to output
			//  string
				strncpy(pbuffer,m_pFormat+startpos,numchars);
				pbuffer[numchars] = '\0';
				text += pbuffer;
				pbuffer[0]='\0';
			}
		}
		OnField = (index < lastpos);
	}

	delete [ ] pfieldata;
	delete [ ] pbuffer;

	if( IsOK )
		return text.GetLength();
	else {
		text.Empty();
		return 0;
	}
         */

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
