/// <remarks>
/// Copyright 2007 - Steve Stanton. This file is part of Backsight
///
/// Backsight is free software; you can redistribute it and/or modify it under the terms
/// of the GNU Lesser General Public License as published by the Free Software Foundation;
/// either version 3 of the License, or (at your option) any later version.
///
/// Backsight is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY;
/// without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
/// See the GNU Lesser General Public License for more details.
///
/// You should have received a copy of the GNU Lesser General Public License
/// along with this program. If not, see <http://www.gnu.org/licenses/>.
/// </remarks>

using System;

using Backsight.Environment;

namespace Backsight.Editor
{
	/// <written by="Steve Stanton" on="22-JUN-2007" />
    /// <summary>
    /// An entity type for a specific map.
    /// </summary>
    [Serializable]
    class MapEntity : EntityFacade
    {
        #region Class data

        /// <summary>
        /// The map this entity type is part of.
        /// </summary>
        [NonSerialized]
        CadastralMapModel m_Map;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>MapEntity</c>
        /// </summary>
        /// <param name="map">The map the entity type is part of (not null)</param>
        /// <param name="e">The data about the entity type (not null)</param>
        internal MapEntity(CadastralMapModel map, IEntity e)
            : base(e)
        {
            if (map==null || e==null)
                throw new ArgumentNullException();

            m_Map = map;
        }

        #endregion

        /// <summary>
        /// The map this entity type is part of.
        /// </summary>
        internal CadastralMapModel MapModel
        {
            get { return m_Map; }
            set
            {
                if (value==null)
                    throw new ArgumentNullException();

                m_Map = value;
            }
        }
    }
}
