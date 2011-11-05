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

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="31-OCT-2011" />
    /// <summary>
    /// Methods that may be used to load the description of edits (previously written using
    /// an implementation of <see cref="IEditWriter"/>).
    /// <para/>
    /// Implemented by <see cref="TextEditReader"/>.
    /// </summary>
    interface IEditReader
    {
        /// <summary>
        /// Reads the next byte.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The byte value that was read.</returns>
        byte ReadByte(string name);

        /// <summary>
        /// Reads a 4-byte signed integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte value that was read.</returns>
        int ReadInt32(string name);

        /// <summary>
        /// Reads a 4-byte unsigned integer.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte unsigned value that was read.</returns>
        uint ReadUInt32(string name);

        /// <summary>
        /// Reads an eight-byte floating-point value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 8-byte floating-point value that was read.</returns>
        double ReadDouble(string name);

        /// <summary>
        /// Reads a four-byte floating-point value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The 4-byte floating-point value that was read.</returns>
        float ReadSingle(string name);

        /// <summary>
        /// Reads a one-byte boolean value.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The boolean value that was read.</returns>
        bool ReadBool(string name);

        /// <summary>
        /// Reads a string.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The string that was read.</returns>
        string ReadString(string name);

        /// <summary>
        /// Reads a value in radians.
        /// </summary>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The radian value that was read.</returns>
        RadianValue ReadRadians(string name);

        /// <summary>
        /// Reads the content of an object that implements <see cref="IPersistent"/>.
        /// </summary>
        /// <typeparam name="T">The type of object expected by the caller.</typeparam>
        /// <param name="name">A name tag associated with the object</param>
        /// <returns>The object that was read (may actually have a type that is derived
        /// from the supplied type).</returns>
        /// <remarks>
        /// In addition to implementing <see cref="IPersistent"/>, the Backsight implementation
        /// assumes that the created type will also provide a constructor that accepts an
        /// instance of the <see cref="IEditReader"/>.
        /// </remarks>
        T ReadObject<T>(string name) where T : IPersistent;

        /// <summary>
        /// Reads a reference to a spatial feature, using that reference to obtain the
        /// corresponding feature.
        /// </summary>
        /// <typeparam name="T">The type of spatial feature expected by the caller</typeparam>
        /// <param name="name">A name tag associated with the value</param>
        /// <returns>The feature that was read (null if not found). May actually have a type
        /// that is derived from the supplied type.</returns>
        /// <remarks>This does not create a brand new feature. Rather, it uses a reference
        /// to try to obtain a feature that should have already been created. To be able to
        /// do this, the implementation of <see cref="IEditReader"/> will probably have to
        /// maintain a collection of the features that have already been produced while
        /// working through an editing stream.
        /// </remarks>
        T ReadFeature<T>(string name) where T : Feature;

        /// <summary>
        /// Checks whether the next data item has a specific name tag. Make a call to any
        /// <c>Read</c> method to actually advance.
        /// </summary>
        /// <param name="name">The name tag to look for</param>
        /// <returns>True if the next data item has the specified name tag</returns>
        bool IsNextName(string name);
    }
}
