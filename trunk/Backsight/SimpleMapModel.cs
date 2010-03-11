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

namespace Backsight
{
    /// <written by="Steve Stanton" on="01-SEP-2006" />
    /// <summary>
    /// A map consisting of a single data file that has an undefined coordinate system.
    /// </summary>
    public class SimpleMapModel : ISpatialModel
    {
        #region Class data

        /// <summary>
        /// The data for this model (never null)
        /// </summary>
        private readonly ISpatialData m_Data;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>SimpleMapModel</c> that refers to the specified data.
        /// </summary>
        /// <param name="data">The data for the model (not null)</param>
        /// <exception cref="ArgumentNullException">If null data was supplied</exception>
        public SimpleMapModel(ISpatialData data)
        {
            if (data==null)
                throw new ArgumentNullException();

            m_Data = data;
        }

        #endregion
        /// <summary>
        /// A user-perceived name for the model.
        /// </summary>
        public string Name
        {
            get { return m_Data.Name; }
        }

        /// <summary>
        /// Draws this model on the specified display
        /// </summary>
        /// <param name="display">The display to render to</param>
        /// <param name="style">The display style to use</param>
        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            m_Data.Render(display, style);
        }

        /// <summary>
        /// Is this model currently empty?
        /// </summary>
        public bool IsEmpty
        {
            get { return m_Data.IsEmpty; }
        }

        /// <summary>
        /// The ground extent of the data in the model (null if the model is empty)
        /// </summary>
        public IWindow Extent
        {
            get { return m_Data.Extent; }
        }

        /// <summary>
        /// Locates the object closest to a specific position.
        /// </summary>
        /// <param name="p">The search position</param>
        /// <param name="radius">The search radius</param>
        /// <param name="types">The type(s) of object to look for</param>
        /// <returns>The closest object of the requested type (null if nothing found)</returns>
        public ISpatialObject QueryClosest(IPosition p, ILength radius, SpatialType types)
        {
            return m_Data.QueryClosest(p, radius, types);
        }

        /// <summary>
        /// Null (always), indicating that the coordinate system for the model is unknown.
        /// </summary>
        public virtual ISpatialSystem SpatialSystem
        {
            get { return null; }
        }
    }
}
