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

using System;

namespace Backsight.Editor
{
    /// <summary>
    /// A forward reference that relates to an array of items.
    /// </summary>
    class ForwardFeatureRefArray : ForwardRef
    {
        #region Class data

        /// <summary>
        /// The object that makes the forward-reference (not null).
        /// </summary>
        IFeatureRefArray ReferenceFrom { get; set; }

        ForwardRefArrayItem[] Items { get; set; }

        #endregion


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ForwardFeatureRefArray"/> class.
        /// </summary>
        /// <param name="referenceFrom">The object that makes the forward-reference (not null).</param>
        /// <param name="field">The ID of the persistent array field.</param>
        /// <param name="items">The items that need to be resolved.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="referenceFrom"/> is not defined.</exception>
        internal ForwardFeatureRefArray(IFeatureRefArray referenceFrom, DataField field, ForwardRefArrayItem[] items)
            : base(field)
        {
            if (referenceFrom == null || items == null)
                throw new ArgumentNullException();

            if (items.Length == 0)
                throw new ArgumentException();

            ReferenceFrom = referenceFrom;
            Items = items;
        }

        #endregion

        internal override void Resolve(CadastralMapModel mapModel)
        {
            foreach (ForwardRefArrayItem item in Items)
            {
                item.Feature = mapModel.Find<Feature>(item.InternalId);
                if (item.Feature == null)
                    throw new ApplicationException("Cannot locate forward reference " + item.InternalId);
            }

            ReferenceFrom.ApplyFeatureRefArray(Field, Items);
        }
    }
}
