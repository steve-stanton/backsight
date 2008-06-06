/// <remarks>
/// Copyright 2008 - Steve Stanton. This file is part of Backsight
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


namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="05-FEB-2008" />
    /// <summary>
    /// Query spatial index to locate a label inside a polygon. This is an experiment to see
    /// if it's faster to locate the label, given the polygon (since the alternative approach
    /// (locate the polygon, given the label) seems to be a bit slow).
    /// <para/>
    /// To be used in conjunction with the other approach. This technique makes sense when
    /// dealing with things like property lots, but not if you're dealing with huge street
    /// network polygons. For that reason, it will ignore polygons that contain any islands.
    /// </summary>
    class FindPolygonLabelQuery
    {
        #region Class data

        /// <summary>
        /// The polygon of interest
        /// </summary>
        private readonly Polygon m_Polygon;

        /// <summary>
        /// The label that's been found
        /// </summary>
        private TextFeature m_Result;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>FindOverlapsQuery</c> (and executes it). The result of the query
        /// can then be obtained through the <c>Result</c> property.
        /// </summary>
        /// <param name="index">The spatial index to search</param>
        /// <param name="closedShape">The closed shape defining the search area.</param>
        /// <param name="spatialType">The type of objects to look for.</param>
        internal FindPolygonLabelQuery(ISpatialIndex index, Polygon polygon)
        {
            m_Polygon = polygon;
            m_Result = null;

            // Could try closed shape...

            if (!polygon.HasAnyIslands)
                index.QueryWindow(m_Polygon.Extent, SpatialType.Text, OnTextFound);
        }

        #endregion

        /// <summary>
        /// Delegate that's called whenever the index finds text with a window that overlaps the query window
        /// </summary>
        /// <param name="item">The item to process (expected to be some sort of <c>TextFeature</c>)</param>
        /// <returns>True (always), indicating that the query should continue.</returns>
        private bool OnTextFound(ISpatialObject item)
        {
            // Ignore test that's already been built
            TextFeature text = (TextFeature)item;
            if (text.IsBuilt)
                return true;

            if (text.IsTopological)
            {
                // Get the label's reference position.
                IPointGeometry posn = text.GetPolPosition();

                // Try to find enclosing polygon
                if (m_Polygon.IsEnclosing(posn))
                {
                    m_Result = text;
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// The result of the query (null if label was not found)
        /// </summary>
        internal TextFeature Result
        {
            get { return m_Result; }
        }
    }
}
