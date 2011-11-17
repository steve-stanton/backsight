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

namespace Backsight.Editor.Operations
{
    /// <summary>
    /// Interface for editing operations that may be revised using the
    /// Cadastral Editor's command update facility.
    /// </summary>
    interface IRevisable
    {
        /// <summary>
        /// Exchanges update items that were previously generated via an
        /// implementation of a method called <c>GetUpdateItems</c>.
        /// <para/>
        /// Every edit that implements <see cref="IRevisable"/> is expected to provide
        /// a method called <c>GetUpdateItems</c>. This isn't defined as part of
        /// the interface because the parameters passed to the method will vary
        /// from one edit to the next.
        /// <para/>
        /// The expectation is that if you obtain update items from one edit,
        /// you will later exchange the data with the same edit.
        /// </summary>
        /// <param name="data">The update data to apply to the edit (modified to
        /// hold the values that were previously defined for the edit)</param>
        void ExchangeData(UpdateItemCollection data);

        /// <summary>
        /// Writes updates for an editing operation to a persistent storage area.
        /// </summary>
        /// <param name="editSerializer">The mechanism for storing content.</param>
        /// <param name="data">The collection of changes to write</param>
        void WriteUpdateItems(EditSerializer editSerializer, UpdateItemCollection data);

        /// <summary>
        /// Reads back updates made to an editing operation.
        /// </summary>
        /// <param name="editDeserializer">The mechanism for reading back content.</param>
        /// <returns>The changes made to the edit</returns>
        UpdateItemCollection ReadUpdateItems(EditDeserializer editDeserializer);
    }
}
