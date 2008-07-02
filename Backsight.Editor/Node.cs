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
using System.Collections.Generic;

namespace Backsight.Editor
{
    /// <summary>
    /// A node represents the geometry that may be shared by one or more
    /// instances of <see cref="PointFeature"/>.
    /// </summary>
    /// <remarks>
    /// This class currently extends <see cref="PointGeometry"/>, which implies that the
    /// position can only be 2D. If there is ever a need for 3D positions, it should be
    /// relatively easy to relate the node to the geometry by association instead (or
    /// let <c>PointGeometry</c> hold a nullable Z-value). The only reason for not doing so
    /// today is that I don't want to bloat the object model unecessarily.
    /// </remarks>
    class Node : PointGeometry, ITerminal
    {
        #region Class data

        /// <summary>
        /// The points that share this node.
        /// </summary>
        readonly List<PointFeature> m_Points;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class with sufficient space
        /// to hold references to the specified number of points.
        /// </summary>
        /// <param name="pointCount">The number of points that will be referenced</param>
        internal Node(uint pointCount)
            : base()
        {
            m_Points = new List<PointFeature>((int)pointCount);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Node"/> class, with sufficient space
        /// to reference a single point feature. Make a subsequent call to <see cref="AddSharedPoint">
        /// to associate the new node with the point.
        /// </summary>
        /// <param name="g">The position of the node</param>
        internal Node(PointGeometry g)
            : base(g)
        {
            m_Points = new List<PointFeature>(1);
        }

        #endregion

        #region ITerminal Members

        /// <summary>
        /// The dividers that start or end at the terminal. If a divider
        /// starts and also ends at the terminal, it should appear in the
        /// returned array just once.
        /// </summary>
        /// <returns></returns>
        public IDivider[] IncidentDividers()
        {
            // In the vast majority of cases, points will NOT share geometry with
            // anything else, so cover that case
            if (m_Points.Count==1)
                return m_Points[0].IncidentDividers();

            // Cover case where multiple points share this node (the structure here is a bit
            // rough, but cleaning it up would require messing with Feature references, which
            // is a bit sensitive)
            List<IDivider> result = new List<IDivider>(4);

            foreach (PointFeature p in m_Points)
                result.AddRange(p.IncidentDividers());

            return result.ToArray();
        }

        #endregion

        /// <summary>
        /// Associates this node with an additional feature
        /// </summary>
        /// <param name="p">The point making use of this node</param>
        /// <exception cref="ArgumentException">If the supplied point has a geometry that
        /// doesn't refer to this node</exception>
        internal void AttachPoint(PointFeature p)
        {
            if (!Object.ReferenceEquals(p.Geometry, this))
                throw new ArgumentException();

            m_Points.Add(p);
        }

        /// <summary>
        /// The number of points associated with this node
        /// </summary>
        internal uint PointCount
        {
            get { return (m_Points==null ? 0 : (uint)m_Points.Count); }
        }

        /// <summary>
        /// The first point associated with this node. When the information for a node
        /// is serialized to XML, the position is written only for the first point -
        /// subsequent points obtain their position via the first point.
        /// </summary>
        internal PointFeature FirstPoint
        {
            get { return (m_Points==null ? null : m_Points[0]); }
        }
    }
}
