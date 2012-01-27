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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="31-OCT-2011" />
    /// <summary>
    /// Methods that may be used to persist the description of edits. The data can be read back
    /// using an implementation of <see cref="IEditReader"/>.
    /// <para/>
    /// Implemented by <see cref="TextEditWriter"/></remarks>
    /// </summary>
    /// <remarks>
    /// Each writing method accepts a name tag to associate with the data item. Avoid punctuation characters in
    /// name tags, as they may have special meaning to the writer implementation (e.g. consider the likes of XML
    /// or JSON).
    /// <para/>
    /// Only basic data types should be mentioned as part of this interface. If you want to write any Backsight-specific
    /// types, you should provide an adapter method as part of the <see cref="EditSerializer"/> class.
    /// </remarks>
    interface IEditWriter
    {
        /// <summary>
        /// Writes an unsigned byte to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The unsigned byte to write.</param>
        void WriteByte(string name, byte value);

        /// <summary>
        /// Writes a four-byte signed integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte signed integer to write.</param>
        void WriteInt32(string name, int value);

        /// <summary>
        /// Writes a four-byte unsigned integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte unsigned integer to write.</param>
        void WriteUInt32(string name, uint value);

        /// <summary>
        /// Writes an eight-byte signed integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The eight-byte signed integer to write.</param>
        void WriteInt64(string name, long value);

        /// <summary>
        /// Writes an eight-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The eight-byte floating-point value to write.</param>
        void WriteDouble(string name, double value);

        /// <summary>
        /// Writes an four-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte floating-point value to write.</param>
        void WriteSingle(string name, float value);

        /// <summary>
        /// Writes a one-byte boolean value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The boolean value to write (0 or 1).</param>
        void WriteBool(string name, bool value);

        /// <summary>
        /// Writes a string to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The string to write (if a null is supplied, just the name tag will be written).</param>
        void WriteString(string name, string value);

        /// <summary>
        /// Writes a timestamp to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="when">The timestamp to write</param>
        void WriteDateTime(string name, DateTime when);

        /// <summary>
        /// Writes any text that precedes the data values for an object.
        /// </summary>
        void WriteBeginObject();

        /// <summary>
        /// Writes any text that follows the data values for an object.
        /// </summary>
        void WriteEndObject();
    }
}
