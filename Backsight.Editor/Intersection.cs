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

using Backsight.Geometry;

namespace Backsight.Editor
{
    /// <written by="Steve Stanton" on="24-OCT-2007" />
    /// <summary>
    /// An intersection of two or more lines.
    /// </summary>
    [Serializable]
    class Intersection : PointGeometry, ITerminal, IPoint
    {
        #region Class data

        /// <summary>
        /// The dividers incident on this intersection
        /// </summary>
        readonly List<IDivider> m_IncidentDividers;

        /// <summary>
        /// Flag bits relating to the intersection
        /// </summary>
        IntersectionFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Intersection</c> at the specified position.
        /// </summary>
        internal Intersection(PointGeometry p)
            : base(p)
        {
            m_IncidentDividers = new List<IDivider>();
            m_Flag = 0;
        }

        #endregion

        /// <summary>
        /// Adds the specified divider to this intersection
        /// </summary>
        /// <param name="d">The divider that is incident on this intersection (not null)</param>
        /// <exception cref="ArgumentNullException">If the specified divider is null</exception>
        /// <exception cref="ArgumentException">If the divider does not start or end at the position of
        /// this intersection</exception>
        internal void Add(IDivider d)
        {
            if (d==null)
                throw new ArgumentNullException();

            if (!(d.From.IsCoincident(this) || d.To.IsCoincident(this)))
                throw new ArgumentException("Divider does not terminate at intersection");

            m_IncidentDividers.Add(d);
        }

        /// <summary>
        /// The dividers that start or end at the terminal. If a divider
        /// starts and also ends at the terminal, it should appear in the
        /// returned array just once.
        /// </summary>
        public IDivider[] IncidentDividers() // ITerminal
        {
            return m_IncidentDividers.ToArray();
        }

        /// <summary>
        /// Go through each divider that is incident on this terminal, marking adjacent
        /// polygons for deletion.
        /// </summary>
        public void MarkPolygons() // ITerminal
        {
            foreach (IDivider d in m_IncidentDividers)
                Topology.MarkPolygons(d);
        }

        /// <summary>
        /// The spatial type of an intersection is <see cref="SpatialType.Point"/>
        /// </summary>
        public SpatialType SpatialType // ISpatialObject
        {
            get { return SpatialType.Point; }
        }

        /// <summary>
        /// Is a flag bit set?
        /// </summary>
        /// <param name="flag">The flag(s) to check for (may be a combination of more
        /// than one flag)</param>
        /// <returns>True if any of the supplied flag bits are set</returns>
        bool IsFlagSet(IntersectionFlag flag)
        {
            return ((m_Flag & flag)!=0);
        }

        /// <summary>
        /// Sets flag bit(s)
        /// </summary>
        /// <param name="flag">The flag bit(s) to set</param>
        /// <param name="setting">True to set, false to clear</param>
        void SetFlag(IntersectionFlag flag, bool setting)
        {
            if (setting)
                m_Flag |= flag;
            else
                m_Flag &= (~flag);
        }

        /// <summary>
        /// Has this intersection been spatially indexed?
        /// </summary>
        internal bool IsIndexed
        {
            get { return IsFlagSet(IntersectionFlag.Indexed); }
            set { SetFlag(IntersectionFlag.Indexed, value); }
        }

        /// <summary>
        /// The geometry for this point is <c>this</c>
        /// </summary>
        public new IPointGeometry PointGeometry // IPoint
        {
            get { return this; }
        }
    }
}
