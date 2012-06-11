// <remarks>
// Copyright 2012 - Steve Stanton. This file is part of Backsight
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
    /// <summary>
    /// Something that makes a reference to a spatial feature.
    /// </summary>
    /// <remarks>This interface was created to handle forward-references that might be
    /// encountered when loading data that originates from the old CEdit system.</remarks>
    interface IFeatureRef
    {
        /// <summary>
        /// Ensures that a persistent field has been associated with a spatial feature.
        /// </summary>
        /// <param name="field">A tag associated with the item</param>
        /// <param name="feature">The feature to assign to the field (not null).</param>
        /// <returns>True if a matching field was processed. False if the field is not known to this
        /// class (may be known to another class in the type hierarchy).</returns>
        bool ApplyFeatureRef(DataField field, Feature feature);
    }
}
