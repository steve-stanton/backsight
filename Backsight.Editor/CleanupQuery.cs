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
using System.Collections.Generic;
using System.Diagnostics;

//using Backsight.

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="08-SEP-2007" />
    /// <summary>
    /// Query spatial index in order to perform cleanup after any sort of edit.
    /// </summary>
    class CleanupQuery
    {
        #region Class data

        /// <summary>
        /// The model that's being cleaned.
        /// </summary>
        readonly CadastralMapModel m_Model;

        /// <summary>
        /// Features that have been moved (and which have not been marked for deletion).
        /// </summary>
        readonly List<Feature> m_Moves;

        /// <summary>
        /// Items that have been deleted.
        /// </summary>
        readonly List<ISpatialObject> m_Deletions;

        /// <summary>
        /// Window corresponding to data that has been marked for deletion.
        /// </summary>
        readonly Window m_UpdateWindow;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>CleanupQuery</c> and executes it.
        /// </summary>
        /// <param name="model">The model to clean</param>
        internal CleanupQuery(CadastralMapModel model)
        {
            if (model==null)
                throw new ArgumentNullException();

            m_Model = model;
            m_UpdateWindow = new Window();
            m_Deletions = new List<ISpatialObject>(100);
            m_Moves = new List<Feature>(100);

            // Cleanup features
            model.Index.QueryWindow(null, SpatialType.Feature, CleanupFeature);

            // Cleanup polygons
            model.Index.QueryWindow(null, SpatialType.Polygon, CleanupPolygon);

            // Remove stuff from spatial index if it's been deleted
            IEditSpatialIndex index = (IEditSpatialIndex)model.Index;
            foreach (ISpatialObject o in m_Deletions)
            {
                m_UpdateWindow.Union(o.Extent);
                bool isRemoved = index.Remove(o);
                Debug.Assert(isRemoved);
            }
        }

        #endregion

        /// <summary>
        /// Features that have been moved.
        /// </summary>
        internal List<Feature> Moves
        {
            get { return m_Moves; }
        }

        /// <summary>
        /// Delegate that's called whenever the index finds a feature
        /// </summary>
        /// <param name="item">The item to process</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool CleanupFeature(ISpatialObject item)
        {
            Debug.Assert(item is Feature);
            Feature f = (Feature)item;

            if (f.IsInactive)
                m_Deletions.Add(f);
            else if (f.IsMoved)
                m_Moves.Add(f);

            f.Clean();
            return true;
        }

        /// <summary>
        /// Delegate that's called whenever the index finds a polygon
        /// </summary>
        /// <param name="item">The item to process</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool CleanupPolygon(ISpatialObject item)
        {
            Debug.Assert(item is Ring);
            Ring r = (Ring)item;

            if (r.IsDeleted)
                m_Deletions.Add(r);

            r.Clean();
            return true;
        }
    }
}
