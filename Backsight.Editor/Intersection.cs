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
        /// The lines passing through this intersection. In most cases, this
        /// will probably involve just two lines.
        /// </summary>
        readonly List<LineFeature> m_Lines;

        /// <summary>
        /// Flag bits relating to the intersection
        /// </summary>
        IntersectionFlag m_Flag;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <c>Intersection</c> at the specified position.
        /// </summary>
        /// <param name="p">The position of the intersection</param>
        internal Intersection(PointGeometry p)
            : base(p)
        {
            m_Lines = new List<LineFeature>(2);
            m_Flag = 0;
        }

        #endregion

        /// <summary>
        /// Associates this intersection with the specified line.
        /// </summary>
        /// <param name="line">The line that passes through this intersection (not null)</param>
        /// <exception cref="ArgumentNullException">If the specified line is null</exception>
        internal void Add(LineFeature line)
        {
            if (line == null)
                throw new ArgumentNullException();

            if (!m_Lines.Contains(line))
                m_Lines.Add(line);
        }

        /// <summary>
        /// Removes the specified line from this intersection.
        /// </summary>
        /// <param name="line">The line to remove</param>
        /// <returns>True if the line was removed. False if it was not associated
        /// with this intersection</returns>
        internal bool Remove(LineFeature line)
        {
            return m_Lines.Remove(line);
        }

        /// <summary>
        /// The dividers that start or end at this intersection. If a divider
        /// starts and also ends here, it should appear in the returned array just once.
        /// </summary>
        public IDivider[] IncidentDividers() // ITerminal
        {
            List<IDivider> result = new List<IDivider>(m_Lines.Count*2);

            foreach (LineFeature line in m_Lines)
                line.AddIncidentDividers(result, this);

            return result.ToArray();
        }

        /// <summary>
        /// Go through each divider that is incident on this terminal, marking adjacent
        /// polygons for deletion.
        /// </summary>
        public void MarkPolygons() // ITerminal
        {
            foreach (IDivider d in IncidentDividers())
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="line"></param>
        internal void OnLineDeactivation(LineFeature line)
        {
            // Remove the reference the intersection has to the line
            Remove(line);

            // If the intersection now refers only to one line, it's no longer
            // an intersection, so remove it from the spatial index and merge
            // the sections incident on the intersection.
            if (m_Lines.Count<=1)
            {
                if (IsIndexed)
                {
                    CadastralMapModel map = line.MapModel;
                    CadastralIndex index = (CadastralIndex)map.Index;
                    index.RemoveIntersection(this);
                }

                if (m_Lines.Count>0)
                {
                    Topology t = m_Lines[0].Topology;
                    if (t!=null)
                        t.MergeSections(this);
                }
            }
        }
    }
}
