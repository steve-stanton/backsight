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
    /// Something that can be persisted using an implementation of <see cref="EditSerializer"/>.
    /// </summary>
    /// <remarks>
    /// Classes that implement this interface are expected to handle deserialization by
    /// providing a constructor that accepts an instance of <see cref="EditDeserializer"/>
    /// (the intention is to make it possible to tag class members as <c>readonly</c> where
    /// that is applicable). Unfortunately, you can't specify constructors as part of an interface.
    /// </remarks>
    interface IPersistent
    {
        /// <summary>
        /// Writes the content of this instance to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        void WriteData(EditSerializer editSerializer);
    }
}
