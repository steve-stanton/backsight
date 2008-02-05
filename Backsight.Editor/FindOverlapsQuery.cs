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
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-2008" />
    /// <summary>
    /// Query spatial index to locate spatial objects that overlap a closed shape. For use
    /// when selecting features.
    /// </summary>
    class FindOverlapsQuery
    {
        #region Class data

        /// <summary>
        /// The closed shape defining the search area.
        /// </summary>
        private readonly IPosition[] m_ClosedShape;

        /// <summary>
        /// The window of the data in <c>m_ClosedShape</c>
        /// </summary>
        private readonly IWindow m_Window;

        /// <summary>
        /// The overlaps found so far.
        /// </summary>
        private readonly List<ISpatialObject> m_Result;

        /// <summary>
        /// Overlapping points
        /// </summary>
        private readonly List<PointFeature> m_Points;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindOverlapsQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="closedShape">The closed shape defining the search area.</param>
        /// <param name="spatialType">The type of objects to look for.</param>
        internal FindOverlapsQuery(ISpatialIndex index, IPosition[] closedShape, SpatialType spatialType)
        {
            m_ClosedShape = closedShape;
            m_Window = new Window(m_ClosedShape);
            m_Points = new List<PointFeature>(100);
            m_Result = new List<ISpatialObject>(100);

            // If we are looking for points or lines, locate points that overlap. Note that
            // if the user does not actually want points in the result, we still do a point
            // search, since it helps with the selection of lines.
            if ((spatialType & SpatialType.Point)!=0 || (spatialType & SpatialType.Line)!=0)
            {
                index.QueryWindow(m_Window, SpatialType.Point, OnPointFound);

                // Remember the points in the result if the caller wants them
                if ((spatialType & SpatialType.Point)!=0)
                    m_Result.AddRange(m_Points.ToArray());
            }

            // Find lines (this automatically includes lines connected to the points we just found)
            if ((spatialType & SpatialType.Line)!=0)
                index.QueryWindow(m_Window, SpatialType.Line, OnLineFound);

            // Find any overlapping text
            if ((spatialType & SpatialType.Text)!=0)
                index.QueryWindow(m_Window, SpatialType.Text, OnTextFound);
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds a point that's inside the query window
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>PointFeature</c>)</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool OnPointFound(ISpatialObject item)
        {
            PointFeature p = (PointFeature)item;

            if (Geom.IsPointInClosedShape(m_ClosedShape, p))
                m_Points.Add(p);

            return true;
        }

        /// <summary>
        /// Delegate that's called whenever the index finds a line with a window that overlaps the query window
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>LineFeature</c>)</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool OnLineFound(ISpatialObject item)
        {
            LineFeature line = (LineFeature)item;


            return true;
        }

        /// <summary>
        /// Delegate that's called whenever the index finds text with a window that overlaps the query window
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>TextFeature</c>)</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool OnTextFound(ISpatialObject item)
        {
            TextFeature line = (TextFeature)item;


            return true;
        }

        /// <summary>
        /// The result of the query (may be an empty list).
        /// </summary>
        internal List<ISpatialObject> Result
        {
            get { return m_Result; }
        }
    }
}
