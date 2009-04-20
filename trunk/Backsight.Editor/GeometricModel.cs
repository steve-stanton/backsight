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

using System;
using System.Collections.Generic;


namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="20-APR-2009" />
    /// <summary>
    /// A snapshot of the geometry in a map model (including polygon topology).
    /// </summary>
    [Serializable]
    class GeometricModel
    {
        #region Class data

        //Dictionary<InternalIdValue, Geo

        readonly List<Polygon> m_Polygons;

        #endregion

        #region Constructors

        internal GeometricModel(CadastralMapModel mapModel)
        {
            m_Polygons = new List<Polygon>();

        }

        #endregion

    }
}
