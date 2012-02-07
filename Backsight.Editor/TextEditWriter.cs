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
using System.Collections.Generic;
using Backsight.Environment;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="31-OCT-2011" />
    /// <summary>
    /// Implementation of <see cref="IEditWriter"/> that saves edits using an instance of
    /// <see cref="System.IO.TextWriter"/>.
    /// </summary>
    class TextEditWriter : IEditWriter, IDisposable
    {
        #region Class data

        /// <summary>
        /// The current indentation of lines written to the text output.
        /// </summary>
        string m_Indent;

        /// <summary>
        /// The mechanism for writing the text.
        /// </summary>
        TextWriter m_Writer;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditWriter"/> class that
        /// writes to a string using an instance of <see cref="System.IO.StringWriter"/>.
        /// </summary>
        internal TextEditWriter()
            : this(new StringWriter())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditWriter"/> class that
        /// writes to a file. The file will be closed by <see cref="Dispose"/>.
        /// </summary>
        /// <param name="fileName">Name of the file to create.</param>
        internal TextEditWriter(string fileName)
            : this(File.CreateText(fileName))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextEditWriter"/> class that
        /// uses the supplied text writer.
        /// </summary>
        /// <param name="writer">The mechanism for writing the text (not null).</param>
        /// <exception cref="ArgumentNullException">If <paramref name="writer"/> is null.</exception>
        TextEditWriter(TextWriter writer)
        {
            if (writer == null)
                throw new ArgumentNullException();

            m_Indent = String.Empty;
            m_Writer = writer;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting
        /// unmanaged resources. This will dispose of the associated <see cref="System.IO.TextWriter"/>.
        /// </summary>
        public void Dispose()
        {
            if (m_Writer != null)
            {
                m_Writer.Dispose();
                m_Writer = null;
            }
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// The result of calling <see cref="System.IO.TextWriter.ToString"/> on the
        /// associated text writing mechanism (a blank string if <see cref="Dispose"/> has
        /// been called).
        /// </returns>
        public override string ToString()
        {
            if (m_Writer == null)
                return String.Empty;
            else
                return m_Writer.ToString();
        }

        #region IEditWriter Members

        /// <summary>
        /// Writes an unsigned byte to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The unsigned byte to write.</param>
        public void WriteByte(string name, byte value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes a four-byte signed integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte signed integer to write.</param>
        public void WriteInt32(string name, int value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes a four-byte unsigned integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        public void WriteUInt32(string name, uint value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes an eight-byte signed integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The eight-byte signed integer to write.</param>
        public void WriteInt64(string name, long value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes an eight-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The eight-byte floating-point value to write.</param>
        public void WriteDouble(string name, double value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes an four-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte floating-point value to write.</param>
        public void WriteSingle(string name, float value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes a one-byte boolean value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The boolean value to write (0 or 1).</param>
        public void WriteBool(string name, bool value)
        {
            WriteValue(name, (value ? "1" : "0"));
        }

        /// <summary>
        /// Writes a string to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The string to write (if a null is supplied, just the name tag will be written).</param>
        public void WriteString(string name, string value)
        {
            WriteValue(name, value);
        }

        /// <summary>
        /// Writes a timestamp to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="when">The timestamp to write</param>
        public void WriteDateTime(string name, DateTime value)
        {
            WriteValue(name, value.ToString("s"));
        }

        /// <summary>
        /// Writes an internal ID to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="id">The internal ID to write</param>
        public void WriteInternalId(string name, InternalIdValue id)
        {
            WriteValue(name, id.ToString());
        }

        #endregion

        /// <summary>
        /// Writes an object to text by calling its implementation of <see cref="System.Object.ToString"/>.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The object to write (if a null is supplied, just the name tag will be written).</param>
        void WriteValue(string name, object value)
        {
            if (value == null)
                m_Writer.WriteLine("{0}{1}", m_Indent, name);
            else
                m_Writer.WriteLine("{0}{1}={2}", m_Indent, name, value.ToString());

            // In BinaryEditWriter, could use System.BitConverter class to get byte array
        }

        /// <summary>
        /// Writes the text that precedes the data values for an object.
        /// </summary>
        public void WriteBeginObject()
        {
            WriteLiteral("{");
            m_Indent += '\t';
        }

        /// <summary>
        /// Writes the text that follows the data values for an object.
        /// </summary>
        public void WriteEndObject()
        {
            m_Indent = m_Indent.Substring(0, m_Indent.Length - 1);
            WriteLiteral("}");
        }

        /// <summary>
        /// Writes a text string (preceded by the current indent).
        /// </summary>
        /// <param name="value">The string to write.</param>
        void WriteLiteral(string value)
        {
            m_Writer.WriteLine("{0}{1}", m_Indent, value);
        }
    }
}
