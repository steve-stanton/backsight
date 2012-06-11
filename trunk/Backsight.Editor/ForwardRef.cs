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

using System;

namespace Backsight.Editor
{
    /// <summary>
    /// A reference to a feature that has not yet been created. This is utilized by code
    /// that handles the forward-references that might be encountered when loading data
    /// originating from the old CEdit system.
    /// </summary>
    class ForwardRef
    {
        #region Class data

        /// <summary>
        /// The object that makes the forward-reference (not null).
        /// </summary>
        internal IFeatureRef ReferenceFrom { get; private set; }

        /// <summary>
        /// The ID of the persistent field.
        /// </summary>
        internal DataField Field { get; private set; }

        /// <summary>
        /// The internal ID that has been persisted for the field (relating to a feature
        /// that has not been created yet).
        /// </summary>
        internal InternalIdValue InternalId { get; private set; }

        // Need the type?

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardRef"/> class.
        /// </summary>
        /// <param name="referenceFrom">The object that makes the forward-reference (not null).</param>
        /// <param name="field">The ID of the persistent field.</param>
        /// <param name="iid">The internal ID that has been persisted for the field (relating to a feature
        /// that has not been created yet).</param>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceFrom"/> is not defined.</exception>
        internal ForwardRef(IFeatureRef referenceFrom, DataField field, InternalIdValue iid)
        {
            if (referenceFrom == null)
                throw new ArgumentNullException();

            ReferenceFrom = referenceFrom;
            Field = field;
            InternalId = iid;
        }

        #endregion
    }
}
