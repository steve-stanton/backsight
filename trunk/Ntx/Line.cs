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

namespace Ntx
{
    public class Line : Feature
    {
        #region Class data

        /// <summary>
        /// Positions for the line (may be allocated with unused positions at the end).
        /// </summary>
        Position[] m_Positions;

        /// <summary>
        /// The number of defined positions in the <c>m_Positions</c> array.
        /// </summary>
        int m_NumPosition;

        /// <summary>
        /// Is it a topological arc?
        /// </summary>
        bool m_IsArc;

        /// <summary>
        /// Is it a circular curve?
        /// </summary>
        bool m_IsCurve;
 
        /// <summary>
        /// Radius for curve
        /// </summary>
        double m_Radius;

        /// <summary>
        /// Centre of curve
        /// </summary>
        Position m_Centre;

        /// <summary>
        /// Beginning of curve
        /// </summary>
        Position m_BC;

        /// <summary>
        /// End of curve
        /// </summary>
        Position m_EC;

        #endregion

        internal Line()
        {
            m_Positions = null;
            m_IsArc = false;
            m_IsCurve = false;
            m_Radius = 0.0;
            m_Centre = m_BC = m_EC = null;
        }

        /// <summary>
        /// The number of points defining the line
        /// </summary>
        int Count
        {
            //get { return (m_Positions==null ? 0 : m_Positions.Length); }
            get { return m_NumPosition; }
        }

	    public bool IsTopologicalArc
        {
            get { return m_IsArc; }
            set { m_IsArc = value; }
        }

        public bool IsCurve
        {
            get { return m_IsCurve; }
            set { m_IsCurve = value; }
        }

        public double Radius
        {
            get { return m_Radius; }
            set { m_Radius = value; }
        }

        public Position Center
        {
            get { return m_Centre; }
            set { m_Centre = value; }
        }

        internal Position BC
        {
            get { return m_BC; }
            set { m_BC = value; }
        }

        internal Position EC
        {
            get { return m_EC; }
            set { m_EC = value; }
        }

	    public Position Position(int index)
        {
            if (m_Positions==null || index<0 || index>m_Positions.Length)
                throw new IndexOutOfRangeException();

            return m_Positions[index];
        }

        public int NumPosition
        {
            get { return m_NumPosition; }
            internal set { m_NumPosition = value; }
        }

        internal Position[] Positions
        {
            set { m_Positions = value; }
        }
    }
}
