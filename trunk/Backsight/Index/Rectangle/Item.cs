// <remarks>
// Copyright 2007 - Steve Stanton. This file is part of Backsight
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

namespace Backsight.Index.Rectangle
{
	/// <written by="Steve Stanton" on="15-DEC-2006" />
    /// <summary>
    /// An entry in the spatial index, consisting of a reference to a spatial
    /// object, as well as a reference to an object that represents its extent.
    /// This is considered useful because it may be relatively expensive to
    /// calculate the spatial object's extent (given that this will likely
    /// be done very repetitively while working with the spatial index).
    /// </summary>
    class Item
    {
        #region Class data

        /// <summary>
        /// The spatial object of interest
        /// </summary>
        private readonly ISpatialObject m_Object;

        /// <summary>
        /// The extent of the spatial object (expressed in a form that
        /// is easily accessible to the spatial index)
        /// </summary>
        private readonly Extent m_Window;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Item</c> for a spatial object. After
        /// creating an instance, the expectation is that you will add
        /// the item into a spatial index.
        /// </summary>
        /// <param name="so">The spatial object that you intend to add
        /// to a spatial index</param>
        internal Item(ISpatialObject so)
        {
            m_Object = so;
            m_Window = new Extent(so.Extent);
        }

        #endregion

        /// <summary>
        /// The extent of the spatial object, expressed in a form that
        /// can be easily worked with by the spatial index. For performance
        /// reasons, use this property instead of a call to <c>Item.Extent</c>.
        /// </summary>
        internal Extent Window
        {
            get { return m_Window; }
        }

        /// <summary>
        /// The spatial object associated with this item. Note that if you need
        /// to obtain the spatial extent of the object, it is preferable to
        /// use the <c>Window</c> property (since a call to <c>Item.Extent</c>
        /// is not necessarily cheap).
        /// </summary>
        internal ISpatialObject SpatialObject
        {
            get { return m_Object; }
        }
    }
}
