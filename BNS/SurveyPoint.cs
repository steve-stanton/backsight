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
using System.Data;

using Backsight;
using Backsight.Geometry;

namespace BNS
{
    class SurveyPoint : ISpatialObject, IPoint, IPosition
    {
        #region Static

        static ILength HEIGHT = new Length(100.0);

        #endregion

        #region Class data

        readonly DataRow m_Row;

        readonly IPointGeometry m_Position;

        #endregion

        #region Constructors

        internal SurveyPoint(DataRow row, IPosition p)
        {
            m_Row = row;
            m_Position = PositionGeometry.Create(p);
        }

        #endregion

        #region ISpatialObject Members

        public SpatialType SpatialType
        {
            get { return SpatialType.Point; }
        }

        public void Render(ISpatialDisplay display, IDrawStyle style)
        {
            style.Render(display, this); // see ViewStyle implementation
        }

        public IWindow Extent
        {
            get { return new Window(m_Position, m_Position); }
        }

        public ILength Distance(IPosition point)
        {
            double d = BasicGeom.Distance(m_Position, point);
            return new Length(d);
        }

        #endregion

        #region IPoint Members

        public IPointGeometry Geometry
        {
            get { return m_Position; }
        }

        #endregion

        #region IPosition Members

        public double X
        {
            get { return m_Position.X; }
        }

        public double Y
        {
            get { return m_Position.Y; }
        }

        public bool IsAt(IPosition p, double tol)
        {
            return Position.IsCoincident(this, p, tol);
        }

        #endregion

        internal PointStatus Status
        {
            get { return (PointStatus)m_Row["status"]; }
        }
    }
}
