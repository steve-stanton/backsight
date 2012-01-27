// <remarks>
// Copyright 2011 - Steve Stanton. This file is part of Backsight
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
using System.IO;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="01-NOV-2011" />
    /// <summary>
    /// Implementation of <see cref="IEditReader"/> that loads edits using an instance of
    /// <see cref="System.IO.TextReader"/>.
    /// </summary>
    class TextEditReader : IEditReader
    {
        #region Class data

        /// <summary>
        /// The mechanism for reading the text.
        /// </summary>
        TextReader m_Reader;

        /// <summary>
        /// The line of text AFTER the current line (used to peek ahead), broken into tokens
        /// that are separated by commas.
        /// </summary>
        string[] m_NextLine;

        /// <summary>
        /// The index of the next element in <c>m_NextLine</c> that will be returned via
        /// a call to <see cref="ReadNextLine"/>.
        /// </summary>
        int m_NextIndex;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditReader"/> class.
        /// </summary>
        [Obsolete]
        internal TextEditReader()
        {
            m_Reader = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditReader"/> class
        /// that utilizes the supplied reader.
        /// </summary>
        /// <param name="reader">The mechanism for reading the text (not null).</param>
        internal TextEditReader(TextReader reader)
        {
            if (reader == null)
                throw new ArgumentNullException();

            m_Reader = reader;
            ReadNextLine();
        }

        #endregion

        /// <summary>
        /// Reads any text that precedes the data values for an object.
        /// </summary>
        public void ReadBeginObject()
        {
            ReadLiteralLine("{");
        }

        /// <summary>
        /// Reads any text that should follow the data values for an object.
        /// </summary>
        public void ReadEndObject()
        {
            ReadLiteralLine("}");
        }

        /// <summary>
        /// Reads a text string that should appear on the next line of the storage medium.
        /// </summary>
        /// <param name="requiredString">The string that should appear next.</param>
        /// <exception cref="ArgumentException">If the required string was not found.</exception>
        void ReadLiteralLine(string requiredString)
        {
            string s = ReadNextLine();
            if (s != requiredString)
                throw new ArgumentException(String.Format("Expected {0}, found {1}", requiredString, s));
        }

        /// <summary>
        /// Attempts to read the next line from the storage medium.
        /// </summary>
        /// <remarks>
        /// The constructor should have made an initial call to obtain the very first line of text.
        /// Thereafter, calling this method will read the subsequent line and return the one that was
        /// previously read!
        /// </remarks>
        /// <returns>The next line of text, with any leading and trailing whitespace trimmed off
        /// (null if there is no more text).</returns>
        string ReadNextLine()
        {
            /*
            string result = m_NextLine;

            // You get back null on reaching the end
            m_NextLine = m_Reader.ReadLine();

            if (m_NextLine != null)
                m_NextLine = m_NextLine.Trim();

            return result;
            */
            string result = PeekNext;

            // Point to the array item we'll return next time
            m_NextIndex++;

            // If we've exhausted all the items that were present in the line, read the next full
            // line and split up any inline items.
            if (m_NextLine == null || m_NextIndex >= m_NextLine.Length)
            {
                string fullLine = m_Reader.ReadLine();

                if (fullLine == null)
                {
                    m_NextLine = null;
                    m_NextIndex = -1;
                }
                else
                {
                    // Ignore the possibility of embedded commas in name tags or values
                    m_NextLine = fullLine.Split(',');
                    m_NextIndex = 0;

                    for (int i = 0; i < m_NextLine.Length; i++)
                        m_NextLine[i] = m_NextLine[i].Trim();
                }
            }

            return result;
        }

        /// <summary>
        /// The next line that will be returned by a call to <see cref="ReadNextLine"/> (null
        /// if we have reached the end).
        /// </summary>
        string PeekNext
        {
            get { return (m_NextLine == null ? null : m_NextLine[m_NextIndex]); }
        }

        /// <summary>
        /// Reads the next data value from the text storage medium (but does not attempt
        /// to parse it).
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The text following the name tag (null if there's nothing there)</returns>
        string ReadValue(string name)
        {
            string s = ReadNextLine();
            if (!s.StartsWith(name))
                throw new ArgumentException(String.Format("Expected '{0}' but found '{1}'", name, s));

            int eqIndex = s.IndexOf('=');
            if (eqIndex < 0)
                return null;

            string result = s.Substring(eqIndex + 1);
            return (result.Length == 0 ? null : result);
        }

        #region IEditReader Members

        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The byte value that was read.</returns>
        public byte ReadByte(string name)
        {
            return Convert.ToByte(ReadValue(name));
        }

        /// <summary>
        /// Reads a 4-byte signed integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte value that was read.</returns>
        public int ReadInt32(string name)
        {
            return Convert.ToInt32(ReadValue(name));
        }

        /// <summary>
        /// Reads a 4-byte unsigned integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte unsigned value that was read.</returns>
        public uint ReadUInt32(string name)
        {
            return Convert.ToUInt32(ReadValue(name));
        }

        /// <summary>
        /// Reads an 8-byte signed integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 8-byte value that was read.</returns>
        public long ReadInt64(string name)
        {
            return Convert.ToInt64(ReadValue(name));
        }

        /// <summary>
        /// Reads an eight-byte floating-point value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>
        /// The 8-byte floating-point value that was read.
        /// </returns>
        public double ReadDouble(string name)
        {
            return Convert.ToDouble(ReadValue(name));
        }

        /// <summary>
        /// Reads a four-byte floating-point value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>
        /// The 4-byte floating-point value that was read.
        /// </returns>
        public float ReadSingle(string name)
        {
            return Convert.ToSingle(ReadValue(name));
        }

        /// <summary>
        /// Reads a one-byte boolean value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The boolean value that was read.</returns>
        public bool ReadBool(string name)
        {
            byte b = Convert.ToByte(ReadValue(name));
            return (b == 0 ? false : true);
        }

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The string that was read (null if nothing follows the name)</returns>
        public string ReadString(string name)
        {
            return ReadValue(name);
        }

        /// <summary>
        /// Reads a timestamp.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The timestamp that was read.</returns>
        public DateTime ReadDateTime(string name)
        {
            string s = ReadString(name);
            return DateTime.Parse(s);
        }

        /// <summary>
        /// Checks whether the next line of text refers to a specific name tag. Make a call to
        /// <see cref="ReadNextLine"/> to actually advance.
        /// </summary>
        /// <param name="name">The name tag to check for</param>
        /// <returns>True if the next line refers to the specified name tag</returns>
        public bool IsNextField(string name)
        {
            string s = PeekNext;

            if (String.IsNullOrEmpty(s))
                return false;

            int eqIndex = s.IndexOf('=');
            if (eqIndex > 0)
            {
                if (eqIndex != name.Length)
                    return false;

                string result = s.Substring(0, name.Length);
                return (result == name);
            }

            return (s == name);
        }

        #endregion
    }
}
