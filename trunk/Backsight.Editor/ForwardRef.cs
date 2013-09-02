// <remarks>
// Copyright 2013 - Steve Stanton. This file is part of Backsight
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
    /// A reference to something that has not yet been created. This is utilized by code
    /// that handles the forward-references that might be encountered when loading data
    /// originating from the old CEdit system.
    /// </summary>
    /// <remarks>This is the base class for <see cref="ForwardFeatureRef"/> and <see cref="ForwardFeatureRefArray"/></remarks>
    abstract class ForwardRef
    {
        #region Class data

        /// <summary>
        /// The ID of the persistent field.
        /// </summary>
        internal DataField Field { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardFeatureRef"/> class.
        /// </summary>
        /// <param name="field">The ID of the persistent field.</param>
        internal ForwardRef(DataField field)
        {
            Field = field;
        }

        #endregion

        /// <summary>
        /// Attempts to resolves this forward reference.
        /// </summary>
        /// <param name="mapModel">The map model that should now contain the relevant features.</param>
        abstract internal void Resolve(CadastralMapModel mapModel);
    }
}
