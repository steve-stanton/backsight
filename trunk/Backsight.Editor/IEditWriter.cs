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

using Backsight.Environment;

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
    /// Having said the above, there is nothing to say that an implementation will actually do anything
    /// with the supplied name tags. For example, a binary writer that expects a strict arrangement of
    /// data may simply write the data values.
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
        /// <param name="value">The four-byte signed integer to write. </param>
        void WriteInt32(string name, int value);

        /// <summary>
        /// Writes an entity type to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The entity type to write. </param>
        void WriteEntity(string name, IEntity entity);

        /// <summary>
        /// Writes a four-byte unsigned integer to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte unsigned integer to write. </param>
        void WriteUInt32(string name, uint value);

        /// <summary>
        /// Writes an eight-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The eight-byte floating-point value to write. </param>
        void WriteDouble(string name, double value);

        /// <summary>
        /// Writes an four-byte floating-point value to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The four-byte floating-point value to write. </param>
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
        /// <param name="value">The string to write</param>
        void WriteString(string name, string value);

        /// <summary>
        /// Write a value in radians to a storage medium.
        /// </summary>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The radian value to write</param>
        void WriteRadians(string name, RadianValue value);

        /// <summary>
        /// Writes the content of an object to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of object being written (as it is known to the instance
        /// that contains it)</typeparam>
        /// <param name="name">A name tag for the item</param>
        /// <param name="value">The object to write (may be null)</param>
        void WriteObject<T>(string name, T value) where T : IPersistent;

        /// <summary>
        /// Writes a reference to a spatial feature to a storage medium.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature being written</typeparam>
        /// <param name="name">A name tag for the item</param>
        /// <param name="feature">The feature that is referenced.</param>
        void WriteFeature<T>(string name, T feature) where T : Feature;
    }
}
